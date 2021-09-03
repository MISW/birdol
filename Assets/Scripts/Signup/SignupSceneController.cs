using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SignupWebClient))]
public class SignupSceneController : SceneVisor
{
    [Header("SignUp Web Client")]
    [SerializeField] private SignupWebClient signupWebClient;

    [Header("Input")]
    [SerializeField] InputField usernameInputField;
    [SerializeField] InputField emailInputField;
    [SerializeField] InputField passwordInputField;
    [SerializeField] Button signupButton;

    [Header("Display")]
    [SerializeField] GameObject ConnectionInProgressDisplayGameObject;
    [SerializeField] GameObject SuccessDisplayGameObject;
    [SerializeField] Text SuccessDisplayText;
    [SerializeField] GameObject ErrorDisplayGameObject;
    [SerializeField] Text ErrorDisplayText;

    [Header("Load Login Scene")]
    [SerializeField] Button loadLoginSceneButton;

    private bool isConnectionInProgress = false;


    private void Start()
    {
        SetUpButtonEvent();
    }

    private void SetUpButtonEvent()
    {
        //Signup
        signupButton.onClick.AddListener(() => {
            OnLoginButtonClicked();
        });
        //Login
        loadLoginSceneButton.onClick.AddListener(() =>
        {
            LoadLoginScene();
        });
    }

    private void OnLoginButtonClicked()
    {
        if (isConnectionInProgress) return;
        StartCoroutine(Login());
    }

    /// <summary>
    /// Login Request 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Login()
    {
        isConnectionInProgress = true;

        string username = usernameInputField.text;
        string email = emailInputField.text;
        string password = passwordInputField.text;
        string _uuid = GenerateGUID();
        signupWebClient.SetData(username, email, password, _uuid);

        //データチェックをサーバへ送信する前に行う。
        if (signupWebClient.CheckRequestData()==false)
        {
            ErrorDisplayText.text = signupWebClient.message;
            Debug.Log(signupWebClient.message);
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, ErrorDisplayGameObject));
            isConnectionInProgress = false;
            yield break;
        }

        ConnectionInProgressDisplayGameObject.SetActive(true);
        float conn_start = Time.time;
        yield return StartCoroutine(signupWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        ConnectionInProgressDisplayGameObject.SetActive(false);

        //処理
        if (signupWebClient.isSuccess == true && signupWebClient.isInProgress == false)
        {
            //成功した時
            SignupWebClient.SignupResponseData lrd = (SignupWebClient.SignupResponseData)signupWebClient.data;
            Debug.Log("ParsedResponseData: \n" + lrd.ToString());
            if (signupWebClient.isSignupSuccess)
            {
                // Store UUID to PlayerPrefs
                PlayerPrefs.SetString(Common.PLAYERPREFS_UUID, _uuid);
                PlayerPrefs.Save();
                
                SuccessDisplayText.text = signupWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, SuccessDisplayGameObject));                
                OnSignupSuccess();
            }
            else
            {
                ErrorDisplayText.text = signupWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, ErrorDisplayGameObject));
            }
        }
        else
        {
            //失敗した時
            ErrorDisplayText.text = signupWebClient.message;
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, ErrorDisplayGameObject));
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

    //LoginSceneへ移動
    private void LoadLoginScene()
    {
        Debug.Log("<color=\"red\">シーンのロードにこのゲーム用のManagerではなくUniryEngine.SceneManagement.SceneManagerを使っています。要修正</color>");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }

    /// <summary>
    /// アカウント登録に成功したときの動作。例えば、Gameシーンへの遷移など。
    /// </summary>
    private void OnSignupSuccess()
    {

    }

    /// <summary>
    /// Generate UUID
    /// </summary>
    private string GenerateGUID(){
        System.Guid guid = System.Guid.NewGuid();
        return guid.ToString();
    }
}
