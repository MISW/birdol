using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

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
    //�Z���t�C�x���g
    public int characterId;
    public string serifu;
    //�I���C�x���g
    public string choiceName;
    public Choice[] choices;
    //���̑�
    public string command;
}

[System.Serializable]
class Choice
{
    public int targetStoryId;
    public string choiceName;
}