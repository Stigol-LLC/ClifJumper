using UnityEngine;
using System.Collections;

public class EtiController : MonoBehaviour {

    public GameObject boot1;
    public GameObject boot2;



    private Animator etiAnimator;
    private GameObject activeBoot;

    void Awake() {
        etiAnimator = GetComponent<Animator>();
    }

    public void KickHero(int bootN) {


        activeBoot = null;
        switch ( bootN ) {
            case 0:
                break;
                
            case 1:
                activeBoot = boot1;
                break;

            case 2:
                activeBoot = boot2;
                break;

            case 3:
                break;
        }
        Invoke( "playBootAnimation", 0.2f );
        etiAnimator.Play("Eti_Kick");
   
        audio.Play();

       // Debug.Log( "AB - " + bootN );
    }


    void playBootAnimation() {
        
        if ( activeBoot != null )
            activeBoot.animation.Play();

        
    }


}
