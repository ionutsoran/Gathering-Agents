using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class hp_background : MonoBehaviour {

	// Use this for initialization
    public GameObject _slider_fill;

	void Update () {
        ChangeColor();
	}

    public void ChangeColor()
    {
        float x = GetComponent<Slider>().value;
        if (x <= 30)
            _slider_fill.GetComponent<Image>().color = Color.red;
        if(x>30&& x<=70)
            _slider_fill.GetComponent<Image>().color = Color.yellow;
        if(x>70)
            _slider_fill.GetComponent<Image>().color = Color.green;
    }
}
