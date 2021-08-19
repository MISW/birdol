using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LoginWebClient))]
public class LoginSceneController : SceneVisor 
{
    [Header("Login Web Client")]
    [SerializeField] private LoginWebClient loginWebClient; 

    [Header("Input")]
    [SerializeField] InputField emailInputField;
    [SerializeField] InputField passwordInputField;
    [SerializeField] Button     loginButton;

    [Header("Display")]
    [SerializeField] GameObject ConnectionInProgressDisplayGameObject;
    [SerializeField] GameObject SuccessDisplayGameObject;
    [SerializeField] Text       SuccessDisplayText;
    [SerializeField] GameObject ErrorDisplayGameObject;
    [SerializeField] Text       ErrorDisplayText;

    [Header("Load Signup Scene")]
    [SerializeField] Button loadSignupSceneButton;

    private bool isConnectionInProgress = false;


    private void Start()
    {
        SetUpButtonEvent();
    }

    private void SetUpButtonEvent()
    {
        loginButton.onClick.AddListener(() => {
            OnLoginButtonClicked();
        });
        loadSignupSceneButton.onClick.AddListener(() =>
        {
            LoadSignupScene();
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

        string email = emailInputField.text;
        string password = passwordInputField.text;
        loginWebClient.SetData(email, password);

        //データチェックをサーバへ送信する前に行う。
        if (loginWebClient.CheckRequestData() == false)
        {
            ErrorDisplayText.text = loginWebClient.message;
            Debug.Log(loginWebClient.message);
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, ErrorDisplayGameObject));
            isConnectionInProgress = false;
            yield break;
        }

        ConnectionInProgressDisplayGameObject.SetActive(true);
        float conn_start = Time.time;
        yield return StartCoroutine(loginWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        ConnectionInProgressDisplayGameObject.SetActive(false);

        //処理
        if (loginWebClient.isSuccess==true && loginWebClient.isInProgress==false)
        {
            //通信に成功した時
            //LoginWebClientはひとまずLoginResponseDataをdataに保存するとする 
            LoginWebClient.LoginResponseData lrd = (LoginWebClient.LoginResponseData)loginWebClient.data;
            Debug.Log("ParsedResponseData: \n"+lrd.ToString());
            if (loginWebClient.isLoginSuccess)
            {
                SuccessDisplayText.text = loginWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, SuccessDisplayGameObject));
                OnLoginSuccess();
            }
            else
            {
                ErrorDisplayText.text = loginWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, ErrorDisplayGameObject));
            }
        }
        else
        {
            //失敗した時
            ErrorDisplayText.text = loginWebClient.message;
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, ErrorDisplayGameObject));
        }

        isConnectionInProgress = false;
        yield break;
    }

    //Show (GameObject)gm for (float)duration seconds
    private IEnumerator ShowForWhileCoroutine(float duration,GameObject gm)
    {
        gm.SetActive(true);
        yield return new WaitForSeconds(duration);
        gm.SetActive(false);
        yield break;
    }

    //SignupSceneへ移動
    private void LoadSignupScene()
    {
        Debug.Log("<color=\"red\">シーンのロードにこのゲーム用のManagerではなくUniryEngine.SceneManagement.SceneManagerを使っています。要修正</color>");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Signup");
    }

    /// <summary>
    /// ログイン成功したときの動作。例えば、Gameシーンへの遷移など。
    /// </summary>
    private void OnLoginSuccess()
    {

    }
}
