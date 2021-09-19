using System;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccountEditWebClient : WebClient
{
    [Header("Request Data Information")]
    [SerializeField] protected AccountEditRequestData accountEditRequestData;

    public bool isEditSuccess { get; private set; } = false;

    private void Start()
    {
        base.path = $"/api/v1/user/account/{Common.UserID}";
    }

    /// <summary>
    /// Request Data: send to Server
    /// </summary>
    [Serializable]
    public struct AccountEditRequestData
    {
        [SerializeField] public string account_id;
        [SerializeField] public string password;
        [SerializeField] public string device_id;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="account_id"></param>
        /// <param name="password"></param>
        /// <param name="device_id"></param>
        public AccountEditRequestData(string account_id, string password, string device_id)
        {
            this.account_id = account_id;
            this.password = password;
            this.device_id = device_id;
        }
    }
    [Serializable]
    public struct Auth
    {
        [SerializeField] public uint user_id;
        [SerializeField] public string session_id;
        [SerializeField] public string device_id;
    }

    /// <summary>
    /// Account Edit Response Data: receive from Server
    /// </summary>
    [Serializable]
    public struct AccountEditResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
    }

    /// <summary>
    /// Constructor:  
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public AccountEditWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    /// <summary>
    /// Constructor: 
    /// </summary>
    /// <param name="aerd"></param>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public AccountEditWebClient(AccountEditRequestData aerd, HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
        this.accountEditRequestData = aerd;
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="account_id"></param>
    /// <param name="password"></param>
    /// <param name="device_id"></param>
    public void SetData(string account_id, string password, string device_id)
    {
        this.accountEditRequestData = new AccountEditRequestData(account_id, password, device_id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aerd"></param>
    /// <returns>true if response data is correctry parsed</returns>
    protected bool CheckResponseData(AccountEditResponseData aerd)
    {
        return ( !string.IsNullOrEmpty(aerd.result) || !string.IsNullOrEmpty(aerd.error) );
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    public override bool CheckRequestData()
    {
        bool ok = true;
        if (this.accountEditRequestData.account_id.Length > ConnectionModel.ACCOUNT_ID_LENGTH_MAX || this.accountEditRequestData.account_id.Length < ConnectionModel.ACCOUNT_ID_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なidです。\n{ConnectionModel.ACCOUNT_ID_LENGTH_MIN}文字から{ConnectionModel.ACCOUNT_ID_LENGTH_MAX}文字で入力してください。";
        }
        else if (this.accountEditRequestData.password.Length > ConnectionModel.PASSWORD_LENGTH_MAX || this.accountEditRequestData.password.Length < ConnectionModel.PASSWORD_LENGTH_MIN)
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
    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        isEditSuccess = false;
        try
        {
            this.accountEditRequestData.password = Hash(this.accountEditRequestData.password);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            this.message = "このパスワードは使用できません。";
            throw;
        }
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.accountEditRequestData) + "}");
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
    protected override void HandleSuccessData(string response)
    {
        this.data = JsonUtility.FromJson<AccountEditResponseData>(response);
        AccountEditResponseData aerd = (AccountEditResponseData)this.data;
        if (CheckResponseData(aerd) != true)
        {
            this.message = "サーバーから不適切な値が送信されました。";
            this.isSuccess = false;
        }
        else
        {
            if (aerd.result == "success")
            {
                this.message = "成功しました。";
            }
            else
            {
                if (!string.IsNullOrEmpty(aerd.error)) this.message = aerd.error;
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
    private void OnAccountEditSuccess()
    {
        isEditSuccess = true;
    }
}
