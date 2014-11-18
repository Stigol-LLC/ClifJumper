using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuController : MonoBehaviour {

    public GameObject coinsGO;
    private Text coinsValue;

    void Awake() {
        coinsValue = coinsGO.GetComponent<Text>();
    }

    public void HideMenu() {
        gameObject.SetActive( false);
    }

    public void ShowMenu() {
        coinsValue.text = GMDataMngr.GameProgress.totalCoinsCollected.ToString();
        gameObject.SetActive( true );
    }

    public void BackButtonPressed() {
        HideMenu();
        GameManager.sceneController.hero.mouseTouched();
        GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost1Pressed() {
        //Debug.Log( "b1" );
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave( 100 );
        GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost2Pressed()
    {
       // Debug.Log("b2");
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave(200);
        GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost3Pressed()
    {
      //  Debug.Log("b3");
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave(1000);
        GameManager.sceneController.gameController.ShowMenu();
    }

    public void boost4Pressed()
    {
        //Debug.Log("b4");
        HideMenu();
        GameManager.sceneController.hero.jumpFromCave(3000);
        GameManager.sceneController.gameController.ShowMenu();
    }
}
