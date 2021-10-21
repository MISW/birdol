using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// リフレッシュトークンを用いて期限切れのアクセストークンを再取得する。
/// </summary>
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

    protected override IEnumerator HandleSuccessData(string response)
    {
        RefreshTokenResponse r = JsonUtility.FromJson<RefreshTokenResponse>(response);
        base.data = r;
        if (r.result == ConnectionModel.Response.ResultRefreshSuccess)
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
            this.message = ConnectionModel.ErrorMessage(r.error);
            Debug.Log($"アクセストークンのリフレッシュに失敗したため、アカウント作成(orアカウント連携)が必要です。 {r.error}");
        }
        yield break;
    }

    protected override IEnumerator HandleErrorData(string error)
    {
        this.message = "失敗しました。";
        Debug.LogError(error);
        yield break;
    }

    protected override void HandleInProgressData()
    {
        this.message = "通信中です";
        Debug.LogError("Connection in progress...");
    }

    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        //www.uploadHandler = (UploadHandler)new UploadHandlerRaw(new byte[] { });
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        GameWebClient.SetAuthenticationHeader(www);
    }

    public override bool CheckRequestData()
    {
        return true;
    }
}
