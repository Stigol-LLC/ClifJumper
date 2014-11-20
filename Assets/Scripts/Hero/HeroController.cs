using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class HeroController : MonoBehaviour {

    public AudioClip finishKickedJump;
    public AudioClip catchRock;
    public AudioClip scream;
    public AudioClip beginStrike;
    public AudioClip kickedFromEti;
    public AudioClip kickedFromEtiScream;
    public AudioClip startJump;

    public Bounds heroBounds;
    public Vector2 coinDropPoint;
    public GameObject heroJumpGO;
    public GameObject heroActionGO;
    public GameObject axeGO;
    public GameObject assAccelerateParticles;

    public GameObject condorCatchGO;

    // Use this for initialization

    private HeroJump heroJumpController;
    private HeroMotion heroMotionController;
    private AxeController axeController;
    private ParticleEmitter assAccParticleEmitter;

    private Vector3 startJumpHeroPos;

    private float startHeight;
    //   private Animator heroJumpAnimator;
    //  private bool isJumpFromCave = true;

    // private bool isFixing;

    private Vector2 fixVector;
    private Vector2 rockFixPoint;

    public CrackController fallCrack;

    private bool canStrike;
    private int coinsCollected;
    private bool isTouchesEnabled;
    private int heroJumpHeight;

    private int speedAnimationNumber;
//  private bool showSpeed;
  //  private bool isCondorCatched;
    private Bezier jumpBezier;
    private float jumpCaveSpeedBoost;
    private float moveBezierPercentage;
    private CrackController jumpCrack;
    private float jumpFromCaveHeight;
    enum heroState {
        waitingForKick,
        jumpingFromCave,
        jumpingFromCrack,
        normal,
        fixing,
        condorCatched,
        
    }

    private heroState currentHeroState;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(getBounds().center, heroBounds.size);


        Gizmos.DrawSphere(
       new Vector3(
               transform.position.x + coinDropPoint.x,
               transform.position.y + coinDropPoint.y,
               transform.position.z),
       5);


        Gizmos.color = Color.green;
        Gizmos.DrawSphere( getHeroStayPoint(), 5);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(condorCatchGO.transform.position, 5);
    }

    public void PutHeroOnStayPoint(Vector3 stayPoint) {
        Vector3 stayPointPos = getHeroStayPoint();
        Vector2 dPos = new Vector2(transform.position.x - stayPointPos.x, transform.position.y - stayPointPos.y);
        transform.position = stayPoint;
        transform.position = new Vector3(transform.position.x + dPos.x, transform.position.y + dPos.y, transform.position.z);
    }

    Vector3 getHeroStayPoint() {
        Bounds hBounds = getBounds();
        return new Vector3( hBounds.center.x,  hBounds.center.y - hBounds.extents.y, hBounds.center.z);
    }



    public Vector2 getCoinDropPointPosition()
    {
        return new Vector2(transform.position.x + coinDropPoint.x, transform.position.y + coinDropPoint.y);
    }

    public Bounds getBounds() {
        Vector2 newCenter = new Vector2(transform.position.x + heroBounds.center.x, transform.position.y + heroBounds.center.y);
        return new Bounds(newCenter, heroBounds.size);
    }

	void Awake () {
	    heroJumpController = heroJumpGO.GetComponent<HeroJump>();
	    heroMotionController = heroActionGO.GetComponent<HeroMotion>();
	    axeController = axeGO.GetComponent<AxeController>();
	    assAccParticleEmitter = assAccelerateParticles.GetComponent<ParticleEmitter>();

	    startJumpHeroPos = heroJumpGO.transform.localPosition;
	    currentHeroState = heroState.waitingForKick;
	  //  isFixing = false;
       // transform.position = new Vector3(0, 0, transform.position.z);
       // transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        startHeight = transform.position.y;
	    coinsCollected = 0;

	    isTouchesEnabled = true;
	}

    public void condorCatch() {
      //  isCondorCatched = true;
        currentHeroState = heroState.condorCatched;
        
        heroJumpController.CondorSave();
        GameManager.HeroCamera.FollowCondor();
    }

    public void condorRelease() {
       // Debug.Log( "CRRRRR" );
        GameManager.HeroCamera.StopFollowing();
       // isCondorCatched = false;
        currentHeroState = heroState.normal;
       // CrackController crack =  GameManager.sceneController.levelRocksController.getClothestCrack( getFixPoint().y );
        Catch( fallCrack, 0.5f );
        axeController.SetCanCatch(true);
       heroMotionController.Strike();

       audio.clip = catchRock;
       audio.Play();
    }

    public void StartHero() {
        
    }

    public void RestartHero() {
       // isCondorCatched = false;
        transform.position = new Vector3(0, 0, transform.position.z);
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        startHeight = transform.position.y;
       // isFixing = false;
        currentHeroState = heroState.normal;
        heroJumpController.Restart();
        canStrike = true;

        StartHero();
        axeController.SetCollider( true );

        heroMotionController.ResetHero();
        axeController.SetCanCatch(true);
      //  isJumpFromCave = true;
        isTouchesEnabled = true;

        currentHeroState = heroState.waitingForKick;

    }

    void Start() {
        GameManager.sceneController.gameController.onTouched += mouseTouched;
    }

    public void setHero( Vector2 rockFixPointPosition ) {

        Vector2 axeFixPointPosition = getFixPoint();
        Vector2 dFixPosition = new Vector2(axeFixPointPosition.x - transform.position.x, axeFixPointPosition.y - transform.position.y);

//        Debug.Log("Axe fix point" + axeFixPointPosition );
     //   Debug.Log("Delta fix point" + dFixPosition);

        gameObject.transform.position = new Vector3(rockFixPointPosition.x - dFixPosition.x, rockFixPointPosition.y - dFixPosition.y);
  
    
     //   Debug.Log( "Hero position - " + gameObject.transform.position );
    }

    public Vector2 getFixPoint() {
       return axeController.getFixedPointPosition();
    }

    public Vector2 getCoinDropPoint() {
       return getCoinDropPointPosition();
    }

    public void mouseTouched(  ) {
        if (!isTouchesEnabled)
            return;

        if ( currentHeroState == heroState.waitingForKick ) {
            jumpFromCave(5, 0 , 0, 3, 0);

            Debug.Log( "JUMP FROM CAVE" );
        } else if ( !heroJumpController.isJumping ) {
            jumpFromCrack();
        } else {
            if (canStrike)
             strike();
        }


    }

    int normalizeHeight(int height) {
        return height;
      
        if ( height < 20 )
            return height;

        if ( height < 50 ) {
            return 20 + ( height - 20 ) / 2;
        }

        if (height < 100)
        {
            return 20 + (height - 20) / 3;
        }

        if (height < 500)
        {
            return 20 + (height - 20) / 10;
        }

         return 20 + (height - 20) / 50;
    }

    public void jumpFromCave(int realHeight, int jumpHeight, int speedAnimationN, float speedBoost, int bootN) {

        jumpCaveSpeedBoost = speedBoost;
       

        isTouchesEnabled = false;
        jumpFromCaveHeight = normalizeHeight( realHeight );
        Debug.Log( "HC Jump from cave" );

       // isFixing = false;
        GameManager.sceneController.levelRocksController.getCave().etiKickHero(bootN);
        heroMotionController.KickedFromCave();
       // heroJumpController.FlyFromCave();
        
      //  isJumpFromCave = false;
        Invoke( "startHeroJumpFromCave", 1.6f );
        Invoke("kickedSoundPlay", 1.4f);

        speedAnimationNumber = speedAnimationN;
        heroJumpHeight = jumpHeight;

    }


    void kickedSoundPlay() {
        audio.clip = kickedFromEti;
        audio.Play();
    }

    void startHeroJumpFromCave() {

        if ( speedAnimationNumber != 0 ) {
            audio.clip = kickedFromEtiScream;
            audio.Play();
        }

        GameManager.HeroCamera.FollowJumpFromCave();
        currentHeroState = heroState.jumpingFromCave;

        jumpCrack = GameManager.sceneController.levelRocksController.getClothestCrack(getObjectPosY(jumpFromCaveHeight));
        // Debug.Log( "JC POS - " + jumpCrack.transform.position );
        Vector3 startPos = axeController.getFixedPointPosition();
       Vector3 crackPos = jumpCrack.transform.position;
        Vector3 endPos = new Vector3(crackPos.x - 100, crackPos.y + 100, crackPos.z);
        Vector3 mid1Pos = new Vector3(startPos.x, startPos.y, 0);
        Vector3 mid2Pos = new Vector3(endPos.x - 100, endPos.y + 100, 0);


        jumpBezier = new Bezier(startPos, endPos, mid1Pos, mid2Pos);
        moveBezierPercentage = 0;

        GameManager.sceneController.levelRocksController.getCave().stopFire();
    }

    void makeJump() {
       
    }

    public void JumpFromCaveFinished() {
       // Debug.Log( "FINISHED!!!!" );
        heroMotionController.StrikeFromJumpCave();
        CrackController crack =  GameManager.sceneController.levelRocksController.getClothestCrack( getFixPoint().y );
        Catch( crack, 0.3f );
        GameManager.HeroCamera.StopFollowing();
    }

    void jumpFromCrack() {
        
   //     Debug.Log("HC Jump from crack");
        currentHeroState = heroState.jumpingFromCrack;
        //isFixing = false;
        canStrike = true;
        heroMotionController.Jump();
        heroJumpController.Jump();

        //startEmmitAssParticles();
//      
        audio.clip = startJump;
        audio.Play();
    }



    void strike() {
//        Debug.Log("HC Strike");
       
            canStrike = false;
            heroMotionController.Strike();

            audio.clip = beginStrike;
            audio.Play();   
    }


    private void startEmmitAssParticles()
    {
        assAccParticleEmitter.Emit();
    }

    private void stopEmmitAssParticles() {
        
    }

    public void StandOnRock() {
        SetCameraToHero();
        heroJumpController.StandOnRock();
    }

    void SetCameraToHero() {
       // heroJumpGO.GetComponent<Animator>().speed = 0;
        float dX = heroJumpGO.transform.localPosition.x - startJumpHeroPos.x;
        float dY = heroJumpGO.transform.localPosition.y - startJumpHeroPos.y;
        heroJumpGO.transform.localPosition = startJumpHeroPos;
        transform.position = new Vector3(transform.position.x + dX, transform.position.y + dY, transform.position.z);
      //  heroJumpGO.GetComponent<Animator>().speed = 1;
    }

    public void Catch(CrackController crack, float followCameraDelay) {
        fallCrack = crack;
        rockFixPoint = crack.getFixPointPosition();

     //   Debug.Log( "Fix point - " + rockFixPoint );
        SetCameraToHero();
       // GameManager.HeroCamera.Follow();

        heroJumpController.CatchRock();

    //    GameManager.HeroCamera.Follow();
        Invoke( "fixRock", 0.1f );
        Invoke("folowCamera", followCameraDelay);

        GMDataMngr.GameProgress.currentHeight = getHeight( transform.position );
        GameManager.sceneController.gameController.setHeight(GMDataMngr.GameProgress.currentHeight);

      //  crack.gameObject.GetComponent<Collider2D>().enabled = false;

//        if ( rock.hasCoin ) {
//            GMDataMngr.GameProgress.totalCoinsCollected ++;
//            GameManager.sceneController.gameController.setCoinsValue( coinsCollected );
//            rock.collectCoin();
//        }

//        audio.clip = catchRock;
//        audio.Play();

        crack.heroCatch();

        heroMotionController.Catch();

    }

    public int getHeight( Vector3 objectPosition ) {
        return (int) (( objectPosition.y - startHeight ) / 100 + 0.5f);
    }

    public void setHeroHeight(int height, Vector3 objectPosition) {
        startHeight = objectPosition.y - height * 100 - 100f;
    }

    float getObjectPosY( float height ) {
        return startHeight + height * 100;
    }

    void fixRock() {
      //  isFixing = true;
        canStrike = true;
        heroMotionController.Fix();
        isTouchesEnabled = true;
        currentHeroState = heroState.fixing;
        
    }

    void folowCamera() {
//       Debug.Log( "Camera FOLOW" );
        GameManager.HeroCamera.Follow();
    }

    public void GameOverStarted() {
     
        audio.clip = scream;
        audio.Play();

        axeController.SetCanCatch(false);
        
        
        Invoke( "startMotionFail", 0.2f );

    }

    void startMotionFail() {
        heroMotionController.FailFall();
    }

    public void GameOver()
    {
        heroMotionController.GameOver();
       
    }

    public void Miss() {
        audio.clip = catchRock;
        audio.Play();
       axeController.SetCollider( false );
       axeController.StartScratch();
    }

    public void EnterCheckPoint() {
        heroMotionController.JumpToCheckpoint();
        heroJumpController.JumpToCheckpoint();
        
    }

    public void ExitCheckPoint() {
        
    }

    public void CondorSave() {
       heroJumpController.SaveFall();
       heroMotionController.SaveFall();
        
    }

    void setCondorPosition( Vector3 condorMarkPosition ) {
        
        Vector3 grabMark = condorCatchGO.transform.position;
        Vector2 dPos = new Vector3(transform.position.x - grabMark.x, transform.position.y - grabMark.y);

        transform.position = new Vector3(condorMarkPosition.x + dPos.x, condorMarkPosition.y + dPos.y, transform.position.z);
       
    }

