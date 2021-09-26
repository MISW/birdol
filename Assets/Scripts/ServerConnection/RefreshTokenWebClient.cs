using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RefreshTokenWebClient : WebClient
{
    public bool IsRefreshSuccess = false;

    [Serializable]
    public struct RefreshTokenResponse
    {
        [SerializeField] public string result;
        [SerializeField] public string error;
        [SerializeField] public string token;
        [SerializeField] public string refresh_token;
        [SerializeField] public string session_id;
    }

    public RefreshTokenWebClient(HttpRequestMethod method, string url) : base(method, url)
    {

    }

    protected override void HandleSuccessData(string response)
    {
        RefreshTokenResponse r = JsonUtility.FromJson<RefreshTokenResponse>(response);
        base.data = r;
        if (r.result == "ok")
        {
            IsRefreshSuccess = true;
            Common.AccessToken = r.token;
            Common.RefreshToken = r.refresh_token;
            Common.SessionID = r.session_id;
            this.message = "成功しました。";
            Debug.Log($"RefreshTokenに成功しました。 AccessToken: {r.token}, RefreshToken: {r.refresh_token}, SessionID: {r.session_id}");
        }
        else
        {
            Debug.Log($"アクセストークンのリフレッシュに失敗したため、アカウント作成(orアカウント連携)が必要です。 {r.error}");
            this.message = r.error;
        }
    }

    protected override void HandleErrorData(string error)
    {
        this.message = "失敗しました。";
        Debug.LogError(error);
    }

    protected override void HandleInProgressData()
    {
        this.message = "通信中です";
        Debug.LogError("Connection in progress...");
    }

    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(new byte[] { });
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        GameWebClient.SetAuthenticationHeader(www);
    }

    public override bool CheckRequestData()
    {
        return true;
    }
}
