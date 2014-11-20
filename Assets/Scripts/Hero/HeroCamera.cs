using UnityEngine;
using System.Collections;

public class HeroCamera : MonoBehaviour {

    
    private Bounds RockScreenBounds;
    public GameObject FollowGameObject;

    private bool isFollowing = false;
    private bool isCondorFollowing = false;
	// Use this for initialization

    private bool deathFollow;
    private bool jumpFromCaveFollow;
    
    private Vector3 realCameraShift;
    private Vector3 newCameraShift;

    private Camera heroCamera;

    private float cameraWidth;
    private float cameraHeight;

    public GameObject paralaxGO;
    private ParalaxManager paralaxManager;

    private Vector3 cameraStartPosition;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(getRockBounds().center, getRockBounds().size);
    }


    void Awake() {
      
        heroCamera = GetComponent<Camera>();

        cameraHeight = heroCamera.orthographicSize * 2;
        float screenAspect = (float)Screen.width / (float)Screen.height;
        cameraWidth = cameraHeight * screenAspect;

        Vector3 rockScreenBoundsCenter = new Vector3(heroCamera.transform.position.x, heroCamera.transform.position.y + cameraHeight * 8 , heroCamera.transform.position.z);
        RockScreenBounds = new Bounds(rockScreenBoundsCenter, new Vector3(cameraWidth , cameraHeight * 25 , 0));
        deathFollow = false;

        paralaxManager = paralaxGO.GetComponent<ParalaxManager>();
    }

    void Start() {
       restartPos();
    }

    public Bounds getRockBounds() {

        Vector3 newCenter = new Vector3(heroCamera.transform.position.x + RockScreenBounds.center.x, heroCamera.transform.position.y + RockScreenBounds.center.y, heroCamera.transform.position.z);
        Bounds bounds = new Bounds(newCenter, RockScreenBounds.size);
        return bounds;
    }

    public Bounds getCameraBounds() {
        
        Bounds bounds = new Bounds(heroCamera.transform.position, new Vector3(cameraWidth, cameraHeight, 0));
        return bounds;
    }

    public void Restart() {
        deathFollow = false;
        isFollowing = false;
        isCondorFollowing = false;
        restartPos();
        StartCamera();
    }

    void restartPos() {
        heroCamera.transform.position = new Vector3(-cameraWidth / 3, 0, heroCamera.transform.position.z);
        heroCamera.transform.localPosition = new Vector3(-cameraWidth / 3, 0, heroCamera.transform.localPosition.z);
        
    }
   

    public void StartCamera() {

        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
        realCameraShift = new Vector3(
                0,
     
              getCameraBounds().size.y / 2 - heroSizes.y ,
                heroCamera.transform.position.z );
        
    }

 
    public void Follow() {
//        Debug.Log( "FOLLOW CAMERA" );
        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
        

     //   cameraShift = new Vector3(
      //          0,
     //           getCameraBounds().size.y / 2 - heroSizes.y,
      //          heroCamera.transform.position.z);
        isFollowing = true;
    }

    public void FollowCondor() {
      
        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
        newCameraShift = new Vector3(
                0,
                getCameraBounds().size.y / 2 - heroSizes.y,
                heroCamera.transform.position.z);
        isCondorFollowing = true;
    }

    public void FollowDeath()
    {
        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
        newCameraShift = new Vector3(0,-heroSizes.y,0);
        deathFollow = true;
    }

    public void FollowJumpFromCave()
    {
        Debug.Log( "FJC" );
//        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
//        cameraShift = new Vector3(
//        0,
//        getCameraBounds().size.y / 2 - heroSizes.y,
//        heroCamera.transform.position.z);

//        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
        newCameraShift = new Vector3(
        0,
        0,
        heroCamera.transform.position.z);


        jumpFromCaveFollow = true;

      
    }

    public void setCatchShift() {
        Vector3 heroSizes = GameManager.sceneController.hero.getBounds().size;
        newCameraShift = new Vector3(
                0,
                getCameraBounds().size.y / 2 - heroSizes.y,
                heroCamera.transform.position.z);
    }

    public void StopFollowing() {
        deathFollow = false;
        isFollowing = false;
        isCondorFollowing = false;
        jumpFromCaveFollow = false;

     

    }

    void Update() {
 
      //  Debug.Log( "Following - " + isFollowing + "  Death follow - " + deathFollow );
        
        if ( !( ( isFollowing ) || ( deathFollow ) || ( isCondorFollowing ) || ( jumpFromCaveFollow ) ) ) {
            paralaxManager.ParalaxUpdate();
            return;
        }




        Vector3 cameraPosition = heroCamera.transform.position;

        float dX = FollowGameObject.transform.position.x - cameraPosition.x + realCameraShift.x;
        float dY = FollowGameObject.transform.position.y - cameraPosition.y + realCameraShift.y;

       Vector2 dCameraShift = new Vector2(newCameraShift.x - realCameraShift.x, newCameraShift.y - realCameraShift.y);

        realCameraShift = new Vector3(realCameraShift.x + dCameraShift.x / 8, realCameraShift.y + dCameraShift.y / 8);

        if ( isCondorFollowing ) {
            //dX = FollowGameObject.transform.position.x;
        }

       // float dX = 0;
        

        if ( ( Mathf.Abs( dX ) < 2 ) &&
             ( Mathf.Abs( dY ) < 2 ) )
            isFollowing = false;


        float smoothKoef = 16;

        if ( deathFollow == true )
            smoothKoef = 4;

        if (jumpFromCaveFollow)
            smoothKoef = 6;

        if ( isCondorFollowing )
            smoothKoef = 16;

       // Debug.Log( "Smooth koef - " + smoothKoef );

        //Debug.Log( "JF - " + jumpFromCaveFollow + "  DF - " + deathFollow + "CondorF" + isCondorFollowing );

        float smoothDX = dX / smoothKoef;
        float smoothDY = dY / smoothKoef;



//        if ( Mathf.Abs(smoothDX) < 2 )
//            smoothDX = 1 * Mathf.Sign( smoothDY );
//
//        if ( Mathf.Abs(smoothDY) < 2 )
//            smoothDY = 1 * Mathf.Sign( smoothDY );
        
        heroCamera.transform.position = new Vector3(cameraPosition.x + smoothDX, cameraPosition.y + smoothDY, cameraPosition.z);
//   Debug.Log( "Camera pos - " + heroCamera.transform.position + "  Follow pos - " + FollowGameObject.transform.position );
//       Debug.Log( "Pos - " + heroCamera.transform.position + "sDX - " + smoothDX + "sDY - " + smoothDY);
        paralaxManager.ParalaxUpdate();
        
    
    }

    
	
}
