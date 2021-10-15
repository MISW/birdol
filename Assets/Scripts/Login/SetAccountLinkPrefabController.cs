using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// データ連携のためにパスワードを保存するためのUI
/// </summary>
public class SetAccountLinkPrefabController : MonoBehaviour
{
    [Header("Web Client")]
    private SetAccountLinkWebClient setAccountLinkWebClient;

    [Header("Account Info Input")]
    [SerializeField] InputField idInputField;
    [SerializeField] InputField passwordInputField;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text AlertText;

    [Header("UI")]
    [SerializeField] GameObject DisplayRootUI;

    private bool isAnimating = false;
    private bool isConnectionInProgress = false;

    private void Start()
    {
        idInputField.text = Common.DefaultAccountID;
        this.setAccountLinkWebClient = new SetAccountLinkWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/auth");
    }

    public void OnSetAccountLinkButtonClicked()
    {
        if (isConnectionInProgress) return;
        StartCoroutine(SetAccountLink());
    }

    /// <summary>
    /// Account Edit Request. This info is used to linkAccount later.  
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetAccountLink()
    {
        isConnectionInProgress = true;

        string password = passwordInputField.text;
        setAccountLinkWebClient.SetData(password);

        //データチェックをサーバへ送信する前に行う。
        if (setAccountLinkWebClient.CheckRequestData() == false)
        {
            AlertText.text = setAccountLinkWebClient.message;
            Debug.Log(setAccountLinkWebClient.message);
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            isConnectionInProgress = false;
            yield break;
        }

        AlertUI.SetActive(true);
        AlertText.text = "通信中...";
        float conn_start = Time.time;
        yield return StartCoroutine(setAccountLinkWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        AlertUI.SetActive(false);

        //処理
        if (setAccountLinkWebClient.isSuccess == true && setAccountLinkWebClient.isInProgress == false)
        {
            //通信に成功した時
            SetAccountLinkWebClient.SetAccountLinkResponseData aerd = (SetAccountLinkWebClient.SetAccountLinkResponseData)setAccountLinkWebClient.data;
            Debug.Log("ParsedResponseData: \n" + aerd.ToString());
            if (setAccountLinkWebClient.isSetAccountLinkSuccess)
            {
                AlertText.text = setAccountLinkWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
                OnSetAccountSuccess();
            }
            else
            {
                AlertText.text = $"<color=\"black\">{setAccountLinkWebClient.message}</color>";
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            }
        }
        else
        {
            //失敗した時
            AlertText.text = $"<color=\"red\">{setAccountLinkWebClient.message}</color>";
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

    /// <summary>
    /// データ連携用パスワード設定に成功したときの処理 
    /// </summary>
    private void OnSetAccountSuccess()
    {

    }

    public void Open()
    {
        if (isAnimating) return;
        StopCoroutine(this.CloseCoroutine(this.DisplayRootUI));
        StartCoroutine(OpenCoroutine(this.DisplayRootUI));
    }
    private IEnumerator OpenCoroutine(GameObject obj)
    {
        if (isAnimating) yield break;
        isAnimating = true;
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
        isAnimating = false;
        yield break;
    }

    public void Close()
    {
        if (isAnimating) return;
        StopCoroutine(CloseCoroutine(this.DisplayRootUI));
        StartCoroutine(CloseCoroutine(this.DisplayRootUI));
    }
    private IEnumerator CloseCoroutine(GameObject obj)
    {
        if (isAnimating) yield break;
        isAnimating = true;
        Vector3 scale = obj.transform.localScale;
        for (int i = 0; i < 5; i++)
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
        isAnimating = false;
        yield break;
    }
}
