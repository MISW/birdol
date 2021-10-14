using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateProgressWebClient : GameWebClient
{
    [Header("Request Data Information")]
    [SerializeField] protected CreateProgressRequestData createProgessRequestData;

    [Serializable]
    public struct CreateProgressRequestData
    {
        [SerializeField] public string session_id;
        [SerializeField] public ProgressModel[] character_progresses;
        [SerializeField] public DendouModel[] teachers;
        public CreateProgressRequestData(ProgressModel[] character_progresses, DendouModel[] teachers)
        {
            this.character_progresses = character_progresses;
            this.teachers = teachers;
            this.session_id = Common.SessionID;
        }
    }

    
    [Serializable]
    public struct CharacterResponse
    {
        [SerializeField] public int character_id;
    }

    [Serializable]
    public struct TeacherResponse
    {
        [SerializeField] public int teacher_id;
    }

    [Serializable]
    public struct CreateProgressResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
        [SerializeField] public int progress_id;
        [SerializeField] public CharacterResponse[] characters;
        [SerializeField] public TeacherResponse[] teachers;
    }

    public void SetData(ProgressModel[] characters,DendouModel[] teachers)
    {
        this.createProgessRequestData = new CreateProgressRequestData(characters,teachers);
    }


    public CreateProgressWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return createProgessRequestData.character_progresses.Length == 5 && createProgessRequestData.teachers.Length == 1;
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.createProgessRequestData));
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    }

    protected override void HandleGameSuccessData(string response)
    {
        CreateProgressResponseData result = JsonUtility.FromJson<CreateProgressResponseData>(response);
        GetCharacterWebClient webClient = new GetCharacterWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/character?session_id=" + Common.SessionID);
        StartCoroutine(webClient.Send());
    }
}
