using UnityEngine;
using System.Collections;

public class CaveController : MonoBehaviour {
    public Vector3 setPoint;
    public Vector3 heroPoint;

    private HeroCamera heroCamera;
    public GameObject etiGO;

    private CaveController cave;
    private Animator etiAnimator;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x + setPoint.x, transform.position.y + setPoint.y, transform.position.z), 10);
    
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(getHeroStayPoint(), 10);
    }

    void Awake() {
        etiAnimator = etiGO.GetComponent<Animator>();
        heroCamera = GameManager.HeroCamera;
        
    }

    public void etiKickHero()
    {
        etiAnimator.Play("Eti_Kick");
    }

    public Vector3 StartCave() {
        setPosition(heroCamera.getCameraBounds().min);
        return getHeroStayPoint();
    }

    public Vector3 getHeroStayPoint() {
        return new Vector3( transform.position.x + heroPoint.x, transform.position.y + heroPoint.y, transform.position.z );
    }

    void setPosition(Vector3 anchorPosition) {
        transform.position = new Vector3(anchorPosition.x - setPoint.x, anchorPosition.y - setPoint.y, anchorPosition.z);
    }

    public void hideCave() {
//        etiAnimator.Play( "Idle_Sit" );
        gameObject.SetActive( false );
    }

    public void showCave() {
        gameObject.SetActive( true );
       
    }

  

}
