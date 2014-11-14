
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class RockGenerator : MonoBehaviour {

    
    public float crackEdgeDistance;

    public List<GameObject> catchCracksPatternsList;
    public List<GameObject> edgeStonesPatternsList;
    public List<GameObject> rockCracksPatternsList;
    public List<GameObject> rockPicturesPatternsList;

    private List<GameObject> cracksList;
    private List<GameObject> edgeStonesList;
    private List<GameObject> decorCracksList;
    private List<GameObject> rockPicturesList; 

    private Bounds rockBounds;

    void Awake() {
        cracksList = new List<GameObject>();
        edgeStonesList = new List<GameObject>();
        decorCracksList = new List<GameObject>();
        rockPicturesList = new List<GameObject>();
        rockBounds = renderer.bounds;
    }

    public void FreezeCracksCollider() {
        foreach ( GameObject crackGO in cracksList ) {
            CrackController crack = crackGO.GetComponent<CrackController>();
            crack.FreezeCollider();
        }
    }

    public void UnFreezeCracksColliders() {
        foreach (GameObject crackGO in cracksList)
        {
            CrackController crack = crackGO.GetComponent<CrackController>();
            crack.UnFreezeCollider();
        }
    }

    Vector3 getRandomScaleFlips(Vector3 localScale, bool flipX, bool flipY) {
        int kFlipX = 1;
        int kFlipY = 1;

        if ( flipX ) {
            int randFlip = Random.Range( 0, 2 );
            if ( randFlip < 1 )
                kFlipX = -1;
        }

        if (flipY) {
            int randFlip = Random.Range(0, 2);
            if (randFlip < 1)
                kFlipY = -1;
        }

        return new Vector3(localScale.x * kFlipX, localScale.y * kFlipY, localScale.z);
    }

    public void GenerateDecorRocksImage( int count ) {

        float rockHeight = rockBounds.size.y;
        float crackGenDist = (rockHeight - crackEdgeDistance * count * 2) / count;

        float genStartHeight = -rockHeight / 2 + crackEdgeDistance;
        float genEndHeight = genStartHeight + crackGenDist;

        for ( int i = 0; i < count; i++ ) {
            float rockGenHeight = Random.Range(genStartHeight, genEndHeight);

            GameObject pattern = getRandomDecorRockImage();
            GameObject decorImage = Instantiate( pattern ) as GameObject;

            RockPictureController imageController = decorImage.GetComponent<RockPictureController>();

            float startPosX = -rockBounds.size.x / 2 + imageController.bounds.size.x / 2;
            float endPosX = startPosX + rockBounds.size.x / 4;

            float posX = Random.Range( startPosX, endPosX );

            decorImage.transform.parent = gameObject.transform;
            decorImage.transform.localPosition = new Vector3(posX, rockGenHeight, -1);

            float randomRotation = Random.Range( 0, 360 );
            decorImage.transform.eulerAngles = new Vector3(0, 0, randomRotation);

            rockPicturesList.Add( decorImage );

            genStartHeight += crackEdgeDistance * 2 + crackGenDist;
            genEndHeight = genStartHeight + crackGenDist;
        }
    }

    GameObject getRandomDecorRockImage()
    {
        int randN = Random.Range(0, rockPicturesPatternsList.Count);
        return rockPicturesPatternsList[randN];
    }

    public void GenerateDecorCracks(int count) {

        float rockHeight = rockBounds.size.y;
        float crackGenDist = (rockHeight - crackEdgeDistance * count * 2) / count;

        float genStartHeight = -rockHeight / 2 + crackEdgeDistance;
        float genEndHeight = genStartHeight + crackGenDist;

        for ( int i = 0; i < count; i++ ) {
            float rockGenHeight = Random.Range(genStartHeight, genEndHeight);
           

            GameObject pattern = getRandomDecorCrack();
            GameObject decorCrack = Instantiate( pattern ) as GameObject;

            RockCrackGenerator crackGenerator = decorCrack.GetComponent<RockCrackGenerator>();
        //    float posX = - rockBounds.size.x / 2 + crackGenerator.getBounds().size.x / 2;

            float startPosX = -rockBounds.size.x / 2 + crackGenerator.getBounds().size.x / 2;
            float endPosX = startPosX - rockBounds.size.x / 10;

            float posX = Random.Range(startPosX, endPosX);

            decorCrack.transform.parent = gameObject.transform;
            decorCrack.transform.localPosition = new Vector3( posX, rockGenHeight, -5 );

            float randomRotation = Random.Range(-15, 15);
            decorCrack.transform.eulerAngles = new Vector3(0, 0, randomRotation);

        //    Vector3 crackLocalScale = decorCrack.transform.localScale;
          //  decorCrack.transform.localScale = getRandomScaleFlips( decorCrack.transform.localScale, true, true );
            decorCracksList.Add( decorCrack );

            genStartHeight += crackEdgeDistance * 2 + crackGenDist;
            genEndHeight = genStartHeight + crackGenDist;
        }
    }



    GameObject getRandomDecorCrack() {
        int randN = Random.Range( 0, rockCracksPatternsList.Count );
        return rockCracksPatternsList[ randN ];
    }

    public void GenerateCracks( int count, bool hasCoin, float flagHeight, bool isFirstCrackFixed, int maxDifficulty ) {
      
  //      bool hasRockFlag = false;

//        if ( flagHeight > 0 ) {
//            hasRockFlag = true;
//            Debug.Log( "Has flag" );
//        }

        float rockHeight = rockBounds.size.y;
        float crackGenDist = (rockHeight - crackEdgeDistance * count * 2) / count;
       
        float genStartHeight = -rockHeight / 2 + crackEdgeDistance;
        float genEndHeight = genStartHeight + crackGenDist;

       // Debug.Log( "Rock height - " + rockHeight + "   crack gen dist - " + crackGenDist );

        int coinIndex = -1;
        
        
        if (hasCoin )
            coinIndex = Random.Range( 1, count );

        for ( int i = 0; i < count; i++ ) {
            float rockGenHeight = genStartHeight;

            GameObject pattern;

            if ( ( i != 0 ) ||
                 ( !isFirstCrackFixed ) ) {
                rockGenHeight = Random.Range( genStartHeight, genEndHeight );
                pattern = getRandomCrack(maxDifficulty);
            } else {
                pattern = getRandomCrack( 1 );
            }


            //  rockGenHeight = genStartHeight;
            
            GameObject crack = Instantiate( pattern ) as GameObject;

            crack.transform.parent = gameObject.transform;
            crack.transform.localPosition = new Vector3(-rockBounds.size.x / 2, rockGenHeight, 0);

     //       if ( hasRockFlag == false ) 
            
     //       {

            if (i == coinIndex)
                crack.GetComponent<CrackController>().hasCoin = true;
            else {
                crack.GetComponent<CrackController>().hasCoin = false;
            }
              
//            } else {
//                crack.GetComponent<CrackController>().hasFlag = hasRockFlag;
//                hasRockFlag = false;
//            }

            cracksList.Add( crack );

            genStartHeight += crackEdgeDistance * 2 + crackGenDist;
            genEndHeight = genStartHeight + crackGenDist;
        }

    }



    GameObject getRandomCrack(int maxDifficulty) {
        List<GameObject> cracksDifficultyList = getCracksWithDifficulty( maxDifficulty );


        int randN = Random.Range(0, cracksDifficultyList.Count);
        return catchCracksPatternsList[randN];
    }

    List<GameObject> getCracksWithDifficulty( int maxDifficulty ) {
        List<GameObject> resultList = new List<GameObject>();

        foreach ( GameObject crackGO in catchCracksPatternsList ) {
            CrackController crack = crackGO.GetComponent<CrackController>();
            if (crack.difficulty <= maxDifficulty)
                resultList.Add( crackGO );
        }

        return resultList;

    }

    public void GenerateStones() {

        float localPosY = -rockBounds.size.y / 2;

        do {
            GameObject pattern = getRandomStone();
            GameObject stone = Instantiate( pattern ) as GameObject;
            localPosY += stone.GetComponent<EdgeStonesController>().getBounds().size.y / 2;

            stone.transform.parent = gameObject.transform;
            stone.transform.localPosition = new Vector3( -rockBounds.size.x / 2, localPosY, 0 );
            edgeStonesList.Add( stone );
            localPosY += stone.GetComponent<EdgeStonesController>().getBounds().size.y / 2;
        } while ( localPosY < rockBounds.size.y / 2 );
    }

    GameObject getRandomStone() {
        int randN = Random.Range( 0, edgeStonesPatternsList.Count );
        return edgeStonesPatternsList[ randN ];
    }

    public CrackController getClothestStartFixCrack(float posY) {
  
        GameObject resultCrackGO = cracksList[ 0 ];
//        float minDist = Mathf.Abs(posY - resultCrackGO.transform.position.y);
//
//        foreach (GameObject rockGO in cracksList)
//        {
//            float dist = Mathf.Abs(rockGO.transform.position.y - posY);
//            if (dist < minDist)
//            {
//                minDist = dist;
//                resultCrackGO = rockGO;
//            }
//        }
        return resultCrackGO.GetComponent<CrackController>();
    }

}
