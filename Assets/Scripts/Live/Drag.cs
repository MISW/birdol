using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
    public Text test;
    public GameObject Visual;
    public bool selected = false;
    public int id = 0;
    int characterId;
    public string area = "";
    public string selectedSkill = "";


    void setArea()
    {
        if (this.transform.position.x > Visual.GetComponent<Renderer>().bounds.size.x / 2)
        {
            area = "vocal";
        }
        else if (this.transform.position.x < -1 * Visual.GetComponent<Renderer>().bounds.size.x / 2)
        {
            area = "dance";
        }
        else
        {
            area = "visual";
        }
    }

    void Start()
    {
        setArea();
    }
    void OnMouseDrag()
    {
        if (!selected)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
            foreach(GameObject obj in objs)
            {
                if (obj == this) obj.GetComponent<Drag>().selected = true;
                else obj.GetComponent<Drag>().selected = false;
            }
            LiveController.selectedcharacter = id;
        }
        Debug.Log("Moving:"+id+",area:"+area);
        Vector3 objectPointInScreen
            = Camera.main.WorldToScreenPoint(this.transform.position);
        float kando = 1.0f;
       // if (changeDis>0.01) kando = 1.1f;//For Mobile
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
        setArea();
    }




}
