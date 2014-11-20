using System.Collections.Generic;
using UnityEngine;

public class ParalaxManager : MonoBehaviour {

   
 

    public List<GameObject> backGroundParalaxList;
    private HeroCamera heroCamera;
    private List<ParalaxBackGroundController> backGroundParalaxControllers;

    public GameObject JumpSpeedGO1 ;
    private JumpSpeedController jumpSpeedEffectController1;

    public GameObject JumpSpeedGO2;
    private JumpSpeedController jumpSpeedEffectController2;

    void Awake() {
        heroCamera = GameManager.HeroCamera;
        backGroundParalaxControllers = new List<ParalaxBackGroundController>();
        foreach (GameObject backGameObject in backGroundParalaxList)
        {
            ParalaxBackGroundController bgController = backGameObject.GetComponent<ParalaxBackGroundController>();
            bgController.InitBackGround(heroCamera);
            backGroundParalaxControllers.Add( bgController );
        }

        jumpSpeedEffectController1 = JumpSpeedGO1.GetComponent<JumpSpeedController>();
        jumpSpeedEffectController2 = JumpSpeedGO2.GetComponent<JumpSpeedController>();
    

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

    public void startSpeedEffect(float fadeTime, float height, int EffectN) {

        switch ( EffectN ) {
            case 1:
                jumpSpeedEffectController1.StartSpeed(fadeTime, height);
                break;

            case 2:
                jumpSpeedEffectController2.StartSpeed(fadeTime, height);
                break;
        } 
      //  jumpSpeedEffectController.StartSpeed( fadeTime, height );
    }

    public void stopSpeedEffect(float fadeTime, int EffectN)
    {
        switch (EffectN)
        {
            case 1:
                jumpSpeedEffectController1.StopSpeed(fadeTime);
                break;

            case 2:
                jumpSpeedEffectController2.StopSpeed(fadeTime);
                break;
        }  
    }

}
