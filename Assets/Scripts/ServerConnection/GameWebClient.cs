using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public abstract class GameWebClient : WebClient
{
    protected bool TryRefreshToken = true; //トークンのリフレッシュ要求をするか否か: Trueの場合一度だけ要求を行う。
    protected bool IsAuthenticated = true; //認証用のヘッダをつけるか否か: Trueの場合ヘッダに認証用データを付加する。 

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
    protected override void HandleSuccessData(string response)
    {
        Response r = JsonUtility.FromJson<Response>(response);
        if(r.result=="failed" && r.error == "invalid_token" && TryRefreshToken==true)
        {
            //トークンのリフレッシュ要求を行う。
            RefreshTokenWebClient refreshTokenWebClient = new RefreshTokenWebClient(HttpRequestMethod.Get, $"/api/{Common.api_version}/auth/refresh?refresh_token={Common.RefreshToken}");
            refreshTokenWebClient.Send();
            
            if (refreshTokenWebClient.IsRefreshSuccess) //アクセストークンのリフレッシュ成功。中断されたデータを再送する。
            {
                TryRefreshToken=false;
                base.Refresh();
                base.Send();
            }
            else //アクセストークンのリフレッシュ失敗。ログイン(orアカウント作成)が必要 
            {
                Debug.Log("アクセストークンのリフレッシュに失敗したため、ログイン(orアカウント作成)が必要。Signupシーンへ飛ばすと良さそう。");
            }
            return;
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
    protected override void HandleErrorData(string error)
    {
        this.message = "通信に失敗しました。";
        Debug.LogError(error);
    }


    /// <summary>
    /// 
    /// </summary>
    protected override void HandleInProgressData()
    {
        this.message = "通信中です...";
        Debug.LogError("通信中です...");
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
        Debug.Log($"TIMESTAMP: {timeStamp}");

        string body = Encoding.UTF8.GetString(www.uploadHandler.data);
        string signature = CalcSignature(timeStamp, body, privateKey);
        www.SetRequestHeader("Authorization", $"Bearer {accessToken}");
        www.SetRequestHeader("X-Birdol-Signature", signature );
        www.SetRequestHeader("X-birdol-TimeStamp", timeStamp);
        www.SetRequestHeader("device_id", uuid);

        Debug.Log($"AccessToken: {Common.AccessToken}, Signature: {signature}, TimeStamp: {timeStamp}, DeviceID: {Common.Uuid}");
    }
    public static void SetAuthenticationHeader(UnityWebRequest www)
    {
        SetAuthenticationHeader(www, Common.AccessToken, Common.Uuid, Common.RsaKeyPair.privateKey);
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
            Debug.Log($"Signature_raw: {signature_raw} ");

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.FromXmlString( Common.StrFromBase64Str(privateKey) );

            byte[] signature_b = csp.SignData(Encoding.UTF8.GetBytes(signature_raw), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            //signature = Convert.ToBase64String(signature_b);
            signature = BitConverter.ToString(signature_b).Replace("-", "").ToLower();
        }
        catch(Exception e)
        {
            Debug.Log($"Signatureの作成に失敗しました。 {e}");
            return "";
        }
        

        return signature;
    }

   
}
