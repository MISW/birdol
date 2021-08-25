using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HomeButtonUtil : MonoBehaviour
{

    public GameObject Home;
    public GameObject Gallery;
    public GameObject Gacha;
    public GameObject Story;
    public GameObject Hensei;
    public GameObject Ikusei;
    public GameObject Rank;
    public GameObject EnterCameraMode;
    public GameObject ExitCameraMode;
    public GameObject CharacterDialog;


    public void onButtonPressed_EnterCameraMode()
    {
        Home.SetActive(false);
        Gallery.SetActive(false);
        Gacha.SetActive(false);
        Story.SetActive(false);
        Hensei.SetActive(false);
        Ikusei.SetActive(false);
        Rank.SetActive(false);
        Ikusei.SetActive(false);
        EnterCameraMode.SetActive(false);
        CharacterDialog.SetActive(false);
        ExitCameraMode.SetActive(true);
    }

    public void onButtonPressed_ExitCameraMode()
    {
        Home.SetActive(true);
        Gallery.SetActive(true);
        Gacha.SetActive(true);
        Story.SetActive(true);
        Hensei.SetActive(true);
        Ikusei.SetActive(true);
        Rank.SetActive(true);
        Ikusei.SetActive(true);
        EnterCameraMode.SetActive(true);
        CharacterDialog.SetActive(true);
        ExitCameraMode.SetActive(false);
    }


}
