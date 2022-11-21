using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アカウント新規作成、またはアカウント連携(ログイン)を行う。アカウント連携用パスワード設定は行えない。
/// アクセストークンの認証(セッション更新)は既にしてあり、認証失敗したということを前提に考えている。
/// </summary>
public class SignupSceneController : SceneVisor
{
    [Header("SignUp Web Client")]
    private SignupWebClient signupWebClient;

    [Header("Input")]
    [SerializeField] InputField usernameInputField;
    [SerializeField] Button signupButton;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text AlertText;

    private bool isConnectionInProgress = false;


    private void Start()
    {
        this.signupWebClient = new SignupWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/user");

        SetUpButtonEvent();
    }

    private void SetUpButtonEvent()
    {
        //Signup
        signupButton.onClick.AddListener(() => {
            OnSignupButtonClicked();
        });
        //username 中の文字としてふさわしくなさそうなものを削除する。
        usernameInputField.onEndEdit.AddListener((s) =>
        {
            usernameInputField.text = System.Text.RegularExpressions.Regex.Replace(usernameInputField.text, @"\n|\r|\s|\t|\v", string.Empty);
        });
    }

    private void OnSignupButtonClicked()
    {
        if (isConnectionInProgress) return;
        StartCoroutine(Signup());
    }

    /// <summary>
    /// Signup Request
    /// </summary>
    /// <returns></returns>
    private IEnumerator Signup()
    {
        AlertUI.SetActive(true);
        AlertText.text = "通信中...";
        isConnectionInProgress = true;
        string username = usernameInputField.text;
        (string privateKey, string publicKey) rsaKeyPair = Common.CreateRsaKeyPair();
        string _uuid = GenerateGUID();
        signupWebClient.SetData(username, rsaKeyPair.publicKey, _uuid, rsaKeyPair.privateKey);

        //データチェックをサーバへ送信する前に行う。
        if (signupWebClient.CheckRequestData()==false)
        {
            AlertText.text = signupWebClient.message;
#if UNITY_EDITOR
            Debug.Log(signupWebClient.message);
#endif
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            isConnectionInProgress = false;
            yield break;
        }

        float conn_start = Time.time;
        yield return StartCoroutine(signupWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        AlertUI.SetActive(false);

        //処理
        if (signupWebClient.isSuccess == true && signupWebClient.isInProgress == false)
        {
            //成功した時
            SignupWebClient.SignupResponseData srd = (SignupWebClient.SignupResponseData)signupWebClient.data;
#if UNITY_EDITOR
            Debug.Log("ParsedResponseData: \n" + srd.ToString());
#endif
            if (signupWebClient.isSignupSuccess)
            {
                Common.Uuid = _uuid;
                Common.RsaKeyPair = rsaKeyPair;
                Common.SavedKeyType = Common.KEY_RSA4096; // set keytype
#if UNITY_EDITOR
                Debug.Log($"Playerprefs Saved.\nUUID: {_uuid}");
#endif

                AlertText.text = signupWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
                OnSignupSuccess();
            }
            else
            {
                AlertText.text = signupWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            }
        }
        else
        {
            //失敗した時
            AlertText.text = $"<color=\"red\">{signupWebClient.message}</color>";
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
        }

        isConnectionInProgress = false;
        yield break;
    }

    //Show (GameObject)gm for (float)duration seconds
    private IEnumerator ShowForWhileCoroutine(float duration, GameObject gm)
    {
        gm.SetActive(true);
        yield return new WaitForSeconds(duration);
        gm.SetActive(false);
        yield break;
    }

    IEnumerator Login()
    {
        TokenAuthorizeWebClient tokenAuthorizeWebClient = new TokenAuthorizeWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/auth");
        yield return tokenAuthorizeWebClient.Send();
        GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
        getCompletedWebClient.target = "home";
        yield return getCompletedWebClient.Send();
        GetStoryWebClient getStoryWebClient = new GetStoryWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/story?session_id=" + Common.SessionID);
        yield return getStoryWebClient.Send();

    }

    /// <summary>
    /// アカウント登録に成功したときの動作。
    /// </summary>
    private void OnSignupSuccess()
    {
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        TokenAuthorizeWebClient webClient = new TokenAuthorizeWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/auth");
        StartCoroutine(Login());
    }

    /// <summary>
    /// Generate UUID
    /// </summary>
    private string GenerateGUID(){
        System.Guid guid = System.Guid.NewGuid();
        return guid.ToString();
    }
}
