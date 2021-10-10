using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateMainStoryWebClient : GameWebClient
{
    public int sceneid;

    [Header("Request Data Information")]
    [SerializeField] protected UpdateMainStoryRequestData updateMainStoryRequestData;

    
    [Serializable]
    public struct UpdateMainStoryRequestData
    {
        [SerializeField] public string session_id;
        [SerializeField] public string main_story_id ;
        public UpdateMainStoryRequestData(string main_story_id)
        {
            
            this.session_id = Common.SessionID;
            this.main_story_id = main_story_id;
        }
    }

    [Serializable]
    public struct UpdateMainStoryResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
    }

    public void SetData(string main_story_id)
    {
        this.updateMainStoryRequestData = new UpdateMainStoryRequestData(main_story_id);
    }


    public UpdateMainStoryWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return updateMainStoryRequestData.main_story_id != null;
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.updateMainStoryRequestData));
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    }

    protected override void HandleGameSuccessData(string response)
    {
        UpdateMainStoryResponseData r= JsonUtility.FromJson<UpdateMainStoryResponseData>(response);
        if (r.result == ConnectionModel.Response.ResultSuccess)
        {
            Manager.manager.StateQueue(sceneid);
        }
       
    }
}

