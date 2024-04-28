using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;



[System.Serializable]
[Table("completed_characters")]
public class DendouModel
{
	[PrimaryKey, AutoIncrement]
	public int id { get; set; }
	public int MainCharacterId { get; set; }
	public string Name { get; set; }
	public float Visual { get; set; }
	public float Vocal { get; set; }
	public float Dance { get; set; }
	public string BestSkill { get; set; }
	public int ActiveSkillLevel { get; set; } = 1;
	public string ActiveSkillType { get; set; }
	public float ActiveSkillScore { get; set; }
	public int MainStoryId { get; set; }
	public int SupportCharacterId { get; set; }
	public int PassiveSkillLevel { get; set; } = 1;
	public string PassiveSkillType { get; set; }
	public float PassiveSkillScore { get; set; }
}

