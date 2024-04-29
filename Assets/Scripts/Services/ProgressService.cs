
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Move data management to client side
public class ProgressService
{
    public static void InitData()
    {
        var db = DatabaseManager.getDB();
        for (int i = 0; i < 4; i++)
        {
            var dendou = new DendouModel();
            dendou.MainCharacterId = i;
            dendou.SupportCharacterId = i;
            dendou.Name = Common.characters[i].name;
            dendou.Vocal = Common.characters[i].vocal;
            dendou.Visual = Common.characters[i].visual;
            dendou.Dance = Common.characters[i].dance;
            db.Insert(dendou);
        }
    }

    public static void FetchCompletedProgressAndUpdateGameStatus(string target)
    {
        var db = DatabaseManager.getDB();
        var characters = db.Table<DendouModel>().ToList();
        if (characters.Count >= 4)
        {
            if (target == "gachaunit")
            {
                GachaUnitManager.teachers.Clear();
                foreach (DendouModel character in characters)
                {
#if UNITY_EDITOR
                    Debug.Log("character:"+character.Name);
#endif
                    GachaUnitManager.teachers.Add(character);
                }
                Manager.manager.StateQueue((int)gamestate.GachaUnit);
            }
            else if (target == "completed")
            {
                CompletedController.CompletedCharacters.Clear();
                foreach (DendouModel character in characters)
                {
#if UNITY_EDITOR
                    Debug.Log("character:" + character.Name);
#endif
                    CompletedController.CompletedCharacters.Add(character);
                }
                Manager.manager.StateQueue((int)gamestate.CompletedCharacters);
            }
            else if (target == "freeselect")
            {
                FreeSelectManager.CompletedCharacters.Clear();
                foreach (DendouModel character in characters)
                {
#if UNITY_EDITOR
                    Debug.Log("character:" + character.Name);
#endif
                    FreeSelectManager.CompletedCharacters.Add(character);
                }
                Manager.manager.StateQueue((int)gamestate.FreeSelect);
            }
            else if (target == "home")
            {
                HomeUtil.isUnlocked = new List<bool>(new bool[32]);
                foreach (DendouModel character in characters)
                {
                    HomeUtil.isUnlocked[character.MainCharacterId] = true;
                }
                Manager.manager.StateQueue((int)gamestate.Home);
            }
            else if (target == "gallery")
            {
                for (int i = 0; i < 32; i++)
                {
                    GalleryManager.SetIsUnlocked(i, false);
                }
                foreach (var character in characters)
                {
                    GalleryManager.SetIsUnlocked(character.MainCharacterId, true);
                }
                Manager.manager.StateQueue((int)gamestate.Gallery);
            }
        }

    }

    public static void FetchStory()
    {
        var db = DatabaseManager.getDB();
        Common.remainingSubstory.Clear();
        for (int i = 10; i <= 16; i++)
        {
            if (!Common.TriggeredSubStory.Contains(i.ToString())) Common.remainingSubstory.Add(i);
        }
        Manager.manager.StateQueue((int)gamestate.Home);
    }

    public static void NewStory()
    {
        Common.MainStoryId = "opening";
        Common.LessonCount = 5;
    }

    public static void UpdateStory(string mainStoryId, int lessonCount, int sceneId)
    {
        Common.MainStoryId = mainStoryId;
        Common.LessonCount = lessonCount;
        if (sceneId != 0)
        {
            Manager.manager.StateQueue(sceneId);
        }
    }


    public static void EndStory(int sceneId)
    {
        NewStory();
        Manager.manager.StateQueue(sceneId);
    }

    public static void NewProgress(ProgressModel[] characters, DendouModel[] completedCharacters)
    {
        var db = DatabaseManager.getDB();
        db.RunInTransaction(() =>
        {
            foreach (var character in characters)
            {
                db.Insert(character);
            }
            var teachers = new List<TeacherModel>();
            foreach (var completedCharacter in completedCharacters)
            {
                var teacher = new TeacherModel();
                teacher.character = completedCharacter;
                teacher.CharacterId = completedCharacter.id;
                db.Insert(teacher);
#if UNITY_EDITOR
            Debug.Log("TeacherId:" + teacher.id);
#endif
                teachers.Add(teacher);
            }
            for (int i = 0; i < 5; i++)
            {
                Common.progresses[i].id = characters[i].id;
#if UNITY_EDITOR
                Debug.Log("NewId:"+ Common.progresses[i].id);
#endif
            }
            Common.teacher.id = teachers[0].id;
            Common.MainStoryId = "1a";


        });
        Manager.manager.StateQueue((int)gamestate.Story);
    }

    public static void UpdateProgress(ProgressModel[] characters)
    {
        var db = DatabaseManager.getDB();
        foreach (var character in characters)
        {
            db.Update(character);
        }
    }

    public static void FetchProgress()
    {
        var db = DatabaseManager.getDB();
        var character_progresses = db.Table<ProgressModel>()
                .OrderByDescending(p => p.id)
                .Take(5).ToList();
        var teachers = db.Table<TeacherModel>()
                .OrderByDescending(p => p.id)
                .Take(1).ToList();
        var teacher_character_id = teachers[0].CharacterId;
        var teacher_characters = db.Table<DendouModel>().Where(t => t.id == teacher_character_id).ToList();
        for (int i = 0; i < 5; i++)
        {
            Common.progresses[i] = character_progresses[i];
#if UNITY_EDITOR
                Debug.Log(Common.progresses[i].id+":"+ Common.progresses[i].Name);
#endif
        }
        Common.teacher = teacher_characters[0];
        if (Common.lessonCount < 5 && Common.lessonCount > 0)
        {
            Manager.manager.StateQueue((int)gamestate.Lesson);
        }
        else
        {
            Manager.manager.StateQueue((int)gamestate.Story);
        }

    }

    public static void EndProgress()
    {
        var db = DatabaseManager.getDB();
        db.RunInTransaction(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                var progress = Common.progresses[i];
                db.Insert(new DendouModel
                {
                    MainCharacterId = progress.MainCharacterId,
                    Name = progress.Name,
                    Visual = progress.Visual,
                    Vocal = progress.Vocal,
                    Dance = progress.Dance,
                    SupportCharacterId = progress.SupportCharacterId,
                    ActiveSkillLevel = progress.ActiveSkillLevel,
                    PassiveSkillLevel = progress.PassiveSkillLevel,
                });
            }
            db.DeleteAll<ProgressModel>();
            db.DeleteAll<TeacherModel>();
        });
    }

}