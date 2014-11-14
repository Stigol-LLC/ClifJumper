﻿using UnityEngine;
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

    private Vector3 grabMarkPos;
    private float weightDY;
    private float dWeightDY;

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

    public void CatchHero() {
       

        Vector3 heroPos = getCatchHeroPosition();

        gameObject.transform.position = new Vector3(heroPos.x - 800, heroPos.y + 400, gameObject.transform.position.z);
        gameObject.SetActive( true );
      
        SetBezier( grabMark.transform.position, heroPos );

        currentCondorState = condorState.flyingToHero;
        weightDY = 0;
       // isFlyingToHero = true;

        isLegsReady = true;
    }

    void SetBezier(Vector3 startPosition, Vector3 endPosition) {
        Vector3 cPoint1 = new Vector3(startPosition.x - 100, startPosition.y - 100, startPosition.z);
        Vector3 cPoint2 = new Vector3(endPosition.x - 100, endPosition.y - 100, endPosition.z);

        catchHeroBezier = new Bezier(startPosition, endPosition, cPoint1, cPoint2);
        currentBezierPercentage = 0;
    }

    public Vector3 getCatchHeroPosition() {
        return hero.condorCatchGO.transform.position;
    }

    public void Update() {

        if (currentCondorState == condorState.waiting )
            return;


        switch ( currentCondorState ) {
                case condorState.flyingToHero:
                if ((currentBezierPercentage > 0.5f) && (isLegsReady))
                {
                    GetLegsReady();
                    currentCondorState = condorState.catchingHero;
                }

             
                break;

                case condorState.catchingHero:
                if (currentBezierPercentage > 1.0f)
                {
                    currentBezierPercentage = 1.0f;
                    PutHeroUp();
                    currentCondorState = condorState.flyingToRock;
                    
                }
               // gameObject.transform.position = getCondorPositionFromGrabPoint(catchHeroBezier.GetBezierPointAtTime(currentBezierPercentage));
                break;

                case condorState.flyingToRock:
                if (currentBezierPercentage > 1.0f)
                {
                    currentBezierPercentage = 1.0f;
                    ReleaseHero();
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

                }
                break;

        }

        

        gameObject.transform.position = getCondorPositionFromGrabPoint(catchHeroBezier.GetBezierPointAtTime(currentBezierPercentage));
        currentBezierPercentage += Time.deltaTime;
        
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
       // condorBodyGO.animation.Play("CondorRelease");
    }

    void GetLegsReady() {
        grabCondorAnimator.Play( "Body_Grab" );
    }

    void PutHeroUp() {
        Vector3 heroPos = getCatchHeroPosition();
        Vector3 endPos = new Vector3(heroPos.x + 200, heroPos.y + 500, heroPos.z);

        SetBezier( heroPos,  endPos);

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

        SetBezier(heroPos, endPos);
    }
}