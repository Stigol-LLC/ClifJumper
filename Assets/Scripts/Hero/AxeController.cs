using UnityEngine;
using System.Collections;

public class AxeController : MonoBehaviour {
    private int prevRockID;
    public Vector2 fixPoint;
    public GameObject particlesGO;

    private ParticleEmitter scratchParticles;

    private bool isScratching;


    public void SetCanCatch( bool value) {
        gameObject.collider2D.enabled = value;
       
    }

    void Awake() {

        isScratching = false;
        scratchParticles = particlesGO.GetComponent<ParticleEmitter>();
    }

    void OnTriggerEnter2D(Collider2D other) {
//       Debug.Log(other.gameObject.GetInstanceID());

        int rockID = other.gameObject.GetInstanceID();
        if ( prevRockID != rockID ) {
            CrackController crack = other.gameObject.GetComponent<CrackController>();
         //  Debug.Log( "Rock pos - " + crack.gameObject.transform.position );

            GameManager.sceneController.hero.Catch(crack, 0);
        }

        prevRockID = rockID;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x + fixPoint.x, transform.position.y + fixPoint.y, transform.position.z), 10);
    }

    public Vector2 getFixedPointPosition()
    {
        return new Vector2(gameObject.transform.position.x + fixPoint.x, gameObject.transform.position.y + fixPoint.y);
    }

    public void SetCollider(bool state) {
        gameObject.collider2D.enabled = state;
    }

    public void StartScratch() {
        if (!isScratching  )
        {
            scratchParticles.emit = true;
            isScratching = true;
            Invoke( "FinishScratch", 0.5f );
        }
    }

    public void FinishScratch() {
        scratchParticles.emit = false;
        isScratching = false;
    }
 
}
