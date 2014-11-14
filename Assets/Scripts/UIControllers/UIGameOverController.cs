using UnityEngine;
using System.Collections;

public class UIGameOverController : MonoBehaviour {

    public void ShowMenu() {
        gameObject.SetActive( true );
        GMDataMngr.SyncTool.SyncGameProgress();
    }

    public void HideMenu() {
        gameObject.SetActive( false );
    }

    public void onButtonRestartPressed() {
        HideMenu();
        GameManager.sceneController.RestartGame();


    }
}
