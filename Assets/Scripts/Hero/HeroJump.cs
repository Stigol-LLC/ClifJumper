using UnityEngine;
using System.Collections;

public class HeroJump : MonoBehaviour {

    private Animator heroJumpAnimator;
    public bool isJumping;
    public bool isFalling;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake() {
        startPosition = gameObject.transform.localPosition;
        heroJumpAnimator = gameObject.GetComponent<Animator>();
        isJumping = false;

        startRotation = gameObject.transform.localRotation;

    }


    public void Restart() {
        GameManager.HeroCamera.FollowGameObject = GameManager.sceneController.hero.gameObject;
        isFalling = false;
        isJumping = false;
       
        gameObject.transform.localPosition = startPosition;
        gameObject.transform.localRotation = startRotation;
        heroJumpAnimator.Play("HeroController_Idle");
    }

    public void Jump()
    {
      //  if (isJumping == false)
      //  {
            heroJumpAnimator.SetBool("isCatched", false);
            heroJumpAnimator.Play("HeroController_Jump");
            isJumping = true;
            isFalling = false;
       // }
    }

    public void FlyFromCave() {
        heroJumpAnimator.SetBool("isCatched", false);
        heroJumpAnimator.Play("HeroController_JumpFromCave");
       
        isFalling = false;
    }

    public void JumpToCheckpoint() {
        heroJumpAnimator.Play( "HeroController_JumpToCheckPoint" );
    }

    public void JumpToCheckpointFinished() {
        GameManager.sceneController.hero.JumpFromCaveFinished();
    }

    public void CatchRock() {
        isJumping = false;
        heroJumpAnimator.SetBool("isCatched", true);
        heroJumpAnimator.Play("HeroController_Idle");
        
    }

    public void StandOnRock() {
        heroJumpAnimator.Play("HeroController_Idle");
        isJumping = false;
    }

    public void MissRock()
    {
        heroJumpAnimator.SetBool("isCatched", false);
    }

    public void RockIdling() {
      
    }

    public void GameOver() {
        GameManager.sceneController.EndGame();
        GameManager.sceneController.hero.GameOver();
        
    }

    public void GameOverStarted() {

        GameManager.HeroCamera.FollowGameObject = gameObject;
        GameManager.HeroCamera.FollowDeath();
        GameManager.sceneController.hero.GameOverStarted();
      
    }

    public void JumpFromCaveStarted() {
        GameManager.sceneController.hero.FlyFromCaveStarted();
    }

    public void StartFalling() {
       // Debug.Log( "MISS" );
        isFalling = true;
        
    }
}
