using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// アクセストークンを使ってログイン 
/// </summary>
public class TokenAuthorizeWebClient : GameWebClient
{
    public bool IsAuthorizeSuccess=false;

    [Serializable]
    public struct TokenAuthorizeResponse
    {
        [SerializeField] public string result; //"ok" or "failed"
        [SerializeField] public string error;
        [SerializeField] public string session_id;
    }


    public TokenAuthorizeWebClient(HttpRequestMethod method, string path):base(method,path)
    {

    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw( System.Text.Encoding.UTF8.GetBytes(" ") );
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }

    protected override void HandleGameSuccessData(string response)
    {
        TokenAuthorizeResponse r = JsonUtility.FromJson<TokenAuthorizeResponse>(response);
        base.data = r;
        if (r.result == "ok")
        {
            IsAuthorizeSuccess = true;
            Common.SessionID = r.session_id;
            this.message = "成功しました。";
            Debug.Log($"アクセストークンを用いてログインに成功しました。SessionID: {r.session_id}");
        }
        else
        {
            IsAuthorizeSuccess = false;
            this.message = r.error;
        }

    }
}
