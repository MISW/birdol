using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class DendouModel
{
	public int id;
	//メインキャラクター
	public int MainCharacterId;
	public string Name;
	public float Visual;
	public float Vocal;
	public float Dance;
	public string BestSkill; //(0:vocal 1:visual 2: dance)
	public int ActiveSkillLevel = 1;
	public string ActiveSkillType;
	public float ActiveSkillScore;
	public int MainStoryId;
	//サポートキャラクター
	public int SupportCharacterId;
	public int PassiveSkillLevel = 1;
	public string PassiveSkillType;
	public float PassiveSkillScore;
}

