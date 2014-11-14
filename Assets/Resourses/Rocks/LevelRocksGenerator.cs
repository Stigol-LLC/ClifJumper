using System.Runtime.Remoting;
using UnityEngine;

using System.Collections.Generic;

public class LevelRocksGenerator : MonoBehaviour {

    private int flagMissCount;

    public float checkpointDistance = 20; // in m
    public float checkpointHeight;

    public List<GameObject> rocksCollectionList;

    private Queue<GameObject> rocksQueue;


    private float rocksHeight;
    private HeroCamera heroCamera;

    private RockController lastRock;

    private int nextCheckPointHeight;

    void Awake()
    {
        rocksQueue = new Queue<GameObject>();
        heroCamera = GameManager.HeroCamera;
        checkpointHeight = checkpointDistance;
    }

    public void FreezeCracksColliders() {
        foreach (GameObject rockGO in rocksQueue)
        {
            RockController rock = rockGO.GetComponent<RockController>();
            rock.FreezeCrackColliders();
        }   
    }

    public void UnFreezeCracksColliders() {
        foreach (GameObject rockGO in rocksQueue)
        {
            RockController rock = rockGO.GetComponent<RockController>();
            rock.UnFreezeCrackColliders();
        }   
    }

    public void Restart()
    {
        checkpointHeight = checkpointDistance;
        DestroyAllRocks();
    }

    public void DestroyAllRocks()
    {
        if (rocksQueue.Count == 0)
            return;

        do
        {
            GameObject rockPart = rocksQueue.Dequeue();
            Destroy(rockPart);
        } while (rocksQueue.Count > 0);
    }

    private GameObject generateRock(GameObject pattern,  out float newHeight, out float newRockHeight) {
        GameObject rock = Instantiate( pattern ) as GameObject;
        Vector3 firstBlockPosition = new Vector3(rock.transform.localPosition.x, rocksHeight + rock.GetComponent<RockController>().getBounds().size.y / 2, 0);
        rock.transform.parent = gameObject.transform;
        rock.transform.localPosition = firstBlockPosition;

        newRockHeight = rock.GetComponent<RockController>().getBounds().size.y;
        newHeight = rock.transform.localPosition.y + newRockHeight / 2;
        rocksQueue.Enqueue( rock );
    
        return rock;
    }

    private void generateCracks( RockGenerator rockGenerator ) {
//        Debug.Log("RocksHeight - " + GameManager.sceneController.hero.getHeight(rockGenerator.transform.position));

  //      if ( GameManager.sceneController.hero.getHeight( rockGenerator.transform.position) > checkpointHeight ) {
  //          flagMissCount = 2;
        float height = GameManager.sceneController.hero.getHeight( rockGenerator.transform.position );
            rockGenerator.GenerateCracks( 2, hasCoin(), checkpointHeight, hasFixedPoints( height ), getDifficulty(height) );
            checkpointHeight += checkpointDistance;
//        } else {
//            rockGenerator.GenerateCracks( 2, hasCoin(), -1 );
//        }

        

    }

    private bool hasFixedPoints(float height) {
        if ( height < 300 )
            return true;

        return false;
    }

    private int getDifficulty(float height )
    {
        
        return (int) (height / 100) + 1;

    }

    bool hasCoin() {

        flagMissCount--;

        if ( flagMissCount > 0 )
            return false;

        int randN = Random.Range( 0, 100 );

        if ( randN < 75 ) {
            return false;
        }

        return true;
    }

    private void generateStones( RockGenerator rockGenerator ) {
        rockGenerator.GenerateStones();
    }

    private void generateDecorCracks( RockGenerator rockGenerator ) {
        rockGenerator.GenerateDecorCracks(8);
    }

    private void generateDecorRockPictures( RockGenerator rockGenerator ) {
        rockGenerator.GenerateDecorRocksImage( 1 );
    }

    void generateRockElements(RockGenerator rockGenerator) {
        generateCracks( rockGenerator );
        generateStones( rockGenerator );
        generateDecorCracks( rockGenerator );
        generateDecorRockPictures( rockGenerator );
    }

    public void GenerateScreenRocks() {
        float blockRockHeight;
        if (rocksQueue.Count == 0) {

            rocksHeight = 0;
            GameObject firstRock = generateRock(
                    getLevelRock( 0 ),
                    out rocksHeight, out blockRockHeight );

       
            generateRockElements(firstRock.GetComponent<RockGenerator>());
        }


        float rockPosY;
        float genCameraHeight = GameManager.HeroCamera.getRockBounds().max.y;


        float topRocksPartHeight;
        do
        {
            GameObject newRock = generateRock( getLevelRock( 0 ), out rocksHeight, out blockRockHeight);

            RockController rock = newRock.GetComponent<RockController>();
            topRocksPartHeight = blockRockHeight / 2;
            rockPosY = newRock.transform.position.y;
            lastRock = rock;
            generateRockElements(rock.GetComponent<RockGenerator>());

        } while (rockPosY - topRocksPartHeight < genCameraHeight);
    }

    GameObject getLevelRock(int level)
    {
        //  int i = Random.Range( 0, 2 );

        GameObject rock = rocksCollectionList[0];
        if (rocksCollectionList.Count < level)
            rock = rocksCollectionList[level];


        return rock;
    }

    public GameObject getClothestRock( float posY ) {
        GameObject resultRockGO = rocksQueue.Peek();
//        Debug.Log( "Start find clothest rock" );
        foreach ( GameObject rockGO in rocksQueue ) {
            RockController rock = rockGO.GetComponent<RockController>();
            Bounds rockBounds = rock.getBounds();

            if ((posY < rockBounds.max.y) && (posY > rockBounds.min.y)) {
                resultRockGO = rockGO;
//                Debug.Log( "result rock posy - " + resultRockGO.transform.position.y );
                break;
                
            }
        }
        return resultRockGO;
    }


    void Update()
    {
        GameObject bottomRockGO = rocksQueue.Peek();
        RockController bottomRock = bottomRockGO.GetComponent<RockController>();

        Bounds rocksCameraBounds = heroCamera.getRockBounds();

        if (lastRock.gameObject.transform.position.y < rocksCameraBounds.max.y)
        {
            GenerateScreenRocks();
        }

        if (rocksCameraBounds.min.y > bottomRock.getBounds().max.y)
        {
            GameObject rockPart = rocksQueue.Dequeue();
            Destroy(rockPart);
        }
    }
}
