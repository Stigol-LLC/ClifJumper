using UnityEngine;
using System.Collections;

public class GameManager {

    public  static SceneController sceneController = GameObject.Find( "SceneController" ).GetComponent<SceneController>();
    public static HeroCamera HeroCamera = GameObject.Find( "Main Camera" ).GetComponent<HeroCamera>();
}
