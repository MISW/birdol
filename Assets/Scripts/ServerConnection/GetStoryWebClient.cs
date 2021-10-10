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
        if (r.result == ConnectionModel.Response.ResultSuccess)
        {
            Common.progressId = r.id;
            Common.mainstoryid = r.main_story_id;
            Manager.manager.StateQueue((int)gamestate.Home);
        }
        else if(r.error == "äYìñÇ∑ÇÈêiíªÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒ")
        {
            Manager.manager.StateQueue((int)gamestate.Home);
        }
        
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(" "));
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }
}