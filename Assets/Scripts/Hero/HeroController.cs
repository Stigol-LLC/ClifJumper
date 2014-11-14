using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class HeroController : MonoBehaviour {

    public AudioClip startJump;
    public AudioClip catchRock;
    public AudioClip scream;
    public AudioClip beginStrike;
    public AudioClip kickedFromEti;

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
    private bool isJumpFromCave = true;

    private bool isFixing;

    private Vector2 fixVector;
    private Vector2 rockFixPoint;


    private bool canStrike;
    private int coinsCollected;
    private bool isTouchesEnabled;

    private bool isCondorCatched;
    

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
	    
	    isFixing = false;
       // transform.position = new Vector3(0, 0, transform.position.z);
       // transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        startHeight = transform.position.y;
	    coinsCollected = 0;

	    isTouchesEnabled = true;
	}

    public void condorCatch() {
        isCondorCatched = true;
    }

    public void condorRelease() {
        isCondorCatched = false;
        CrackController crack =  GameManager.sceneController.levelRocksController.getClothestCrack( getFixPoint().y );
        Catch( crack, 0.3f );
    }

    public void StartHero() {
        
    }

    public void RestartHero() {
        isCondorCatched = false;
        transform.position = new Vector3(0, 0, transform.position.z);
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        startHeight = transform.position.y;
        isFixing = false;
        heroJumpController.Restart();
        canStrike = true;

        StartHero();
        axeController.SetCollider( true );

        heroMotionController.ResetHero();
        axeController.SetCanCatch(true);
        isJumpFromCave = true;
        isTouchesEnabled = true;

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

        if ( isJumpFromCave ) {
            jumpFromCave();
         //   GameManager.sceneController.condorController.CatchHero();
            isTouchesEnabled = false;
        } else if ( !heroJumpController.isJumping ) {
            jumpFromCrack();
        } else {
            if (canStrike)
             strike();
        }


    }

    void jumpFromCave() {
        Debug.Log( "HC Jump from cave" );

        isFixing = false;
        GameManager.sceneController.levelRocksController.getCave().etiKickHero();
        heroMotionController.KickedFromCave();
        heroJumpController.FlyFromCave();
        
        isJumpFromCave = false;
    }

    public void JumpFromCaveFinished() {
       // Debug.Log( "FINISHED!!!!" );
        heroMotionController.StrikeFromJumpCave();
        CrackController crack =  GameManager.sceneController.levelRocksController.getClothestCrack( getFixPoint().y );
        Catch( crack, 0.3f );
        
    }

    void jumpFromCrack() {

      //  Debug.Log("HC Jump from crack");

        isFixing = false;
        canStrike = true;
        heroMotionController.Jump();
        heroJumpController.Jump();

        //startEmmitAssParticles();

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

        crack.gameObject.GetComponent<Collider2D>().enabled = false;

//        if ( rock.hasCoin ) {
//            GMDataMngr.GameProgress.totalCoinsCollected ++;
//            GameManager.sceneController.gameController.setCoinsValue( coinsCollected );
//            rock.collectCoin();
//        }

        audio.clip = catchRock;
        audio.Play();

        crack.heroCatch();

        heroMotionController.Catch();

    }

    public int getHeight( Vector3 objectPosition ) {
        return (int) ( objectPosition.y - startHeight ) / 100;
    }

    void fixRock() {
        isFixing = true;
        canStrike = true;
        heroMotionController.Fix();
        isTouchesEnabled = true;
    }

    void folowCamera() {
       Debug.Log( "Camera FOLOW" );
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

    void setCondorPosition( Vector3 condorMarkPosition ) {
        
        Vector3 grabMark = condorCatchGO.transform.position;
        Vector2 dPos = new Vector3(transform.position.x - grabMark.x, transform.position.y - grabMark.y);

        transform.position = new Vector3(condorMarkPosition.x + dPos.x, condorMarkPosition.y + dPos.y, transform.position.z);
       
    }

    void Update() {

        if ( isCondorCatched ) {
  

            setCondorPosition( GameManager.sceneController.getCatchHeroPos() );
            return;
        }
        

        if (!isFixing)
            return;

        

        Vector2 axeFixPoint = axeController.getFixedPointPosition();
        fixVector = new Vector2(rockFixPoint.x - axeFixPoint.x, rockFixPoint.y - axeFixPoint.y);

        if ( ( Mathf.Abs( fixVector.x ) < 1 ) &&
             ( Mathf.Abs( fixVector.y ) < 1 ) ) {
            isFixing = false;
            return;
        }

        transform.position = new Vector3(transform.position.x + fixVector.x / 3, transform.position.y + fixVector.y / 3, transform.position.z);
    }

    public void FlyFromCaveStarted() {
        audio.clip = kickedFromEti;
        audio.Play();
    }

}
