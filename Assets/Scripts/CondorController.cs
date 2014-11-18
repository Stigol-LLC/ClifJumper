using UnityEngine;
using System.Collections;

public class CondorController : MonoBehaviour {

    public GameObject grabMark;

    public GameObject grabCondorPartGO ;
    public GameObject flyCondorPartGO;
    public GameObject condorBodyGO;

    private Animator grabCondorAnimator;
    private Animator flyCondorAnimator;

    private Bezier catchHeroBezier;
    private HeroController hero;

    private float currentBezierPercentage;
   
    private bool isLegsReady;
    private bool isReleasingHero;

    private Vector3 grabMarkPos;
    private float weightDY;
    private float dWeightDY;

    private Vector3 popUpPosition;

    private Vector3 startCondorPosition;
    private Vector3 condorMid1Pos;
    private Vector3 condorMid2Pos;

    enum condorState {
        waiting,
        flyingToHero,
        catchingHero,
        flyingToRock,
        releasingHero,
        flyingAway
    }

    private condorState currentCondorState;

    void Awake() {
        grabCondorAnimator = grabCondorPartGO.GetComponent<Animator>();
        flyCondorAnimator = flyCondorPartGO.GetComponent<Animator>();
        
        
    }

    void Start() {
        hero = GameManager.sceneController.hero;
        gameObject.SetActive( false );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(grabMark.transform.position, 10);
    }

    public void CatchHero(Vector3 crackPos) {
       

        Vector3 heroPos = getCatchHeroPosition();

        gameObject.transform.position = new Vector3(heroPos.x - 800, heroPos.y - 800, gameObject.transform.position.z);
        gameObject.SetActive( true );

        currentBezierPercentage = 0;
        startCondorPosition = grabMark.transform.position;
        condorMid1Pos = new Vector3(startCondorPosition.x - 100, startCondorPosition.y - 100, startCondorPosition.z);
        condorMid2Pos = new Vector3(heroPos.x - 100, heroPos.y - 100, heroPos.z);


        SetBezier( startCondorPosition,condorMid1Pos, condorMid2Pos, heroPos );

        currentCondorState = condorState.flyingToHero;
        weightDY = 0;
       // isFlyingToHero = true;

        isLegsReady = true;
        isReleasingHero = false;

        popUpPosition = crackPos;
    }

    void SetBezier(Vector3 startPosition, Vector3 mid1Pos, Vector3 mid2Pos, Vector3 endPosition) {
       // Vector3 cPoint1 = new Vector3(startPosition.x - 100, startPosition.y - 100, startPosition.z);
       // Vector3 cPoint2 = new Vector3(endPosition.x - 100, endPosition.y - 100, endPosition.z);

        catchHeroBezier = new Bezier(startPosition, endPosition, mid1Pos, mid2Pos);
        
    }

    public Vector3 getCatchHeroPosition() {
        return hero.condorCatchGO.transform.position;
    }

