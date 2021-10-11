using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FinishProgressWebClient : GameWebClient
{
    [Header("Request Data Information")]
    [SerializeField] protected FinishProgressRequestData updateCharacterRequestData;


    [Serializable]
    public struct FinishProgressRequestData
    {
        [SerializeField] public string session_id;
        public FinishProgressRequestData(string session_id)
        {
            this.session_id = session_id;
        }
    }

    [Serializable]
    public struct FinishProgressResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
    }

    public void SetData()
    {
        this.updateCharacterRequestData = new FinishProgressRequestData(Common.SessionID);
    }


    public FinishProgressWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.updateCharacterRequestData));
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    }

    protected override void HandleGameSuccessData(string response)
    {
         FinishProgressResponseData r = JsonUtility.FromJson<FinishProgressResponseData>(response);
        if (r.result == ConnectionModel.Response.ResultSuccess)
        {
            Debug.Log("Success");
        }
    }
}

