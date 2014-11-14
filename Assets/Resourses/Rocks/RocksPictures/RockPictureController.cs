using UnityEngine;
using System.Collections;

public class RockPictureController : MonoBehaviour {

    public Bounds bounds;

    void Awake() {
        bounds = renderer.bounds;
    }
}
