using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
    Vector3 beforePos;
    float changeDis;
    public Text test;
    public GameObject Visual;

    void Start()
    {
        beforePos = this.transform.position;
    }
    void OnMouseDrag()
    {
        Vector3 objectPointInScreen
            = Camera.main.WorldToScreenPoint(this.transform.position);
        float kando = 1.0f;
       // if (changeDis>0.01) kando = 1.1f;//For Mobile
        beforePos = this.transform.position;
        Vector3 mousePointInScreen
            = new Vector3(Input.mousePosition.x,
                          Input.mousePosition.y*kando,
                          objectPointInScreen.z);
        Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(mousePointInScreen);
        mousePointInWorld.y = this.transform.position.y;
        this.transform.position = mousePointInWorld;
        
    }

    void Update()
    {
        changeDis = Math.Abs(beforePos.z-this.transform.position.z);
        beforePos = this.transform.position;
        Debug.Log("x:"+this.transform.position.x+"Width:"+Visual.GetComponent<Renderer>().bounds.size.x/2);    
        if(this.transform.position.x > Visual.GetComponent<Renderer>().bounds.size.x / 2)
        {
            test.text = "CurrentArea:Vocal";
        }
        else if (this.transform.position.x <-1* Visual.GetComponent<Renderer>().bounds.size.x / 2)
        {
            test.text = "CurrentArea:Dance";
        }
        else
        {
            test.text = "CurrentArea:Visual";
        }
    }




}
