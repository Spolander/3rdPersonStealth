using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotCamera : MonoBehaviour
{

    string path;
    void Start()
    {
        path = Application.dataPath;

        path = path + "/Screenshots";
        print(path);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
			StartCoroutine(ScreenShotRoutine());
        }

    }

    IEnumerator ScreenShotRoutine()
    {
        yield return new WaitForEndOfFrame();
        string filename = path + "/capture.png";
        UnityEngine.ScreenCapture.CaptureScreenshot(filename, 1);
    }
}
