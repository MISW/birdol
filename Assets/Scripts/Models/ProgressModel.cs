using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

public enum SkillType
{
	Plus,
	Multiply
}


[Table("progresses")]
public class ProgressModel
{
	[PrimaryKey, AutoIncrement]
	public int id { get; set; }
	public int MainCharacterId { get; set; }
	public string Name { get; set; }
	public float Visual { get; set; }
	public float Vocal { get; set; }
	public float Dance { get; set; }
	public int ActiveSkillLevel { get; set; } = 1;
	public int SupportCharacterId { get; set; }
	public int PassiveSkillLevel { get; set; } = 1;

	[Ignore] public string Group { get; set; }
	[Ignore] public string BestSkill { get; set; } //(0:vocal 1:visual 2: dance)

	[Ignore] public string ActiveSkillName { get; set; }
	[Ignore] public string ActiveSkillType { get; set; }
	[Ignore] public string ActiveSkillParams { get; set; }
	[Ignore] public string ActiveSkillDescription { get; set; }
	[Ignore] public float ActiveSkillScore { get; set; }

	[Ignore] public string SupportSkillName { get; set; }
	[Ignore] public string PassiveSkillType { get; set; }
	[Ignore] public string PassiveSkillParams { get; set; }
	[Ignore] public string PassiveSkillDescription { get; set; }
	[Ignore] public float PassiveSkillScore { get; set; }
	[Ignore] public float PassiveSkillProbability { get; set; }
}