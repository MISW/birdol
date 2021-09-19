using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sailium : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(rotate());
    }

    private IEnumerator rotate()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        float plus = 0;
        bool adding = true;
        while (true)
        {
            if (Math.Abs(plus) == 8.0f) adding = !adding;
            plus += (adding ? 0.5f : -0.5f);
            rect.transform.Rotate(new Vector3(0,0,plus));
            
            yield return new WaitForSeconds(0.02f);
        }
    }
}
