using UnityEngine;
using System.Collections;

public class BackGroundController : MonoBehaviour {
   
    public Bounds bounds;
    public int rendererQueueValue;

    void Awake() {
        if (rendererQueueValue == 0)
            rendererQueueValue = 2000;



        if (renderer != null)
        {
            renderer.material.renderQueue = rendererQueueValue;
            bounds = renderer.bounds;
        }

    }
}
