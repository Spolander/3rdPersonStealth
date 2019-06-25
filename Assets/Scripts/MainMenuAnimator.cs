using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Rendering.PostProcessing;
public class MainMenuAnimator : MonoBehaviour
{



    // properties of class
    public float bloomMin = 0;
    public float bloomMax = 4;

    public float sinSpeed = 4;
    private float sinTimer;
  

    Bloom bloomLayer = null;


    void Start()
    {
        // somewhere during initializing
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);

    }

    void Update()
    {


        bloomLayer.intensity.value = Mathf.Lerp(bloomMin, bloomMax, (Mathf.Sin(sinTimer)+1)/2);
        sinTimer += Time.deltaTime*sinSpeed;

    }
}


