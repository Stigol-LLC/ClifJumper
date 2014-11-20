using UnityEngine;
using System.Collections;

public class CrackGenerator : MonoBehaviour {

    public GameObject coinGO;
    public GameObject flagGO;

    private float moveCoinPercentage;
    private bool isCoinFlying;
    private Bezier coinFlyBezier;

    void Awake() {
        moveCoinPercentage = 0;
        isCoinFlying = false;
    }

    public void setCoinState(bool isAvailable) {
        coinGO.SetActive( isAvailable );
    }

    public void setFlagState( bool isAvailable ) {
        flagGO.SetActive( isAvailable );
    }

    public void collectCoin() {
       GameManager.sceneController.levelRocksController.audio.Play();

        Vector3 heroBagPosition = GameManager.sceneController.hero.getCoinDropPoint();
        Vector3 midVector1 = new Vector3(coinGO.transform.position.x - 150, coinGO.transform.position.y + 50, coinGO.transform.position.z);
        Vector3 midVector2 = new Vector3(heroBagPosition.x - 150, heroBagPosition.y - 50, heroBagPosition.z);


        coinFlyBezier = new Bezier(coinGO.transform.position, heroBagPosition, midVector1, midVector2);

//        Debug.Log("Start - " + coinGO.transform.position);
//        Debug.Log("Finish - " + heroBagPosition);
//
//        Debug.Log("Hero position - " + GameManager.sceneController.hero.transform.position);
//
//        Debug.Log("Mid vector 1 - " + midVector1);
//        Debug.Log("Mid vector 2 - " + midVector2);

        moveCoinPercentage = 0;
        isCoinFlying = true;

    }

    private void Update() {
                if (isCoinFlying == false)
                    return;
        
               coinGO.transform.position = coinFlyBezier.GetBezierPointAtTime( moveCoinPercentage );
                moveCoinPercentage += Time.deltaTime * 2;

        if ( moveCoinPercentage > 1 ) {
           
            coinGO.SetActive( false );
            isCoinFlying = false;
        }
    }

}
