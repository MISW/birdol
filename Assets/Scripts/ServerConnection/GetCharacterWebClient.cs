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
        if (r.result == ConnectionModel.Response.ResultSuccess)
        {
            for (int i=0;i < 5;i++)
            {
                Common.progresses[i] = r.character_progresses[i];
                Debug.Log(Common.progresses[i].id+":"+ Common.progresses[i].Name);
            }
            Common.teacher = r.teachers[0].character;
            Manager.manager.StateQueue((int)gamestate.Story);
        }
        else
        {
            Debug.Log("Connection Error Occured.");
        }

    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(" "));
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }
}
