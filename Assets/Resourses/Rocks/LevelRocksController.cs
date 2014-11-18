using UnityEngine;
using System.Collections;

public class LevelRocksController : MonoBehaviour {

    public GameObject caveGO;
    private LevelRocksGenerator levelRockGenerator;

    private CaveController cave;
    

    void Awake() {
        levelRockGenerator = GetComponent<LevelRocksGenerator>();
        cave = caveGO.GetComponent<CaveController>();

    }

    void Start() {
        //cave.hideCave();
    }

    public CaveController getCave() {
        return cave;
    }

 

    public void Restart()
    {
        levelRockGenerator.Restart();
        StartRocks();
    }

   

    public void StartRocks() {

        gameObject.transform.position = new Vector3(
                gameObject.transform.position.x,
                GameManager.HeroCamera.getRockBounds().min.y,
                gameObject.transform.position.z );
        levelRockGenerator.GenerateScreenRocks();
    }

    public CrackController getClothestCrack(float posY) {
        GameObject clothestRockGO = levelRockGenerator.getClothestRock( posY );
        RockController rock = clothestRockGO.GetComponent<RockController>();
        return rock.GetStartClothestFixCrack(posY);
    }

    public void FreezeCracksColliders() {
        levelRockGenerator.FreezeCracksColliders();
    }

    public void UnFreezeCracksColliders()
    {
        levelRockGenerator.UnFreezeCracksColliders();
    }
}
