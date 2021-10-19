using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetStoryWebClient : GameWebClient
{

    
    [Serializable]
    public struct GetStoryResponse
    {
        [SerializeField] public string result;
        [SerializeField] public string error;
        [SerializeField] public int id;
        [SerializeField] public string main_story_id;
        [SerializeField] public int lesson_count;
    }

    public GetStoryWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSuccessData(string response)
    {
        GetStoryResponse r = JsonUtility.FromJson<GetStoryResponse>(response);
        base.data = r;
        if (r.result == ConnectionModel.Response.ResultOK)
        {
            Common.progressId = r.id;
            Common.mainstoryid = r.main_story_id;
            Common.lessonCount = r.lesson_count;
            Manager.manager.StateQueue((int)gamestate.Home);
        }
        else if(r.error == "data_not_found")
        {
            Manager.manager.StateQueue((int)gamestate.Home);
        }
        
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        //www.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(" "));
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }
}