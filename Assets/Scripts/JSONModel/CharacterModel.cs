using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommonCharacters
{
    public CharacterModel[] characters;
}

[System.Serializable]
public class CharacterModel
{
	public int id;
	public string name;
	public string group;
	public int rarity;
	public string bestskill;
	public int vocal;
	public int visual;
	public int dance;
	public string skillname;
    public string activetype;
	public string activeparams;
	public float activeskillscore;
	public string activedescription;
	public string passivetype;
	public string passiveparams;
	public float passiveskillscore;
	public string passivedescription;
	public float passiveprobability;
}