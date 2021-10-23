using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sailium : MonoBehaviour
{
    public int layer;
    // Start is called before the first frame update
    List<String> keys = new List<string>() { "blue", "pink", "yellow" };
    public static Dictionary<String, Sprite> map = new Dictionary<String, Sprite>();
    WaitForSeconds wait = new WaitForSeconds(0.001f);

    void Start()
    {
        Application.targetFrameRate = 60;
        if(!map.ContainsKey("blue"))map.Add("blue", Resources.Load<Sprite>("Images/Live/lightstick_blue"));
        if (!map.ContainsKey("pink")) map.Add("pink", Resources.Load<Sprite>("Images/Live/lightstick_pink"));
        if (!map.ContainsKey("yellow")) map.Add("yellow", Resources.Load<Sprite>("Images/Live/lightstick_yellow"));
        StartCoroutine(rotate());
    }

    public void enableSailium()
    {
        string imagename = RandomArray.GetRandom(keys);
        Image image = GetComponent<Image>();
        image.sprite = map[imagename];
        Color newcolor = image.color;
        newcolor.r -= (20f * layer / 255f);
        newcolor.g -= (20f * layer / 255f);
        newcolor.b -= (20f * layer / 255f);
        image.color = newcolor;
        image.enabled = true;
        StartCoroutine(fadeIn(newcolor));
    }

    private IEnumerator fadeIn(Color color)
    {
        Image image = GetComponent<Image>();
        for (float i=0;i<=255f;i+=5f)
        {
            color.a = (i/255f);
            image.color = color;
            yield return wait;
        }
        
    }



    private IEnumerator rotate()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        float plus = 0;
        bool adding = true;
        var fixedupdate = new WaitForFixedUpdate();
        while (true)
        {
            if (Math.Abs(plus) == 5.5f) adding = !adding;
            plus += (adding ? 0.25f : -0.25f);
            rect.transform.Rotate(new Vector3(0,0,plus));
            yield return fixedupdate;
        }
    }
}
