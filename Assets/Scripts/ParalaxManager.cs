﻿using System.Collections.Generic;
using UnityEngine;

public class ParalaxManager : MonoBehaviour {

   
 

    public List<GameObject> backGroundParalaxList;
    private HeroCamera heroCamera;
    private List<ParalaxBackGroundController> backGroundParalaxControllers;
    public GameObject JumpSpeedGO ;

    private JumpSpeedController jumpSpeedEffectController;

    void Awake() {
        heroCamera = GameManager.HeroCamera;
        backGroundParalaxControllers = new List<ParalaxBackGroundController>();
        foreach (GameObject backGameObject in backGroundParalaxList)
        {
            ParalaxBackGroundController bgController = backGameObject.GetComponent<ParalaxBackGroundController>();
            bgController.InitBackGround(heroCamera);
            backGroundParalaxControllers.Add( bgController );
        }

        jumpSpeedEffectController = JumpSpeedGO.GetComponent<JumpSpeedController>();
    }

    public void RestartBackground() {
        foreach ( GameObject backGameObject in backGroundParalaxList ) {
            ParalaxBackGroundController bgController = backGameObject.GetComponent<ParalaxBackGroundController>();
            bgController.StartBackground();
        }
    }

    public void ParalaxUpdate() {
        foreach ( ParalaxBackGroundController paralaxController in backGroundParalaxControllers ) {
            paralaxController.UpdateParalax();
        }
    }

    public void startSpeedEffect(float fadeTime) {
        
    }

    void stopSpeedEffect(float fadeTime) {
        
    }

}
