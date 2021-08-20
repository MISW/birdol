using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveController : MonoBehaviour
{
    public GameObject startbutton;
    public static GameObject dancebutton;
    public static GameObject visualbutton;
    public static GameObject vocalbutton;
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
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        foreach (GameObject obj in objs)
        {
            string area = obj.GetComponent<Drag>().area;
            if (area == "dance") dance++;
            else if (area == "visual") visual++;
            else if (area == "vocal") vocal++;
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
        checkPos();   
    }

    // Update is called once per frame
    void Update()
    {
        checkPos();
    }
}
