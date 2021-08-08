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
        [SerializeField] public string username;
        [SerializeField] public string email;
        [SerializeField] public string password;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public LoginRequestData(string username,string email,string password)
        {
            this.username = username;
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
        [SerializeField] public string status;
        [SerializeField] public Session session;
    }
    [Serializable]
    public class Session
    {
        [SerializeField] public string user_id;
        [SerializeField] public string session_id;
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
    /// <returns></returns>
    protected bool CheckResponseData(LoginResponseData lrd)
    {
        return lrd.status != null;
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    protected override bool CheckRequestData()
    {
        bool ok = true;
        if (this.loginRequestData.username.Length > USERNAME_LENGTH_MAX || this.loginRequestData.username.Length < USERNAME_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なユーザ名です!\n{USERNAME_LENGTH_MIN}文字〜{USERNAME_LENGTH_MAX}文字で入力してください。";
        }else if (this.loginRequestData.email.Length > EMAIL_LENGTH_MAX || this.loginRequestData.email.Length<EMAIL_LENGTH_MIN)
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
        byte[] postData = System.Text.Encoding.UTF8.GetBytes("{\"user\":" + JsonUtility.ToJson(this.loginRequestData) + "}");
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
            Debug.Log(this.message);
        }
        else
        {
            //this.message = $"通信成功!\nData: {lrd.ToString()}";
            this.message = $"通信成功!\nData: status: {lrd.status}, session: {{user_id: {lrd.session.user_id}, session_id: {lrd.session.user_id}}} ";
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
}
