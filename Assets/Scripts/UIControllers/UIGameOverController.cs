using UnityEngine;
using UnityEngine.UI;

public class UIGameOverController : MonoBehaviour {

    public GameObject scoreGO;
    public GameObject bestScoreGO;

    public void ShowMenu() {
        gameObject.SetActive( true );
       

        int bestScore = GMDataMngr.GameProgress.bestHeight;
        int currentScore = GMDataMngr.GameProgress.currentHeight;

        


        if ( currentScore> bestScore )
            bestScore = currentScore;

        scoreGO.GetComponent<Text>().text = currentScore.ToString();
        bestScoreGO.GetComponent<Text>().text = bestScore.ToString();

        GMDataMngr.GameProgress.currentHeight = 0;

        GMDataMngr.SyncTool.SyncGameProgress();

        animation.Play("ShowMenu");
    }

    public void HideMenu() {
       // gameObject.SetActive( false );
        animation.Play("HideMenu");

    }

    public void onButtonRestartPressed() {
        HideMenu();
        GameManager.sceneController.RestartGame();


    }

    public void onButtonSavePressed() {
        HideMenu();
        GameManager.sceneController.SaveHero();
    }

    public void onButtonLeaderboardsPressed() {
        Debug.Log( "scores" );
    }

    public void onButtonSharePressed() {
        Debug.Log( "share" );
    }

    public void onButtonRatingPressed() {
        Debug.Log( "rating" );
    }
}
