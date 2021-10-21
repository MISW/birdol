using System;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// アカウントの連携を行う。ユーザIDとパスワードを必要する。
/// </summary>
public class LinkAccountWebClient: WebClient 
{
    [Header("LinkAccount Information")]
    [SerializeField] protected LinkAccountRequestData linkAccountRequestData;

    public bool isLinkAccountSuccess { get; private set; } //アカウントのデータ連携が成功したか否か。通信成功の後にチェックする。 
    public string privateKey;

    /// <summary>
    /// LinkAccount Request Data: send to Server
    /// </summary>
    [Serializable]
    public struct LinkAccountRequestData
    {
        [SerializeField] public string account_id;
        [SerializeField] public string password;
        [SerializeField] public string device_id;
        [SerializeField] public string public_key;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="account_id"></param>
        /// <param name="password"></param>
        /// <param name="device_id"></param>
        /// <param name="public_key"></param>
        public LinkAccountRequestData(string account_id, string password, string device_id, string public_key)
        {
            this.account_id = account_id;
            this.password = password;
            this.device_id = device_id;
            this.public_key = public_key;
        }
    }

    /// <summary>
    /// LinkAccount Response Data: receive from Server
    /// </summary>
    [Serializable]
    public struct LinkAccountResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
        [SerializeField] public uint user_id;
        [SerializeField] public string access_token;
        [SerializeField] public string refresh_token;
    }

    /// <summary>
    /// Constructor:  
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public LinkAccountWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <param name="device_id"></param>
    /// <param name="publicKey"></param>
    /// <param name="privateKey"></param>
    public void SetData(string id, string password, string device_id, string publicKey, string privateKey)
    {
        this.linkAccountRequestData = new LinkAccountRequestData(id, password, device_id, publicKey);
        this.privateKey = privateKey;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lrd"></param>
    /// <returns>true if response data is correctry parsed</returns>
    protected bool CheckResponseData(LinkAccountResponseData lrd)
    {
        return !string.IsNullOrEmpty(lrd.result) || !string.IsNullOrEmpty(lrd.error) || !string.IsNullOrEmpty(lrd.access_token) || !string.IsNullOrEmpty(lrd.refresh_token);
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    public override bool CheckRequestData()
    {
        bool ok = true;
        if (this.linkAccountRequestData.account_id.Length > ConnectionModel.ACCOUNT_ID_LENGTH_MAX || this.linkAccountRequestData.account_id.Length< ConnectionModel.ACCOUNT_ID_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なidです。\n{ConnectionModel.ACCOUNT_ID_LENGTH_MIN}文字から{ConnectionModel.ACCOUNT_ID_LENGTH_MAX}文字で入力してください。";
        }else if (this.linkAccountRequestData.password.Length > ConnectionModel.PASSWORD_LENGTH_MAX || this.linkAccountRequestData.password.Length< ConnectionModel.PASSWORD_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なパスワードです。\n{ConnectionModel.PASSWORD_LENGTH_MIN}文字から{ConnectionModel.PASSWORD_LENGTH_MAX}文字で入力してください。";
        }else if (string.IsNullOrEmpty(this.privateKey))
        {
            ok = false;
            this.message = "エラー";
            Debug.LogError("privateKeyがセットされていません。");
        }
        return ok;
    }

    /// <summary>
    /// Setup Web Request Data 
    /// </summary>
    /// <returns></returns>
    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        isLinkAccountSuccess = false;
        try
        {
            this.linkAccountRequestData.password = Hash512(this.linkAccountRequestData.password);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            this.message = "このパスワードは使用できません。";
            throw;
        }
        byte[] postData = System.Text.Encoding.UTF8.GetBytes( JsonUtility.ToJson(this.linkAccountRequestData) );
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        GameWebClient.SetAuthenticationHeader(www, Common.AccessToken, Common.Uuid, privateKey);
    }


    /// <summary>
    /// HandleSuccessData: 通信に成功した時にクライアントが行う処理
    /// dataに値を保存 
    /// </summary>
    /// <param name="response">received data</param>
    /// <returns></returns>
    protected override IEnumerator HandleSuccessData(string response)
    {
        this.data = JsonUtility.FromJson<LinkAccountResponseData>(response);
        LinkAccountResponseData lrd = (LinkAccountResponseData)this.data;
        if (CheckResponseData(lrd)!=true)
        {
            this.message = "サーバーから不適切な値が送信されました。";
            this.isSuccess = false;
        }
        else
        {
            if (lrd.result == ConnectionModel.Response.ResultOK)
            {
                this.message = "アカウント連携に成功しました。";
                OnLinkAccountSuccess(lrd);
            }
            else
            {
                this.message = ConnectionModel.ErrorMessage(lrd.error);
            }
        }

        yield break;
    }

    /// <summary>
    /// HandleErrorData: 通信に失敗した時にクライアントが行う処理
    /// </summary>
    protected override IEnumerator HandleErrorData(string error)
    {
        this.message = $"通信に失敗しました。";
        Debug.Log($"error: \n{error}");
        yield break;
    }

    /// <summary>
    /// HandleInProgressData: 通信の途中だった時にクライアントが行う処理 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中です。"; 
        Debug.LogError("Unexpected UnityWebRequest Result");
    }


    /// <summary>
    /// アカウント連携に成功した時の動作。クライアント側としてデバイスへのデータ保存などを行う。
    /// </summary>
    /// <param name="lrd">LinkAccount Response Data</param>
    private void OnLinkAccountSuccess(LinkAccountResponseData lrd)
    {
        isLinkAccountSuccess = true;
        Common.UserID = lrd.user_id;
        Common.AccessToken = lrd.access_token;
        Common.RefreshToken = lrd.refresh_token;
    }
}
