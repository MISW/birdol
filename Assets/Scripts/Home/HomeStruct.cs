using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HomeCharacters
{
    public HomeCharacterStruct[] Characters;
}

[System.Serializable]
public class HomeCharacterStruct
{
    public int id;
    public string name;
    public string[] text;

    public float charascale;
    public int charaposx;
    public int charaposy;
}