using System.IO;
using UnityEngine;
using System.Collections;

public class EdgeStonesController : MonoBehaviour {

	// Use this for initialization
    public Bounds getBounds() {
        return renderer.bounds;
    }
}
