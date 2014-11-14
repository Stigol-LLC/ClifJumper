using UnityEngine;
using UnityEngine.UI;

public class UIGameController : MonoBehaviour {



    public GameObject heightGameObject;
    public GameObject coinsGameObject;
    private Text heightText;
    private Text coinsText;

    public delegate void screenTouched();
    public event screenTouched onTouched;

	void Awake () {
	    coinsText = coinsGameObject.GetComponent<Text>();
	    heightText = heightGameObject.GetComponent<Text>();
	    heightText.text = "0m";
	    setCoinsValue( GMDataMngr.GameProgress.totalCoinsCollected );
	  
	}

    public void setHeight( int height ) {
        heightText.text = height + "m";
    }

    public void setCoinsValue( int coinsValue ) {
        coinsText.text = coinsValue.ToString();
    }

    public void onTouchedScreen() {

     
            onTouched();
         

    }



}
