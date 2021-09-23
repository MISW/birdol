using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sailium : MonoBehaviour
{
    public int layer;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(rotate());
    }

    public void enableSailium()
    {
        string imagename = RandomArray.GetRandom(new List<string>() { "blue", "pink", "yellow" });
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/lightstick_"+imagename);
        Color newcolor = GetComponent<Image>().color;
        newcolor.r -= (20f * layer / 255f);
        newcolor.g -= (20f * layer / 255f);
        newcolor.b -= (20f * layer / 255f);
        GetComponent<Image>().color = newcolor;
        GetComponent<Image>().enabled = true;
    }

    private IEnumerator rotate()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        float plus = 0;
        bool adding = true;
        while (true)
        {
            if (Math.Abs(plus) == 5.5f) adding = !adding;
            plus += (adding ? 0.25f : -0.25f);
            rect.transform.Rotate(new Vector3(0,0,plus));
            
            yield return new WaitForSeconds(0.01f);
        }
    }
}
