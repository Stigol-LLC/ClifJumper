using UnityEngine;
using System.Collections;

public class UICaveDialogController : MonoBehaviour {
    public GameObject cloudGO ;
    private Animator cloudAnimator;

    void Awake() {
        cloudAnimator = cloudGO.GetComponent<Animator>();
    }

  
    public void showDialog() {
        cloudAnimator.speed = 1;
        cloudAnimator.Play( "Comics_Cloud_PopUp" );
    }

    public void hideDialog() {
        cloudAnimator.speed = -1;
        cloudAnimator.Play("Comics_Cloud_PopUp");
    }
}
