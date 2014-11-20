using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuController : MonoBehaviour {

    public GameObject coinsGO;
    private Text coinsValue;

    void Awake() {
        coinsValue = coinsGO.GetComponent<Text>();
    }

    public void HideMenu() {
        //gameObject.SetActive( false);

        animation.Play( "HideMainMenu" );
    }

    public void ShowMenu() {
        coinsValue.text = GMDataMngr.GameProgress.totalCoinsCollected.ToString();
        gameObject.SetActive( true );

        animation.Play("ShowMainMenu");
    }

    public void BackButtonPressed() {
        HideMenu();
        GameManager.sceneController.hero.mouseTouched();
       // GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost1Pressed() {
        //Debug.Log( "b1" );
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave( 50 , 100, 0, 1, 0);
     //   GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost2Pressed()
    {
       // Debug.Log("b2");
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave(50, 200, 1, 0.8f, 1);
     //   GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost3Pressed()
    {
      //  Debug.Log("b3");
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave(50, 1000, 2, 0.6f, 2);
      //  GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost4Pressed()
    {
        //Debug.Log("b4");
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave(50, 3000, 2, 0.4f, 3);
        //GameManager.sceneController.gameController.ShowMenu();
    }
}
