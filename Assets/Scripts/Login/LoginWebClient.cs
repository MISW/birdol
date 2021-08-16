using System;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoginWebClient: WebClient 
{
    const int USERNAME_LENGTH_MIN = 1;
    const int USERNAME_LENGTH_MAX = 20;
    const int EMAIL_LENGTH_MIN = 1;
    const int EMAIL_LENGTH_MAX = 300;
    const int PASSWORD_LENGTH_MIN = 4;
    const int PASSWORD_LENGTH_MAX = 300;

    [Header("Login Information")]
    [SerializeField] protected LoginRequestData loginRequestData;

    /// <summary>
    /// Login Request Data: send to Server
    /// </summary>
    [Serializable]
    public struct LoginRequestData
    {
        [SerializeField] public string email;
        [SerializeField] public string password;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public LoginRequestData(string username,string email,string password)
        {
            this.email = email;
            this.password = password;
        }
    }

    /// <summary>
    /// Login Response Data: receive from Server
    /// </summary>
    [Serializable]
    public struct LoginResponseData
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
    /// Constructor: requestMethod to $"(hostname}:{port}{loginPath}" with loginRequestData 
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="path">default "/"</param>
    public LoginWebClient(ProtocolType protocol,HttpRequestMethod requestMethod, string hostname, string port, string loginPath) : base(protocol, requestMethod, hostname, port, loginPath)
    {
    }

    /// <summary>
    /// Constructor: requestMethod to $"(hostname}:{port}{loginPath}" with loginRequestData 
    /// </summary>
    /// <param name="loginRequestData"></param>
    /// <param name="requestMethod"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="path">default "/"</param>
    public LoginWebClient(LoginRequestData loginRequestData, ProtocolType protocol, HttpRequestMethod requestMethod, string hostname, string port, string loginPath): base(protocol,requestMethod,hostname, port,loginPath)
    {
        this.loginRequestData = loginRequestData;
    }

    /// <summary>
    /// Constructor: requestMethod to $"(hostname}:{port}{loginPath}" with loginRequestData 
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="path">default "/"</param>
    public LoginWebClient(string username, string email, string password,ProtocolType protocol, HttpRequestMethod requestMethod, string hostname, string port, string loginPath) : base(protocol,requestMethod, hostname, port, loginPath)
    {
        SetData(username,email,password);
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    public void SetData(string username, string email, string password)
    {
        this.loginRequestData = new LoginRequestData(username, email, password);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lrd"></param>
    /// <returns>true if response data is correctry parsed</returns>
    protected bool CheckResponseData(LoginResponseData lrd)
    {
        return !(string.IsNullOrEmpty(lrd.result) || string.IsNullOrEmpty(lrd.error) ) || !string.IsNullOrEmpty(lrd.access_token);
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    protected override bool CheckRequestData()
    {
        bool ok = true;
        if (this.loginRequestData.email.Length > EMAIL_LENGTH_MAX || this.loginRequestData.email.Length<EMAIL_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なメールアドレスです!\n{EMAIL_LENGTH_MIN}文字〜{EMAIL_LENGTH_MAX}文字で入力してください。";
        }else if (this.loginRequestData.password.Length > PASSWORD_LENGTH_MAX || this.loginRequestData.password.Length<PASSWORD_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なパスワードです!\n{PASSWORD_LENGTH_MIN}文字〜{PASSWORD_LENGTH_MAX}文字で入力してください。";
        }
        else
        {
            try
            {
                new MailAddress(this.loginRequestData.email);
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
        byte[] postData = System.Text.Encoding.UTF8.GetBytes( JsonUtility.ToJson(this.loginRequestData) + "}");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
    }


    /// <summary>
    /// HandleSuccessData: 通信に成功した時にLoginクライアントが行う処理
    /// dataに値を保存 
    /// </summary>
    /// <param name="response">received data</param>
    /// <returns></returns>
    protected override void HandleSuccessData(string response)
    {
        Debug.Log("Receive data: " +response);
        this.data = JsonUtility.FromJson<LoginResponseData>(response);
        LoginResponseData lrd = (LoginResponseData)this.data;
        if (CheckResponseData(lrd)!=true)
        {
            this.message = "Failed to parse response data. ";
            this.isSuccess = false;
        }
        else
        {
            if (lrd.result == "success")
            {
                this.message = "ログイン成功!!";
                OnLoginSuccess(lrd.user_id,lrd.access_token);
            }
            else
            {
                if (!string.IsNullOrEmpty(lrd.error)) this.message = lrd.error;
            }
        }
    }

    /// <summary>
    /// HandleErrorData: 通信に失敗した時にLoginクライアントが行う処理
    /// </summary>
    protected override void HandleErrorData(string error)
    {
        this.message = $"通信失敗！\n{error}";
        Debug.Log($"error: \n{error}");
    }

    /// <summary>
    /// HandleInProgressData: 通信に途中だった時にLoginクライアントが行う処理 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中..."; 
        Debug.LogError("Unexpected UnityWebRequest Result");
    }


    /// <summary>
    /// ログイン成功した時の動作。クライアント側としてデバイスへのデータ保存などを行う。
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="access_token"></param>
    private void OnLoginSuccess(int user_id, string access_token)
    {
        Debug.Log($"user_id: {user_id}, access_token: {access_token}\n<color=\"red\">TO DO: デバイスに保存する。</color>");
    }
}
