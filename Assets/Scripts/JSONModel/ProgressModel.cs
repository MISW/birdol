using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressData
{
    public ProgressModel[] progresses;
}

public enum SkillType
{
	Plus,
	Multiply
}

[System.Serializable]
public class ProgressModel
{
	public int id;
	//メインキャラクター
	public int MainCharacterId;
	public string Name;
	public float Visual;
	public float Vocal;
	public float Dance;
	public string Group;
	public string BestSkill; //(0:vocal 1:visual 2: dance)
	public int ActiveSkillLevel = 1;

	public string ActiveSkillName;
	public string ActiveSkillType;
	public string ActiveSkillParams;
	public string ActiveSkillDescription;
	public float ActiveSkillScore;
	//サポートキャラクター
	public int SupportCharacterId;

	public string SupportSkillName;
	public int PassiveSkillLevel = 1;
	public string PassiveSkillType;
	public string PassiveSkillParams;
	public string PassiveSkillDescription;
	public float PassiveSkillScore;
	public float PassiveSkillProbability;
}