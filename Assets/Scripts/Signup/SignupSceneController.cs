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

    [Header("Input")]
    [SerializeField] InputField usernameInputField;
    [SerializeField] Button signupButton;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text AlertText;

    private bool isConnectionInProgress = false;


    private void Start()
    {

        SetUpButtonEvent();
    }

    private void SetUpButtonEvent()
    {
        //Signup
        signupButton.onClick.AddListener(() =>
        {
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
        string username = usernameInputField.text;
        Common.PlayerName = username;

        AlertUI.SetActive(true);
        AlertText.text = "初期設定が完了しました。";
        yield return new WaitForSeconds(1.0f);

        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        yield return new WaitForSeconds(0.5f);

        ProgressService.FetchStory();
        ProgressService.FetchCompletedProgressAndUpdateGameStatus("home");
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
    }

    /// <summary>
    /// Generate UUID
    /// </summary>
    private string GenerateGUID()
    {
        System.Guid guid = System.Guid.NewGuid();
        return guid.ToString();
    }
}
