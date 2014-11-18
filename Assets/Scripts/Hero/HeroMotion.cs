using UnityEngine;
using System.Collections;

public class HeroMotion : MonoBehaviour {

    
    private Animator motionAnimator;

   // public bool isStriking;
    private bool isGameOver;





    void Awake() {
        motionAnimator = GetComponent<Animator>();
        isGameOver = false;
    }

    public void ResetHero() {
        isGameOver = false;
        motionAnimator.Play( "Idle_Start" );
    }

    public void Jump() {
        //isStriking = false;
        motionAnimator.Play( "Jump" );
    }

    public void KickedFromCave()
    {
        //isStriking = false;
        motionAnimator.Play("GetKicked");
    }

    public void Strike() {
      //  isStriking = true;
        motionAnimator.SetBool( "isCatched", false );
        motionAnimator.Play( "Strike" );
    }

    public void StrikeFromJumpCave() {
        motionAnimator.Play( "StrikeFromJump" );
    }


    public void Catch() {
        motionAnimator.SetBool("isCatched", true);
        
    }

    public void Fix() {
      //  isStriking = false;
        motionAnimator.Play("Success");
    }

    public void GameOver() {
        isGameOver = true;
        motionAnimator.Play("Fail");
    }

    public void FailFall() {
        //motionAnimator.Play( "FailFall" );
    }

    public void SaveFall() {
        motionAnimator.Play( "FailFall" );
    }

    public void MissedRock() {
        if (isGameOver)
            return;

       // isStriking = false;
   //    Debug.Log( "MISS!!!" );
       GameManager.sceneController.hero.Miss();
    }

    public void JumpToCheckpoint() {
        motionAnimator.Play( "Checkpoint_Jump" );
    }

}
