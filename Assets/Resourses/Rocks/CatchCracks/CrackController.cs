using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System.Collections;

public class CrackController : MonoBehaviour {
    public Vector2 fixPoint;
    public int difficulty;
    private bool prevColliderState;
    private bool _hasCoin;
    public bool hasCoin {
        set {
//            if ( _hasFlag ) {
//                return;
//            }

            _hasCoin = value;
            crackGenerator.setCoinState( _hasCoin );

        }
        get {  return _hasCoin; }
    }

//    private bool _hasFlag;
//    public bool hasFlag
//    {
//        set
//        {
//            _hasFlag = value;
//            crackGenerator.setFlagState(_hasFlag);
//
//            if ( _hasFlag ) {
//                hasCoin = false;
//            }
//
//        }
//        get { return _hasFlag; }
//    }


    private CrackGenerator crackGenerator;

    void Awake() {
        crackGenerator = GetComponent<CrackGenerator>();
        crackGenerator.setCoinState(false);
        crackGenerator.setFlagState(false);
    }


    public void FreezeCollider() {
        prevColliderState = gameObject.collider2D.enabled;

        gameObject.collider2D.enabled = false;
    }

    public void UnFreezeCollider() {
        gameObject.collider2D.enabled = prevColliderState;
    }

    public void heroCatch() {
//        if ( hasFlag ) {
//            CaveController cave = GameManager.sceneController.levelRocksController.getCave();
//            Vector3 cavePosition = cave.transform.position;
//            Debug.Log("cave height - " + cave.bounds.size.y);
//            Vector3 newCavePosition = new Vector3(cavePosition.x,transform.position.y - 70, cavePosition.z);
//
//            cave.transform.position = newCavePosition;
//            cave.showCave();
//            GameManager.sceneController.gameController.StartOnCheckPoint();
//            hasFlag = false;
//
//        }


        if ( hasCoin ) {
            _hasCoin = false;
            crackGenerator.collectCoin();
            GMDataMngr.GameProgress.totalCoinsCollected ++;
            GameManager.sceneController.gameController.setCoinsValue(GMDataMngr.GameProgress.totalCoinsCollected);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x + fixPoint.x, transform.position.y + fixPoint.y, transform.position.z), 10);
    }

    public Vector2 getFixPointPosition()
    {
        return new Vector2(gameObject.transform.position.x + fixPoint.x, gameObject.transform.position.y + fixPoint.y);
    }

 


}
