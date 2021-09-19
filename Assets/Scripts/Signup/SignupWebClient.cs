using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SignupWebClient : WebClient
{
    [Header("SignUp Information")]
    [SerializeField] protected SignupRequestData signupRequestData;

    public bool isSignupSuccess { get; private set; } //ログインが成功したか否か。通信成功の後にチェックする。 

    /// <summary>
    /// Signup Request Data: send to Server
    /// </summary>
    [Serializable]
    public struct SignupRequestData
    {
        [SerializeField] public string name;
        [SerializeField] public string password;
        [SerializeField] public string device_id;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="name">user name</param>
        /// <param name="password">password</param>
        /// <param name="device_id">Unique ID to determine device</param>
        public SignupRequestData(string name, string password, string device_id)
        {
            this.name = name;
            this.password = password;
            this.device_id = device_id;
        }
    }

    /// <summary>
    /// Signup Response Data: receive from Server
    /// </summary>
    [Serializable]
    public struct SignupResponseData
    {
        [SerializeField] public string error;
        [SerializeField] public string result;
        [SerializeField] public uint user_id;
        [SerializeField] public string access_token;
        [SerializeField] public string account_id;
    }
    [Serializable]
    public class Auth
    {
        [SerializeField] public uint user_id;
        [SerializeField] public string access_token;
        [SerializeField] public string device_id;
    }

    /// <summary>
    /// Constructor:
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public SignupWebClient(HttpRequestMethod requestMethod, string loginPath) : base(requestMethod, loginPath)
    {
    }

    /// <summary>
    /// Constructor: 
    /// </summary>
    /// <param name="signupRequestData"></param>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public SignupWebClient(SignupRequestData signupRequestData, HttpRequestMethod requestMethod, string loginPath) : base(requestMethod, loginPath)
    {
        this.signupRequestData = signupRequestData;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="device_id"></param>
    /// <param name="requestMethod"></param>
    /// <param name="loginPath"></param>
    public SignupWebClient(string username, string password, string device_id, HttpRequestMethod requestMethod, string loginPath) : base(requestMethod, loginPath)
    {
        SetData(username, password, device_id);
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="device_id"></param>
    public void SetData(string username, string password, string device_id)
    {
        this.signupRequestData = new SignupRequestData(username, password, device_id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="srd"></param>
    /// <returns>true if response data is correctry parsed</returns>
    protected bool CheckResponseData(SignupResponseData srd)
    {
        bool ok = true;
        if (string.IsNullOrEmpty(srd.result)) ok = false;
        else if (srd.result == "success" && (string.IsNullOrEmpty(srd.access_token) || string.IsNullOrEmpty(srd.account_id))) ok = false;
        return ok;
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    public override bool CheckRequestData()
    {
        bool ok = true;
        if (this.signupRequestData.name.Length > ConnectionModel.USERNAME_LENGTH_MAX || this.signupRequestData.name.Length < ConnectionModel.USERNAME_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なユーザ名です。\n{ConnectionModel.USERNAME_LENGTH_MIN}文字から{ConnectionModel.USERNAME_LENGTH_MAX}文字で入力してください。";
        }
        /*
        else if (this.signupRequestData.account_id.Length > ConnectionModel.ACCOUNT_ID_LENGTH_MAX || this.signupRequestData.account_id.Length < ConnectionModel.ACCOUNT_ID_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なidです。\n{ConnectionModel.ACCOUNT_ID_LENGTH_MIN}文字から{ConnectionModel.ACCOUNT_ID_LENGTH_MAX}文字で入力してください。";
        }
        else if (this.signupRequestData.password.Length > ConnectionModel.PASSWORD_LENGTH_MAX || this.signupRequestData.password.Length < ConnectionModel.PASSWORD_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なパスワードです。\n{ConnectionModel.PASSWORD_LENGTH_MIN}文字から{ConnectionModel.PASSWORD_LENGTH_MAX}文字で入力してください。";
        }
        */

        return ok;
    }

    /// <summary>
    /// Setup Web Request Data 
    /// </summary>
    /// <returns></returns>
    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        isSignupSuccess = false;
        try
        {
            this.signupRequestData.password = Hash(this.signupRequestData.password);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            this.message = "このパスワードは使用できません。";
            throw;
        }
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.signupRequestData) + "}");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    }


    /// <summary>
    /// HandleSuccessData: 通信に成功した時にSignupクライアントが行う処理
    /// dataに値を保存 
    /// </summary>
    /// <param name="response">received data</param>
    /// <returns></returns>
    protected override void HandleSuccessData(string response)
    {
        this.data = JsonUtility.FromJson<SignupResponseData>(response);
        SignupResponseData srd = (SignupResponseData)this.data;
        if (CheckResponseData(srd) != true)
        {
            this.message = "サーバーから不適切な値が送信されました。";
            this.isSuccess = false;
        }
        else
        {
            if (srd.result == "success")
            {
                this.message = "アカウント新規登録に成功しました。";
                OnSignupSuccess(srd.user_id, srd.access_token, srd.account_id);
            }
            else
            {
                if (!string.IsNullOrEmpty(srd.error)) this.message = srd.error;
            }
        }
    }

    /// <summary>
    /// HandleErrorData: 通信に失敗した時にSignupクライアントが行う処理
    /// </summary>
    protected override void HandleErrorData(string error)
    {
        this.message = $"通信に失敗しました。";
        Debug.Log($"error: \n{error}");
    }

    /// <summary>
    /// HandleInProgressData: 通信に途中だった時にSignupクライアントが行う処理 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中です。";
        Debug.LogError("Unexpected UnityWebRequest Result");
    }


    /// <summary>
    /// Signup成功した時の動作。クライアント側としてデバイスへのデータ保存などを行う。
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="access_token"></param>
    /// <param name="account_id"></param>
    private void OnSignupSuccess(uint user_id, string access_token, string account_id)
    {
        isSignupSuccess = true;
        Debug.Log($"PlayerPrefs Saved\nuser_id: {user_id}, access_token: {access_token}, default_account_id: {account_id}");
        Common.UserID = user_id;
        Common.AccessToken = access_token;
        Common.DefaultAccountID = account_id;
    }
}
