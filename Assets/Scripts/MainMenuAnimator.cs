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

    ColorGrading colorGrading = null;

    public float exposureMax = -0.1f;
    public float exposureMin = -10;
    public float effectTime = 4;


    void Start()
    {
        // somewhere during initializing
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);
        volume.profile.TryGetSettings(out colorGrading);

    }

    public void FadeToBlack()
    {
        StartCoroutine(fadeAnimation());
    }

    IEnumerator fadeAnimation()
    {
        float timer = 0;

        while(timer < 1)
        {
            colorGrading.postExposure.value = Mathf.Lerp(exposureMax, exposureMin, timer);
            timer += Time.deltaTime/effectTime;
            yield return null;
        }

       
    }

    void Update()
    {


        bloomLayer.intensity.value = Mathf.Lerp(bloomMin, bloomMax, (Mathf.Sin(sinTimer)+1)/2);
        sinTimer += Time.deltaTime*sinSpeed;

    }
}


