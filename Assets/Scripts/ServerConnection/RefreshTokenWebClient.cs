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
        if (r.result == "success")
        {
            IsRefreshSuccess = true;
            Common.AccessToken = r.token;
            Common.RefreshToken = r.refresh_token;
            Common.SessionID = r.session_id;
            this.message = "成功";
            Debug.Log($"RefreshTokenに成功しました。 AccessToken: {r.token}, RefreshToken: {r.refresh_token}, SessionID: {r.session_id}");
        }
        else
        {
            Common.AccessToken = null;
            Common.RefreshToken = null;
            Debug.LogError($"Failed to refresh Access Token, {r.error}");
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
        GameWebClient.SetAuthenticationHeader(www);
    }

    public override bool CheckRequestData()
    {
        throw new System.NotImplementedException();
    }
}
