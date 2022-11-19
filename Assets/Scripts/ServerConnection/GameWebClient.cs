using System;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// トークンが期限切れの場合、自動的にリフレッシュトークンによるトークン更新を行う。
/// 認証用のヘッダを付加する。
/// </summary>
public abstract class GameWebClient : WebClient
{
    const string timeoutError = "Request timeout";

    protected bool TryRefreshToken = true; //トークンのリフレッシュ要求をするか否か: Trueの場合一度だけ要求を行う。
    protected bool IsAuthenticated = true; //認証用のヘッダをつけるか否か: Trueの場合ヘッダに認証用データを付加する。
    public bool IsDoCheckIfContinueOrQuitConnection = true; //これがtrueの場合、サーバとの通信失敗時に再送するか諦めるかを選択できるようにする
    public bool IsDoBackToTitleIfTokenRefreshError = true; //これがtrueの場合、通信成功だが認証失敗(AccessTokenもRefreshTokenも失敗)の時に、タイトルシーンへ遷移する
    protected ContinueConnectionChoice continueConnectionChoice = ContinueConnectionChoice.None;
    public bool IsExitOnFailed = false;
    public enum ContinueConnectionChoice
    {
        None,
        Continue,
        Quit
    }

    [Serializable]
    public struct Response
    {
        [SerializeField] public string result;
        [SerializeField] public string error;
    }

    public GameWebClient(HttpRequestMethod method, string path) : base(method, path)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    protected override IEnumerator HandleSuccessData(string response)
    {
        Response r = JsonUtility.FromJson<Response>(response);
#if UNITY_EDITOR
        Debug.Log($"TEST: {response}");
        Debug.Log($"TOKEN_RESULT: {r.result}");
#endif
        if (r.result == ConnectionModel.Response.ResultNeedTokenRefresh && TryRefreshToken == true)
        {
            //トークンのリフレッシュ要求を行う。
            RefreshTokenWebClient refreshTokenWebClient = new RefreshTokenWebClient(HttpRequestMethod.Get, $"/api/{Common.api_version}/refresh?refresh_token={Common.RefreshToken}");
            yield return GlobalCoroutine.StartCoroutineG(refreshTokenWebClient.Send());

            if (refreshTokenWebClient.IsRefreshSuccess && TryRefreshToken == true) //アクセストークンのリフレッシュ成功。中断されたデータを再送する。
            {
                TryRefreshToken = false;
                base.Refresh();
                base.Send();
                GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
                getCompletedWebClient.target = "home";
                yield return getCompletedWebClient.Send();
                GetStoryWebClient getStoryWebClient = new GetStoryWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/story?session_id=" + Common.SessionID);
                yield return getStoryWebClient.Send();

            }
            else //アクセストークンのリフレッシュ失敗。アカウント作成(orアカウント連携)が必要 
            {
                if (IsDoBackToTitleIfTokenRefreshError)
                {
                    //タイトルシーンへ遷移
#if UNITY_EDITOR
                    Debug.LogError("認証に失敗したため、タイトルシーンに遷移します。");
#endif
                    NetworkErrorDialogController.OpenConfirmDialog(() => {
                        Common.loadingCanvas.SetActive(true);
                        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
                        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
                        Common.bgmplayer.Stop();
                        Common.bgmplayer.time = 0;
                        Manager.manager.StateQueue((int)gamestate.Title);
                    }, "認証に失敗しました。\nタイトルに戻ります。");
                }
            }
            yield break;
        }
        HandleGameSuccessData(response);
    }

    protected abstract void HandleGameSuccessData(string response);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="www"></param>
    protected override void HandleSetupWebRequestData(UnityWebRequest www)
    {
        HandleGameSetupWebRequestData(www);
        SetAuthenticationHeader(www);
    }

