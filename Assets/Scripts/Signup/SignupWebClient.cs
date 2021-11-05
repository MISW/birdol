using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SignupWebClient : WebClient
{
    private string privateKey;

    [Header("SignUp Information")]
    [SerializeField] protected SignupRequestData signupRequestData;

    public bool isSignupSuccess { get; private set; } //アカウント作成が成功したか否か。通信成功の後にチェックする。 

    /// <summary>
    /// Signup Request Data: send to Server
    /// </summary>
    [Serializable]
    public struct SignupRequestData
    {
        [SerializeField] public string name;
        [SerializeField] public string public_key;
        [SerializeField] public string device_id;
        [SerializeField] public DendouModel[] completed_progresses;
        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="name">user name</param>
        /// <param name="public_key">public_key</param>
        /// <param name="device_id">Unique ID to determine device</param>
        public SignupRequestData(string name, string public_key, string device_id)
        {
            this.name = name;
            this.public_key = public_key;
            this.device_id = device_id;
            completed_progresses = new DendouModel[4];
            for (int i = 0; i < 4; i++)
            {
                DendouModel dendouModel = new DendouModel();
                dendouModel.MainCharacterId = i;
                dendouModel.SupportCharacterId = i;
                dendouModel.Name = Common.characters[i].name;
                dendouModel.Vocal = Common.characters[i].vocal;
                dendouModel.Visual = Common.characters[i].visual;
                dendouModel.Dance = Common.characters[i].dance;
                completed_progresses[i] = dendouModel;
            }
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
        [SerializeField] public string refresh_token;
        [SerializeField] public string account_id;
        [SerializeField] public string session_id;
    }

    /// <summary>
    /// Constructor:
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="path">default "/"</param>
    public SignupWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    /// <summary>
    /// Setdata 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="public_key"></param>
    /// <param name="device_id"></param>
    /// <param name="private_key"></param>
    public void SetData(string username, string public_key, string device_id, string private_key)
    {
        this.signupRequestData = new SignupRequestData(username, public_key, device_id );
        this.privateKey = private_key;
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
        else if (srd.result == ConnectionModel.Response.ResultOK && (string.IsNullOrEmpty(srd.access_token) || string.IsNullOrEmpty(srd.account_id) || string.IsNullOrEmpty(srd.refresh_token))) ok = false;
        return ok;
    }

    /// <summary>
    /// </summary>
    /// <returns>if request data is appropriate or not</returns>
    public override bool CheckRequestData()
    {
        bool ok = true;

        int nameLength = ConnectionModel.CountHalfWidthCharLength(this.signupRequestData.name);
        if (nameLength > ConnectionModel.USERNAME_LENGTH_MAX || nameLength < ConnectionModel.USERNAME_LENGTH_MIN)
        {
            ok = false;
            this.message = $"不適切なユーザ名です。\n{ConnectionModel.USERNAME_LENGTH_MIN}文字から{ConnectionModel.USERNAME_LENGTH_MAX}文字で入力してください。なお、全角文字は2文字分として数えられます。\n現在文字数; <color=\"red\">{nameLength}</color>";
        }
        else if (string.IsNullOrEmpty(this.privateKey))
        {
            ok = false;
            this.message = "エラー";
#if UNITY_EDITOR
            Debug.LogError("privateKeyがセットされていません。");
#endif
        }

        return ok;
    }

    /// <summary>
    /// Setup Web Request Data 
    /// </summary>
    /// <returns></returns>
    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        isSignupSuccess = false;

        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(this.signupRequestData) );
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        GameWebClient.SetAuthenticationHeader(www, Common.AccessToken, Common.Uuid, privateKey);
    }


    /// <summary>
    /// HandleSuccessData: 通信に成功した時にSignupクライアントが行う処理
    /// dataに値を保存 
    /// </summary>
    /// <param name="response">received data</param>
    /// <returns></returns>
    protected override IEnumerator HandleSuccessData(string response)
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
            if (srd.result == ConnectionModel.Response.ResultOK)
            {
                this.message = "アカウント新規登録に成功しました。";
                OnSignupSuccess(srd);
            }
            else
            {
                this.message = ConnectionModel.ErrorMessage(srd.error);
            }
        }
        yield break;
    }

    /// <summary>
    /// HandleErrorData: 通信に失敗した時にSignupクライアントが行う処理
    /// </summary>
    protected override IEnumerator HandleErrorData(string error)
    {
        this.message = $"通信に失敗しました。";
#if UNITY_EDITOR
        Debug.Log($"error: \n{error}");
#endif
        yield break;
    }

    /// <summary>
    /// HandleInProgressData: 通信に途中だった時にSignupクライアントが行う処理 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中です。";
#if UNITY_EDITOR
        Debug.LogError("Unexpected UnityWebRequest Result");
#endif
    }


    /// <summary>
    /// Signup成功した時の動作。クライアント側としてデバイスへのデータ保存などを行う。
    /// </summary>
    /// <param name="srd">Signup Response Data</param>
    private void OnSignupSuccess(SignupResponseData srd)
    {
        isSignupSuccess = true;
#if UNITY_EDITOR
        Debug.Log($"PlayerPrefs Saved\nuser_id: {srd.user_id}, access_token: {srd.access_token}, refresh_token: {srd.refresh_token}, default_account_id: {srd.account_id}");
#endif
        Common.UserID = srd.user_id;
        Common.AccessToken = srd.access_token;
        Common.RefreshToken = srd.refresh_token;
        Common.DefaultAccountID = srd.account_id;
        Common.PlayerName = signupRequestData.name;
    }
}
