using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckVersionWebClient : GameWebClient
{
    [Header("Request Data Information")]
    [SerializeField] protected CheckVersionRequestData checkVersionRequest;

   

    [Serializable]
    public struct CheckVersionRequestData
    {
        [SerializeField] public string platform;
        [SerializeField] public string version_string;
        [SerializeField] public string build_identification;

        public CheckVersionRequestData(string platform, string version_string, string build_identification)
        {
            this.platform = platform;
            this.version_string = version_string;
            this.build_identification = build_identification;
        }
    }

    [Serializable]
    public struct CheckVersionResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
    }

    public void SetData(string platform, string version_string, string build_identification)
    {
        this.checkVersionRequest = new CheckVersionRequestData(platform,version_string,build_identification);
    }


    public CheckVersionWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.checkVersionRequest));
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        EnableExitOnFailure();
    }

    protected override void HandleGameSuccessData(string response)
    {
         CheckVersionResponseData r = JsonUtility.FromJson<CheckVersionResponseData>(response);
        if (r.result != ConnectionModel.Response.ResultOK)
        {
            Common.hasUpdate = true;
        }
        Manager.manager.StateQueue((int)gamestate.Title);
    }
}

