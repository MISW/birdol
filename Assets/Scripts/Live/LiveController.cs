using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LiveController : MonoBehaviour
{
    public GameObject startbutton;
    public GameObject light;
    int dance = 0;
    int visual = 0;
    int vocal = 0;
    public static int selectedcharacter = 0;
    public static string[] selectedskills = new string[5];
    // Start is called before the first frame update

    public static void setupSkillButton(int index)
    {

    }

    void checkPos()
    {
        dance = 0;
        visual = 0;
        vocal = 0;
        Debug.Log("CurrentCharacter:"+ selectedcharacter);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        Array.Sort(objs, delegate (GameObject a1, GameObject a2) { return -1*a1.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y
            .CompareTo(a2.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y); });
        int depth = 0;
        for (int i=0;i<5;i++)
        {
            string area = objs[i].GetComponent<Drag>().area;
            if (area == "dance") dance++;
            else if (area == "visual") visual++;
            else if (area == "vocal") vocal++;
            objs[i].transform.parent.gameObject.transform.SetSiblingIndex(i+1);
            if (objs[i].GetComponent<Drag>().id == selectedcharacter)
            {
                Vector2 position = objs[i].transform.parent.gameObject.transform.localPosition;
                position.y += 450;
                light.transform.localPosition = position;
                light.transform.SetSiblingIndex(0);
            }
        }
        if (dance <= 2 && visual <= 2 && vocal <= 2)
        {
            startbutton.SetActive(true);
        }
        else
        {
            startbutton.SetActive(false);
        }
    }

    
    void Start()
    {
        light.SetActive(true);
        checkPos();   
    }

    // Update is called once per frame
    void Update()
    {
        checkPos();
    }
}
