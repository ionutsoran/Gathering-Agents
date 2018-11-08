using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour {

    public Camera[] cams;
    public GameObject canvas;
    public int indx=0;
    
    public void SwitchCam()
    {

        indx++;

        if (indx == cams.Length)
            indx = 0;

        if (indx== 0)
        {
            cams[cams.Length - 1].gameObject.SetActive(false);
            cams[0].gameObject.SetActive(true);
            canvas.GetComponent<Canvas>().worldCamera = cams[0];
            canvas.transform.rotation = cams[0].transform.rotation;
        }
        else
        {         
            cams[indx - 1].gameObject.SetActive(false);
            cams[indx].gameObject.SetActive(true);
            canvas.GetComponent<Canvas>().worldCamera = cams[indx];
            canvas.transform.rotation = cams[indx].transform.rotation;
        }

    }

    public void ResetTimeScale()
    {
        
        Time.timeScale = 1;
    }

}
