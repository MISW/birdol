using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetGalleryWebClient : GameWebClient
{
    [Serializable]
    public struct GetCompletedResponse
    {
        [SerializeField] public string result;
        [SerializeField] public string error;
        [SerializeField] public GalleryModel[] birdols;
    }

    public GetGalleryWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSuccessData(string response)
    {
        GetCompletedResponse r = JsonUtility.FromJson<GetCompletedResponse>(response);
        for (int i = 0; i < 32; i++)
        {
            GalleryManager.SetIsUnlocked(i,false);
        }
        foreach(GalleryModel birdol in r.birdols)
        {
            GalleryManager.SetIsUnlocked(birdol.id, true);
        }
        Manager.manager.StateQueue((int)gamestate.Gallery);

    }

    

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(" "));
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }
}
