using UnityEngine;
using System.Collections;

public class BackMountainControl : MonoBehaviour {

    void Awake()
    {
        renderer.material.renderQueue = 2001;
    }

   
}