    public void Update() {

        if (currentCondorState == condorState.waiting )
            return;

//        Debug.Log( "POs - " + transform.position );

        Vector3 heroPos;
        switch ( currentCondorState ) {
                case condorState.flyingToHero:
                heroPos = getCatchHeroPosition();
              //      SetBezier(startCondorPosition, condorMid1Pos, condorMid2Pos, heroPos);
                SetBezier(startCondorPosition, startCondorPosition, heroPos, heroPos);

                if ((currentBezierPercentage > 0.5f) && (isLegsReady))
                {
                    
                   // Debug.Log( "Hero pos - " + heroPos );

                    GetLegsReady();
                    currentCondorState = condorState.catchingHero;
                }

             
                break;

                case condorState.catchingHero:

                heroPos = getCatchHeroPosition();
                 //   SetBezier(startCondorPosition, condorMid1Pos, condorMid2Pos, heroPos);
                SetBezier(startCondorPosition, startCondorPosition, heroPos, heroPos);



                if (currentBezierPercentage > 1.0f)
                {

                    

                    currentBezierPercentage = 1.0f;
                    PutHeroUp(popUpPosition);
                    currentCondorState = condorState.flyingToRock;
                    
                }
               // gameObject.transform.position = getCondorPositionFromGrabPoint(catchHeroBezier.GetBezierPointAtTime(currentBezierPercentage));
                break;

                case condorState.flyingToRock:
                if ( (currentBezierPercentage > 0.9f) && (!isReleasingHero) ) {
                    ReleaseHero();
                }


                if (currentBezierPercentage > 1.0f)
                {
                    currentBezierPercentage = 1.0f;
                    
                    currentCondorState = condorState.releasingHero;
                    
                }
              //  gameObject.transform.position = getCondorPositionFromGrabPoint(catchHeroBezier.GetBezierPointAtTime(currentBezierPercentage));
                break;

                case condorState.releasingHero:
                FlyAway();
                currentCondorState = condorState.flyingAway;
                return;
                break;

                  case condorState.flyingAway:
                if (currentBezierPercentage > 1.0f)
                {
                    currentBezierPercentage = 1.0f;  
                    currentCondorState = condorState.waiting;
                    gameObject.SetActive( false );
                }
                break;

        }

        

        gameObject.transform.position = getCondorPositionFromGrabPoint(catchHeroBezier.GetBezierPointAtTime(currentBezierPercentage));

        float dT = Time.deltaTime;

        if ( currentCondorState == condorState.flyingToRock )
            dT /= 2;

        currentBezierPercentage +=  dT;
        
    }

    Vector3 getCondorPositionFromGrabPoint(Vector3 grabPosition) {
        Vector3 condorPosition = transform.position;
        Vector3 grabMarkPosition = grabMark.transform.position;

        Vector2 dPosition = new Vector2(transform.position.x - grabMarkPosition.x, transform.position.y - grabMarkPosition.y);

        weightDY += dWeightDY;

//        if ( Mathf.Abs( weightDY ) < dWeightDY * 1.2f )
//            weightDY = 0;

        return new Vector3(grabPosition.x + dPosition.x, grabPosition.y + dPosition.y , condorPosition.z);
    }

    void ReleaseHero() {
        GameManager.sceneController.hero.condorRelease();
        grabCondorAnimator.Play("Body_Release");
        isReleasingHero = true;
        // condorBodyGO.animation.Play("CondorRelease");
    }

    void GetLegsReady() {
        isLegsReady = false;
        grabCondorAnimator.Play( "Body_Grab" );
    }

    void PutHeroUp(Vector3 endPosition) {
        Vector3 heroPos = getCatchHeroPosition();
       // Vector3 endPos = new Vector3(heroPos.x + 200, heroPos.y + 500, heroPos.z);
        currentBezierPercentage = 0;

     //   ve

        condorMid1Pos = new Vector3(grabMark.transform.position.x - 100, grabMark.transform.position.y - 100, grabMark.transform.position.z);
        condorMid2Pos = new Vector3(endPosition.x - 100, endPosition.y - 100, heroPos.z);


        SetBezier( heroPos, condorMid1Pos, condorMid2Pos, endPosition);

        GameManager.sceneController.hero.condorCatch();
        grabMarkPos = grabMark.transform.position;
        flyCondorAnimator.Play( "Wings_Fly" );
        weightDY = -50;
        dWeightDY = 0.1f;
        //condorBodyGO.animation.Play( "CondorGrab" );
    }

    void FlyAway() {
        Vector3 heroPos = getCatchHeroPosition();
        Vector3 endPos = new Vector3(heroPos.x + 800, heroPos.y + 300, heroPos.z);
        currentBezierPercentage = 0;
        condorMid1Pos = new Vector3(heroPos.x - 100, heroPos.y - 100, heroPos.z);
        condorMid2Pos = new Vector3(endPos.x - 100, endPos.y - 100, heroPos.z);


        SetBezier(heroPos, condorMid1Pos, condorMid2Pos, endPos);
    }
}
