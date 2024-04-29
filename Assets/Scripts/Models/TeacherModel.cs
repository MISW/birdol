using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SQLite;

[Table("teachers")]
public class TeacherModel
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int CharacterId { get; set; }
    [Ignore] public DendouModel character { get; set; }
}