    protected abstract void HandleGameSetupWebRequestData(UnityWebRequest www);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    protected override IEnumerator HandleErrorData(string error)
    {
        this.message = "通信に失敗しました。";
#if UNITY_EDITOR
        Debug.LogError(error);
#endif

        if (IsDoCheckIfContinueOrQuitConnection) //サーバとの通信ができなかった時(、かつその際に通信再送するか否かのチェックをする場合)
        {
            IsDoCheckIfContinueOrQuitConnection = false;

            while (!string.IsNullOrEmpty(base.error) ) //サーバと通信できないエラーが続いている間繰り返す
            {
                /*
                if (base.error == timeoutError) //タイムアウトエラー
                {
                */
                    NetworkErrorDialogController.OpenTimeoutDialog(this); //再送か否かの選択肢を表示する
                    yield return new WaitWhile(() => { return continueConnectionChoice == ContinueConnectionChoice.None; }); //タイムアウトを受けて、終了するか、の選択待ち

                    if (continueConnectionChoice == ContinueConnectionChoice.Continue) //通信再送
                    {
                        base.isInProgress = false;
                        yield return GlobalCoroutine.StartCoroutineG(this.Send()); //再送し終了待ち
                    }
                    else if (continueConnectionChoice == ContinueConnectionChoice.Quit)
                    {
                        base.error = null;
                        break;
                    }

                    continueConnectionChoice = ContinueConnectionChoice.None;
                    yield return new WaitForFixedUpdate();

                /*
                }
                else //その他のサーバと通信できないエラー。DNS解決できないなど。
                {
                    NetworkErrorDialogController.OpenConfirmDialog(() => {
                        Common.loadingCanvas.SetActive(true);
                        Manager.manager.StateQueue((int)gamestate.Title);
                    }, "通信に失敗しました。\nタイトルに戻ります。");
                    break;
                }
                */
            }
        }
        
        yield break;
    }


    /// <summary>
    /// 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中です...";
#if UNITY_EDITOR
        Debug.LogError("通信中です...");
#endif
    }

    /// <summary>
    /// After Set Data to Body 
    /// </summary>
    /// <param name="www"></param>
    /// <param name="accessToken">Request Body</param>
    /// <param name="privateKey"></param>
    public static void SetAuthenticationHeader(UnityWebRequest www, string accessToken, string uuid, string privateKey)
    {
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        string body = "";
        if (www.uploadHandler!=null) body = Encoding.UTF8.GetString(www.uploadHandler.data);
        string signature = CalcSignature(timeStamp, body, privateKey);
        www.SetRequestHeader("Authorization", $"Bearer {accessToken}");
        www.SetRequestHeader("X-Birdol-Signature", signature );
        www.SetRequestHeader("X-birdol-TimeStamp", timeStamp);
        www.SetRequestHeader("DeviceID", uuid);
#if UNITY_EDITOR
        Debug.Log($"AccessToken: {Common.AccessToken}, Signature: {signature}, TimeStamp: {timeStamp}, DeviceID: {Common.Uuid}");
#endif
    }
    public static void SetAuthenticationHeader(UnityWebRequest www)
    {
        SetAuthenticationHeader(www, Common.AccessToken, Common.Uuid, Common.RsaKeyPair.privateKey);
    }

    public void EnableExitOnFailure()
    {
        IsExitOnFailed = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="body"></param>
    /// <returns>Signature </returns>
    public static string CalcSignature(string timestamp, string body, string privateKey)
    {
        string signature;
        try
        {
            string signature_raw = $"{Common.api_version}:{timestamp}:{body}";
#if UNITY_EDITOR
            //Debug.Log($"Signature_raw: {signature_raw} ");
#endif
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.FromXmlString( Common.StrFromBase64Str(privateKey) );

            byte[] signature_b = csp.SignData(Encoding.UTF8.GetBytes(signature_raw), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            //signature = Convert.ToBase64String(signature_b);
            signature = BitConverter.ToString(signature_b).Replace("-", "").ToLower();
        }
        catch(Exception e)
        {
#if UNITY_EDITOR
            Debug.Log($"Signatureの作成に失敗しました。 {e}");
#endif
            return "";
        }
        

        return signature;
    }

    /// <summary>
    /// 通信タイムアウトしたのち、通信再送を試す選択肢を選んだ場合
    /// </summary>
    public void Choice_ContinueConnection()
    {
        this.continueConnectionChoice = ContinueConnectionChoice.Continue;
    }

    /// <summary>
    /// 通信タイムアウトしたのち、通信再送を諦める選択肢を選んだ場合
    /// </summary>
    public void Choice_QuitConnection()
    {
        this.continueConnectionChoice = ContinueConnectionChoice.Quit;
    }
}
