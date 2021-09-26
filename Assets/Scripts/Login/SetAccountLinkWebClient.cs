using System;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// アカウントのデータ連携を行うためにパスワードを設定する。アカウント連携が完了すると、パスワードの再設定が必要となる。
/// </summary>
public class SetAccountLinkWebClient : GameWebClient
{
    [Header("Request Data Information")]
    [SerializeField] protected SetAccountLinkRequestData setAccountLinkRequestData;

    public bool isSetAccountLinkSuccess { get; private set; } = false;


    /// <summary>
    /// Request Data: send to Server, アカウント連携(パスワード設定) 
    /// </summary>
    [Serializable]
    public struct SetAccountLinkRequestData
    {
        [SerializeField] public string password;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="password"></param>
        public SetAccountLinkRequestData(string password)
        {
            this.password = password;
        }
    }

    /// <summary>
    /// Account Edit Response Data: receive from Server
    /// </summary>
    [Serializable]
    public struct SetAccountLinkResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
    }

    /// <summary>
    /// Constructor:  
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public SetAccountLinkWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    /// <summary>
    /// Constructor: 
    /// </summary>
    /// <param name="aerd"></param>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public SetAccountLinkWebClient(SetAccountLinkRequestData aerd, HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
        this.setAccountLinkRequestData = aerd;
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="password"></param>
    public void SetData(string password)
    {
        this.setAccountLinkRequestData = new SetAccountLinkRequestData(password);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aerd"></param>
    /// <returns>true if response data is correctry parsed</returns>
    protected bool CheckResponseData(SetAccountLinkResponseData aerd)
    {
        return ( !string.IsNullOrEmpty(aerd.result) || !string.IsNullOrEmpty(aerd.error) );
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    public override bool CheckRequestData()
    {
        bool ok = true;
        if (this.setAccountLinkRequestData.password.Length > ConnectionModel.PASSWORD_LENGTH_MAX || this.setAccountLinkRequestData.password.Length < ConnectionModel.PASSWORD_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なパスワードです。\n{ConnectionModel.PASSWORD_LENGTH_MIN}文字から{ConnectionModel.PASSWORD_LENGTH_MAX}文字で入力してください。";
        }
        return ok;
    }

    /// <summary>
    /// Setup Web Request Data 
    /// </summary>
    /// <returns></returns>
    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        isSetAccountLinkSuccess = false;
        try
        {
            this.setAccountLinkRequestData.password = Hash512(this.setAccountLinkRequestData.password);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            this.message = "このパスワードは使用できません。";
            throw;
        }
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.setAccountLinkRequestData));
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    }


    /// <summary>
    /// HandleSuccessData: 通信に成功した時にクライアントが行う処理
    /// dataに値を保存 
    /// </summary>
    /// <param name="response">received data</param>
    /// <returns></returns>
    protected override void HandleGameSuccessData(string response)
    {
        this.data = JsonUtility.FromJson<SetAccountLinkResponseData>(response);
        SetAccountLinkResponseData aerd = (SetAccountLinkResponseData)this.data;
        if (CheckResponseData(aerd) != true)
        {
            this.message = "サーバーから不適切な値が送信されました。";
            this.isSuccess = false;
        }
        else
        {
            if (aerd.result == ConnectionModel.Response.ResultOK)
            {
                this.message = "成功しました。";
            }
            else
            {
                this.message = ConnectionModel.ErrorMessage(aerd.error);
            }
        }
    }

    /// <summary>
    /// HandleErrorData: 通信に失敗した時にクライアントが行う処理
    /// </summary>
    protected override void HandleErrorData(string error)
    {
        this.message = $"通信に失敗しました。";
        Debug.Log($"error: \n{error}");
    }

    /// <summary>
    /// HandleInProgressData: 通信に途中だった時にクライアントが行う処理 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中です。";
        Debug.LogError("Unexpected UnityWebRequest Result");
    }


    /// <summary>
    /// 成功した時の動作。クライアント側としてデバイスへのデータ保存などを行う。
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="access_token"></param>
    private void OnSetAccountLinkSuccess()
    {
        isSetAccountLinkSuccess = true;
    }
}
