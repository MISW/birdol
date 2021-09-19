using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginPrefabController : MonoBehaviour 
{
    [Header("Web Client")]
    [SerializeField] private LoginWebClient loginWebClient;

    [Header("Login Input")]
    [SerializeField] InputField idInputField;
    [SerializeField] InputField passwordInputField;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text       AlertText;

    [Header("UI")]
    [SerializeField] GameObject DisplayRootUI;

    private bool isConnectionInProgress = false;

    public void OnLoginButtonClicked()
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

        string id = idInputField.text;
        string password = passwordInputField.text;
        string _uuid = GenerateGUID();
        loginWebClient.SetData(id, password, _uuid);

        //データチェックをサーバへ送信する前に行う。
        if (loginWebClient.CheckRequestData() == false)
        {
            AlertText.text = loginWebClient.message;
            Debug.Log(loginWebClient.message);
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            isConnectionInProgress = false;
            yield break;
        }

        AlertUI.SetActive(true);
        AlertText.text = "通信中...";
        float conn_start = Time.time;
        yield return StartCoroutine(loginWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        AlertUI.SetActive(false);

        //処理
        if (loginWebClient.isSuccess==true && loginWebClient.isInProgress==false)
        {
            //通信に成功した時
            //LoginWebClientはひとまずLoginResponseDataをdataに保存するとする 
            LoginWebClient.LoginResponseData lrd = (LoginWebClient.LoginResponseData)loginWebClient.data;
            Debug.Log("ParsedResponseData: \n"+lrd.ToString());
            if (loginWebClient.isLoginSuccess)
            {
                Common.Uuid = _uuid;

                AlertText.text = loginWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
                OnLoginSuccess();
            }
            else
            {
                AlertText.text = $"{loginWebClient.message}";
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            }
        }
        else
        {
            //失敗した時
            AlertText.text = $"<color=\"red\">{loginWebClient.message}</color>";
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
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

    /// <summary>
    /// ログイン成功したときの動作。例えば、Gameシーンへの遷移など。
    /// </summary>
    private void OnLoginSuccess()
    {

    }

    /// <summary>
    /// Generate UUID
    /// </summary>
    private string GenerateGUID() {
        System.Guid guid = System.Guid.NewGuid();
        return guid.ToString();
    }

    public void Open()
    {
        StopCoroutine(this.CloseCoroutine(this.DisplayRootUI));
        StartCoroutine(OpenCoroutine(this.DisplayRootUI));
    }
    private IEnumerator OpenCoroutine(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        Vector3 scaleAdd = obj.transform.localScale / 10.0f;
        obj.transform.localScale = Vector3.zero;
        obj.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            obj.transform.localScale += scaleAdd;
            yield return new WaitForFixedUpdate();
        }
        obj.transform.localScale = scale;
        yield break;
    }

    public void Close()
    {
        StopCoroutine(CloseCoroutine(this.DisplayRootUI));
        StartCoroutine(CloseCoroutine(this.DisplayRootUI));
    }
    private IEnumerator CloseCoroutine(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        for(int i = 0; i < 5; i++)
        {
            obj.transform.localScale += Vector3.one * 0.01f;
            yield return new WaitForFixedUpdate();
        }
        Vector3 scaleSub = obj.transform.localScale / 10;
        for (int i = 0; i < 10; i++)
        {
            obj.transform.localScale -= scaleSub;
            yield return new WaitForFixedUpdate();
        }
        obj.SetActive(false);
        obj.transform.localScale = scale;
        yield break;
    }
}
