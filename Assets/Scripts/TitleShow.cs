using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Just an object to show some placards with info for a few seconds
/// </summary>
public class TitleShow : MonoBehaviour
{
    Camera cam;

    //This method will be called from GM everytime
    // you want to show a text or img on screen
    public void showTitle(RawImage im, System.Action callback = null) {
        StartCoroutine(showTitleCoroutine(im,callback));
    }

    IEnumerator showTitleCoroutine(RawImage im, System.Action callback= null)
    {
        im.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        im.gameObject.SetActive(false);

        if (callback != null)
            callback();
    }

    //Not used
    public void Init(Camera c) {
        cam = c;
        int ph = cam.pixelHeight;
        int pw = cam.pixelWidth;
    }
}
