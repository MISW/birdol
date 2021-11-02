using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateCharacterWebClient : GameWebClient
{
    [Header("Request Data Information")]
    [SerializeField] protected UpdateCharacterRequestData updateCharacterRequestData;

    public bool isReturnLesson = false;

    [Serializable]
    public struct UpdateCharacterRequestData
    {
        [SerializeField] public string session_id;
        [SerializeField] public ProgressModel[] character_progresses;
        public UpdateCharacterRequestData(ProgressModel[] progresses)
        {
            this.session_id = Common.SessionID;
            this.character_progresses = progresses;
        }
    }

    [Serializable]
    public struct UpdateCharacterResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
    }

    public void SetData()
    {
        this.updateCharacterRequestData = new UpdateCharacterRequestData(Common.progresses);
    }


    public UpdateCharacterWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return updateCharacterRequestData.character_progresses.Length == 5;
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
        UpdateCharacterResponseData r = JsonUtility.FromJson<UpdateCharacterResponseData>(response);
        if (r.result == ConnectionModel.Response.ResultSuccess)
        {
#if UNITY_EDITOR
            Debug.Log("Success");
#endif
        }
    }
}
