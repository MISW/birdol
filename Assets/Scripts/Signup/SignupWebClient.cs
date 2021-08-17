using System;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SignupWebClient : WebClient
{
    [Header("SignUp Information")]
    [SerializeField] protected SignupRequestData signupRequestData;

    /// <summary>
    /// Signup Request Data: send to Server
    /// </summary>
    [Serializable]
    public struct SignupRequestData
    {
        [SerializeField] public string name;
        [SerializeField] public string email;
        [SerializeField] public string password;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public SignupRequestData(string name, string email, string password)
        {
            this.name = name;
            this.email = email;
            this.password = password;
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
        [SerializeField] public int user_id;
        [SerializeField] public string access_token;
    }
    [Serializable]
    public class Auth
    {
        [SerializeField] public string user_id;
        [SerializeField] public string access_token;
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
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="requestMethod"></param>
    /// <param name="loginPath"></param>
    public SignupWebClient(string username, string email, string password, HttpRequestMethod requestMethod, string loginPath) : base(requestMethod, loginPath)
    {
        SetData(username, email, password);
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    public void SetData(string username, string email, string password)
    {
        this.signupRequestData = new SignupRequestData(username, email, password);
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
        else if (srd.result == "success" && string.IsNullOrEmpty(srd.access_token)) ok = false;
        return ok;
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    protected override bool CheckRequestData()
    {
        bool ok = true;
        if (this.signupRequestData.name.Length > ConnectionModel.USERNAME_LENGTH_MAX || this.signupRequestData.name.Length < ConnectionModel.USERNAME_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なユーザ名です!\n{ConnectionModel.USERNAME_LENGTH_MIN}文字〜{ConnectionModel.USERNAME_LENGTH_MAX}文字で入力してください。";
        }
        else if (this.signupRequestData.email.Length > ConnectionModel.EMAIL_LENGTH_MAX || this.signupRequestData.email.Length < ConnectionModel.EMAIL_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なメールアドレスです!\n{ConnectionModel.EMAIL_LENGTH_MIN}文字〜{ConnectionModel.EMAIL_LENGTH_MAX}文字で入力してください。";
        }
        else if (this.signupRequestData.password.Length > ConnectionModel.PASSWORD_LENGTH_MAX || this.signupRequestData.password.Length < ConnectionModel.PASSWORD_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なパスワードです!\n{ConnectionModel.PASSWORD_LENGTH_MIN}文字〜{ConnectionModel.PASSWORD_LENGTH_MAX}文字で入力してください。";
        }
        else
        {
            try
            {
                new MailAddress(this.signupRequestData.email);
            }
            catch
            {
                ok = false;
                this.message = "不適切なメールアドレスです！\n間違っていないか確認してください。";
            }
        }

        return ok;
    }

    /// <summary>
    /// Setup Web Request Data 
    /// </summary>
    /// <returns></returns>
    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
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
                this.message = "新規登録成功!!";
                OnSignupSuccess(srd.user_id, srd.access_token);
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
        this.message = $"通信失敗！\n{error}";
        Debug.Log($"error: \n{error}");
    }

    /// <summary>
    /// HandleInProgressData: 通信に途中だった時にSignupクライアントが行う処理 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中...";
        Debug.LogError("Unexpected UnityWebRequest Result");
    }


    /// <summary>
    /// Signup成功した時の動作。クライアント側としてデバイスへのデータ保存などを行う。
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="access_token"></param>
    private void OnSignupSuccess(int user_id, string access_token)
    {
        Debug.Log($"user_id: {user_id}, access_token: {access_token}\n");
        PlayerPrefs.SetInt(ConnectionModel.PLAYERPREFS_USER_ID, user_id);
        PlayerPrefs.SetString(ConnectionModel.PLAYERPREFS_ACCESS_TOKEN, access_token);
    }
}
