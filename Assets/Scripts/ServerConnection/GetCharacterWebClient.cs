using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetCharacterWebClient : GameWebClient
{
    [Serializable]
    public struct GetCharacterResponse
    {
        [SerializeField] public string result;
        [SerializeField] public string error;
        [SerializeField] public ProgressModel[] character_progresses;
        [SerializeField] public TeacherModel[] teachers;
    }

    public GetCharacterWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSuccessData(string response)
    {
        GetCharacterResponse r = JsonUtility.FromJson<GetCharacterResponse>(response);
        base.data = r;
        if (r.character_progresses.Length == 5 && r.teachers.Length == 1)
        {
            for (int i=0;i < 5;i++)
            {
                Common.progresses[i] = r.character_progresses[i];
#if UNITY_EDITOR
                Debug.Log(Common.progresses[i].id+":"+ Common.progresses[i].Name);
#endif
            }
            Common.teacher = r.teachers[0].character;
            if (Common.lessonCount<5&&Common.lessonCount>0)
            {
                Manager.manager.StateQueue((int)gamestate.Lesson);
            }
            else
            {
                Manager.manager.StateQueue((int)gamestate.Story);
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Connection Error Occured.");
#endif
        }

    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        //www.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(" "));
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }
}
