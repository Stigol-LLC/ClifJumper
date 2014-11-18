using System.Linq.Expressions;
using UnityEngine;
using System.Collections.Generic;

public class JumpSpeedController : MonoBehaviour {
    public List<GameObject> speedPictList;
    Vector2 uvOffset = Vector2.zero;
    private float uvAnimationRate;
    private float fadeRate;

    private bool isPlaying;

    void Awake() {
        uvAnimationRate = 1f;
        isPlaying = true;
    }

    public void StartSpeed(float fadeTime, float height) {
        Color prevColor = renderer.material.color;
        renderer.material.color = new Color(prevColor.r, prevColor.g, prevColor.b, 0);
      
        isPlaying = true;
        fadeRate = -1 / fadeTime ;
    }

    public void StopSpeed( float fadeTime ) {
        Color prevColor = renderer.material.color;
        renderer.material.color = new Color(prevColor.r, prevColor.g, prevColor.b, 1);

        fadeRate = 1 / fadeTime;
        isPlaying = false;
    }

    void LateUpdate()
    {
        if (!isPlaying)
            return;

        uvOffset.y += (uvAnimationRate * Time.deltaTime);
    
        if (renderer.enabled) {
            Color prevColor = renderer.material.color;
            //renderer.material.SetTextureOffset("_MainTex", uvOffset);
            renderer.material.color = new Color(1,1,1, prevColor.a - Time.deltaTime * fadeRate);
            renderer.material.mainTextureOffset = uvOffset;
           // renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureOffset.x, renderer.material.mainTextureOffset.y + uvOffset.y);
           // renderer.material.mainTextureScale = 
        }
    }
}
