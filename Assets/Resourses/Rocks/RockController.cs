
using UnityEngine;
using System.Collections.Generic;

public class RockController : MonoBehaviour {

    private RockGenerator rockGenerator;
    private float moveCoinPercentage;
   

    void Awake() {
        rockGenerator = gameObject.GetComponent<RockGenerator>();
    }

    public void FreezeCrackColliders() {
        rockGenerator.FreezeCracksCollider();
    }

    public void UnFreezeCrackColliders() {
        rockGenerator.UnFreezeCracksColliders();
    }

    public CrackController GetStartClothestFixCrack(float posY) {
        return rockGenerator.getClothestStartFixCrack( posY );
    }


    public Bounds getBounds() {
        return renderer.bounds;
    }


}
