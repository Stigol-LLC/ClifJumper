using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {

    public GameObject heroGO;

    [HideInInspector]
    public HeroController hero;

    public GameObject GameOverGO;

    [HideInInspector]
    public UIGameOverController gameOverController;

    public GameObject GameCanvasGO;

    [HideInInspector]
    public UIGameController gameController;

    public GameObject levelRocksControllerGO;
    [HideInInspector] public LevelRocksController levelRocksController;

    public GameObject paralaxBackgroundGO;
    [HideInInspector]
    public ParalaxManager paralaxBackgroundController;

    public GameObject condorGO;
    [HideInInspector]
    public CondorController condorController;

	void Awake () {
	    Application.targetFrameRate = 60;
	    hero = heroGO.GetComponent<HeroController>();
	    gameOverController = GameOverGO.GetComponent<UIGameOverController>();
	    gameController = GameCanvasGO.GetComponent<UIGameController>();
	    levelRocksController = levelRocksControllerGO.GetComponent<LevelRocksController>();
	    paralaxBackgroundController = paralaxBackgroundGO.GetComponent<ParalaxManager>();
	    condorController = condorGO.GetComponent<CondorController>();
	}

    void Start() {
        gameOverController.HideMenu();
        GameManager.sceneController.hero.StartHero();
        GameManager.HeroCamera.StartCamera();
        levelRocksController.StartRocks();
        paralaxBackgroundController.RestartBackground();

        Vector3 heroStayPoint = GameManager.sceneController.levelRocksController.getCave().StartCave();
        GameManager.sceneController.hero.PutHeroOnStayPoint( heroStayPoint );
        
        //FixHeroOnRock();
    }

    public void RestartGame() {
        GameManager.sceneController.hero.RestartHero();
        GameManager.HeroCamera.Restart();
        GameManager.sceneController.levelRocksController.Restart();
        GameManager.sceneController.gameController.setHeight(0);

        paralaxBackgroundController.RestartBackground();
       // heroGO.SetActive(true);
        levelRocksControllerGO.SetActive(true);
        GameCanvasGO.SetActive( true );

      //  FixHeroOnRock();
      //  GameManager.HeroCamera.Follow();
        GameManager.sceneController.levelRocksController.getCave().StartCave();

        Vector3 heroStayPoint = GameManager.sceneController.levelRocksController.getCave().StartCave();
        GameManager.sceneController.hero.PutHeroOnStayPoint(heroStayPoint);
        
    }

    public Vector3 getCatchHeroPos() {
        return condorController.grabMark.transform.position;
    }

//    public void FixHeroOnRock() {
//      
//        Vector2 heroFixPointPos = GameManager.sceneController.hero.getFixPoint();
//        CrackController crack = GameManager.sceneController.levelRocksController.getStartFixCrack(heroFixPointPos.y);
//        Vector2 fixPoint = crack.getFixPointPosition();
//        
//        GameManager.sceneController.hero.setHero( fixPoint );
//        crack.gameObject.collider2D.enabled = false;
//
//
//      //    Debug.Log( "clothest fix point - " + fixPoint.y );
//    }

  //  void 

    public void EndGame() {
     //   heroGO.SetActive( false );
        levelRocksControllerGO.SetActive( false );
        GameCanvasGO.SetActive( false );
        gameOverController.ShowMenu();
    }
}