//    int getHeightKickValue() {
//        switch ( speedAnimationNumber ) {
//            case 0:
//                return 50;
//
//            case 1:
//                return 100;
//
//            case 2:
//                return 500;
//
//            case 3:
//                return 1000;
//
//            case 4:
//                return 3000;
//        }
//
//        return 0;
//    }

    void Update() {


//        Debug.Log( "TOUCHES ENABLED - " + isTouchesEnabled );

        if ( currentHeroState == heroState.condorCatched ) {
  

            setCondorPosition( GameManager.sceneController.getCatchHeroPos() );
            return;
        }


        if ( currentHeroState == heroState.fixing ) {

            Vector2 axeFixPoint = axeController.getFixedPointPosition();
            fixVector = new Vector2( rockFixPoint.x - axeFixPoint.x, rockFixPoint.y - axeFixPoint.y );

            if ( ( Mathf.Abs( fixVector.x ) < 1 ) &&
                 ( Mathf.Abs( fixVector.y ) < 1 ) ) {
                currentHeroState = heroState.normal;
                return;
            }

            transform.position = new Vector3(
                    transform.position.x + fixVector.x / 3,
                    transform.position.y + fixVector.y / 3,
                    transform.position.z );

            return;
        }

        if ( currentHeroState == heroState.jumpingFromCave ) {

            float speed = 0.8f;
          
            if ( moveBezierPercentage > 1 ) {
               // SetCameraToHero();
                setHeroHeight( heroJumpHeight, gameObject.transform.position );
                GameManager.HeroCamera.setCatchShift();
                currentHeroState = heroState.fixing;
                heroMotionController.Strike();
                Catch( jumpCrack, 0.5f );
                moveBezierPercentage = 1;
                JumpFromCaveFinished();
                isTouchesEnabled = true;

                GameManager.sceneController.gameController.ShowMenu();


                if ( speedAnimationNumber == 0 ) {
                    audio.clip = finishKickedJump;
                    audio.Play();
                }
            }

           // gameObject.transform.position = jumpBezier.GetBezierPointAtTime(moveBezierPercentage);

            setHeroPosition(  jumpBezier.GetBezierPointAtTime(moveBezierPercentage) );
          
            if ( moveBezierPercentage < 0.2f ) {
                

                speed = 1.5f;
              //  if (showSpeed)
                GameManager.sceneController.paralaxBackgroundController.startSpeedEffect( 0.25f, 100, speedAnimationNumber );
            }
            else if ( moveBezierPercentage < 0.6f ) {
                
                speed = 1;
            } else if  (moveBezierPercentage > 0.8f) {
                GameManager.sceneController.paralaxBackgroundController.stopSpeedEffect(0.25f, speedAnimationNumber);
            }

            moveBezierPercentage += Time.deltaTime * speed * jumpCaveSpeedBoost ;
        }

    }


    public void setHeroPosition(Vector3 fixPointPosition) {
        Vector3 axePos = axeController.getFixedPointPosition();
        Vector2 dPos = new Vector2(gameObject.transform.position.x - axePos.x, gameObject.transform.position.y - axePos.y);

        gameObject.transform.position = new Vector3(fixPointPosition.x + dPos.x, fixPointPosition.y + dPos.y, -800);
    }

    public void FlyFromCaveStarted() {
       
    }

}
