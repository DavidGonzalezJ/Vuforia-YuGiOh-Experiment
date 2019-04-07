using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleShow : MonoBehaviour
{
    Camera cam;

    //This method will be called from GM everytime
    // you want to show a text or img on screen
    public void showTitle(RawImage im) {
        StartCoroutine(showTitleCoroutine(im));
    }

    IEnumerator showTitleCoroutine(RawImage im)
    {

        im.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        im.gameObject.SetActive(false);
    }

    //Not used
    public void Init(Camera c) {
        cam = c;
        int ph = cam.pixelHeight;
        int pw = cam.pixelWidth;
    }

    // Start is called before the first rame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
