using UnityEngine;
using System.Collections.Generic;

public class ParalaxBackGroundController : MonoBehaviour {

    private Vector2 heroCameraStartPosition;
    private Vector3 backgroundStartPosition;

    private Queue<BackGroundController> backGroundQueue;
    private BackGroundController _lastParalaxBackGround;

    public float moveKoef ;
 

    public GameObject backgroundGO;

    private HeroCamera heroCamera;

    private Vector3 prevParalaxPosition;
    private Vector3 paralaxPosition;

    void Awake() {
 
        backGroundQueue = new Queue<BackGroundController>();

        
    }

    public void InitBackGround( HeroCamera camera ) {
        heroCamera = camera;
        heroCameraStartPosition = heroCamera.transform.position;
    }

    public void StartBackground()
    {

        gameObject.transform.localPosition = new Vector3(0, 0, gameObject.transform.localPosition.z);
        DestroyQueueBackGrounds();

        backgroundStartPosition = transform.position;
        AddBackground( backgroundGO );

    }

    private void AddBackground(GameObject backGroundPattern)
    {
        GameObject newBackground = Instantiate(backGroundPattern) as GameObject;
        newBackground.SetActive(true);
        newBackground.transform.parent = gameObject.transform;

        BackGroundController newParalaxBackGroundController = newBackground.GetComponent<BackGroundController>();

        if (backGroundQueue.Count > 0)
        {
            newBackground.transform.localPosition = new Vector3(0,
                _lastParalaxBackGround.transform.localPosition.y + _lastParalaxBackGround.bounds.size.y / 2 + newParalaxBackGroundController.bounds.size.y / 2,
                0);

        }
        else
        {
            newBackground.transform.localPosition = new Vector3(0, 0, 0);
        }

        _lastParalaxBackGround = newParalaxBackGroundController;
        
        backGroundQueue.Enqueue(newParalaxBackGroundController);
    }

    void DestroyQueueBackGrounds()
    {
        if (backGroundQueue == null)
            backGroundQueue = new Queue<BackGroundController>();

        if (backGroundQueue.Count == 0)
            return;

        do
        {
            BackGroundController paralaxBackGround = backGroundQueue.Dequeue();
            Destroy(paralaxBackGround.gameObject);
        } while (backGroundQueue.Count > 0);
    }

    private void RemoveBackground()
    {
        BackGroundController lastParalaxBack = backGroundQueue.Dequeue();
        Destroy(lastParalaxBack.gameObject);
    }

    public void UpdateParalax()
    {

        if (heroCamera.transform.position.y >
             _lastParalaxBackGround.transform.position.y + _lastParalaxBackGround.bounds.size.y * 0.25f)
        {
            AddBackground(backgroundGO);
            if (backGroundQueue.Count > 2)
               RemoveBackground();
        }


        paralaxPosition = getBackgroundPos(getCameraDPos(heroCamera.transform.position));
//       Debug.Log( "Paralax pos - " + paralaxPosition );
        
        transform.localPosition = getParalaxPosition(paralaxPosition, prevParalaxPosition);
        prevParalaxPosition = paralaxPosition;

    }

    private Vector3 getParalaxPosition(Vector3 prevPos, Vector3 currentPos) {
        
        float smoothKief = 800.0f;
        Vector2 dPos = new Vector3(currentPos.x - prevPos.x, currentPos.y - prevPos.y);

//        Debug.Log("DPOS: X - " + dPos.x / smoothKief + "  Y - " +  dPos.y / smoothKief);

        return new Vector3(prevPos.x + dPos.x / smoothKief, prevPos.y + dPos.y / smoothKief, currentPos.z);
        
    }

     private Vector3 getBackgroundPos( Vector2 dPosition) {
//        Debug.Log( "DPOS - " + dPosition );
           Vector2 backDpos = new Vector2(dPosition.x * moveKoef * 0.5f , dPosition.y * moveKoef );
           return new Vector3(backgroundStartPosition.x + backDpos.x, backgroundStartPosition.y + backDpos.y , backgroundStartPosition.z);
       }



     Vector2 getCameraDPos(Vector2 cameraPosition)
     {
         return new Vector2(cameraPosition.x - heroCameraStartPosition.x, cameraPosition.y - heroCameraStartPosition.y);
     }
}
