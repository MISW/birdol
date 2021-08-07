using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class StoryData
{
    public Story[] stories;
}

[System.Serializable]
class Story
{
    public int id;
    public Event[] events;
}

[System.Serializable]
class Event
{
    //セリフイベント
    public int characterId;
    public string serifu;
    //選択イベント
    public string choiceName;
    public Choice[] choices;
    //その他
    public string command;
}

[System.Serializable]
class Choice
{
    public int targetStoryId;
    public string choiceName;
}
