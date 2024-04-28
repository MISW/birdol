
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        foreach (DendouModel character in characters)
        {
            Debug.Log("character:" + character.Visual);
            Debug.Log("character:" + character.Vocal);

        }
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


    public static void EndStory()
    {

    }

}