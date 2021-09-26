using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アカウント連携 
/// </summary>
public class LinkAccountPrefabController : MonoBehaviour 
{
    [Header("Web Client")]
    private LinkAccountWebClient linkAccountWebClient;

    [Header("LinkAccount Input")]
    [SerializeField] InputField idInputField;
    [SerializeField] InputField passwordInputField;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text       AlertText;

    [Header("UI")]
    [SerializeField] GameObject DisplayRootUI;

    private bool isConnectionInProgress = false;

    private void Start()
    {
        this.linkAccountWebClient = new LinkAccountWebClient(WebClient.HttpRequestMethod.Post, $"/api/{Common.api_version}/user");
    }

    public void OnLinkAccountButtonClicked()
    {
        if (isConnectionInProgress) return;
        StartCoroutine(LinkAccount());
    }

    /// <summary>
    /// LinkAccount Request 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LinkAccount()
    {
        isConnectionInProgress = true;

        string id = idInputField.text;
        string password = passwordInputField.text;
        string _uuid = GenerateGUID();
        (string privateKey, string publicKey) rsaKeyPair = Common.CreateRsaKeyPair();
        linkAccountWebClient.SetData(id, password, _uuid, rsaKeyPair.publicKey, rsaKeyPair.privateKey);

        //データチェックをサーバへ送信する前に行う。
        if (linkAccountWebClient.CheckRequestData() == false)
        {
            AlertText.text = linkAccountWebClient.message;
            Debug.Log(linkAccountWebClient.message);
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            isConnectionInProgress = false;
            yield break;
        }

        AlertUI.SetActive(true);
        AlertText.text = "通信中...";
        float conn_start = Time.time;
        yield return StartCoroutine(linkAccountWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        AlertUI.SetActive(false);

        //処理
        if (linkAccountWebClient.isSuccess==true && linkAccountWebClient.isInProgress==false)
        {
            //通信に成功した時 
            LinkAccountWebClient.LinkAccountResponseData lrd = (LinkAccountWebClient.LinkAccountResponseData)linkAccountWebClient.data;
            Debug.Log("ParsedResponseData: \n"+lrd.ToString());
            if (linkAccountWebClient.isLinkAccountSuccess)
            {
                Common.Uuid = _uuid;
                Common.RsaKeyPair = rsaKeyPair;
                AlertText.text = linkAccountWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
                OnLinkAccountSuccess();
            }
            else
            {
                AlertText.text = $"{linkAccountWebClient.message}";
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            }
        }
        else
        {
            //失敗した時
            AlertText.text = $"<color=\"red\">{linkAccountWebClient.message}</color>";
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
    /// アカウント連携に成功したときの動作。例えば、Gameシーンへの遷移など。
    /// </summary>
    private void OnLinkAccountSuccess()
    {
        //TODO Menuシーンへ遷移 
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
