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
	//メインキャラクター
	public int characterId;
	public string characterName;
	public float visual;
	public float vocal;
	public float dance;
	public int activeSkillLevel = 1;
	public SkillType activeSkillType;
	public float activeSkillScore;
	public int mainStoryId;
	//サポートキャラクター
	public int supportCharacterId;
	public int passiveSkillLevel = 1;
	public SkillType passiveSkilltype;
	public float passiveSkillScore;
}