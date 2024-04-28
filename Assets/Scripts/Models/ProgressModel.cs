using System;
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
	//���C���L�����N�^�[
	public int MainCharacterId;
	public string Name;
	public float Visual;
	public float Vocal;
	public float Dance;
	public int ActiveSkillLevel = 1;
	public int SupportCharacterId;
	public int PassiveSkillLevel = 1;

	[NonSerialized] public string Group;
	[NonSerialized] public string BestSkill; //(0:vocal 1:visual 2: dance)

	[NonSerialized] public string ActiveSkillName;
	[NonSerialized] public string ActiveSkillType;
	[NonSerialized] public string ActiveSkillParams;
	[NonSerialized] public string ActiveSkillDescription;
	[NonSerialized] public float ActiveSkillScore;

	[NonSerialized] public string SupportSkillName;
	[NonSerialized] public string PassiveSkillType;
	[NonSerialized] public string PassiveSkillParams;
	[NonSerialized] public string PassiveSkillDescription;
	[NonSerialized] public float PassiveSkillScore;
	[NonSerialized] public float PassiveSkillProbability;
}