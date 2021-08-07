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
	public int rarity;
	public int visual;
	public int vocal;
	public int dance;
}