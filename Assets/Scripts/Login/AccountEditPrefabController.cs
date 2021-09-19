using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountEditPrefabController : MonoBehaviour
{
    [Header("Web Client")]
    [SerializeField] private AccountEditWebClient accountEditWebClient;

    [Header("Account Info Input")]
    [SerializeField] InputField idInputField;
    [SerializeField] InputField passwordInputField;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text AlertText;

    [Header("UI")]
    [SerializeField] GameObject DisplayRootUI;

    private bool isConnectionInProgress = false;

    public void OnAccountEditButtonClicked()
    {
        if (isConnectionInProgress) return;
        StartCoroutine(AccountEdit());
    }

    /// <summary>
    /// Account Edit Request. This info is used to login later.  
    /// </summary>
    /// <returns></returns>
    private IEnumerator AccountEdit()
    {
        isConnectionInProgress = true;

        string id = idInputField.text;
        string password = passwordInputField.text;
        accountEditWebClient.SetData(id, password, Common.Uuid);

        //データチェックをサーバへ送信する前に行う。
        if (accountEditWebClient.CheckRequestData() == false)
        {
            AlertText.text = accountEditWebClient.message;
            Debug.Log(accountEditWebClient.message);
            yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            isConnectionInProgress = false;
            yield break;
        }

        AlertUI.SetActive(true);
        AlertText.text = "通信中...";
        float conn_start = Time.time;
        yield return StartCoroutine(accountEditWebClient.Send());
        float conn_end = Time.time;
        if (conn_end - conn_start > 0) yield return new WaitForSeconds(0.5f); //ユーザ側視点としては、通信時間としてに必ず最低0.5秒はかかるとする。さもなくば「通信中...」の表示がフラッシュみたいになって気持ち悪い気がする。
        AlertUI.SetActive(false);

        //処理
        if (accountEditWebClient.isSuccess == true && accountEditWebClient.isInProgress == false)
        {
            //通信に成功した時
            AccountEditWebClient.AccountEditResponseData aerd = (AccountEditWebClient.AccountEditResponseData)accountEditWebClient.data;
            Debug.Log("ParsedResponseData: \n" + aerd.ToString());
            if (accountEditWebClient.isEditSuccess)
            {
                AlertText.text = accountEditWebClient.message;
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
                OnAccountEditSuccess();
            }
            else
            {
                AlertText.text = $"<color=\"black\">{accountEditWebClient.message}</color>";
                yield return StartCoroutine(ShowForWhileCoroutine(2.0f, AlertUI));
            }
        }
        else
        {
            //失敗した時
            AlertText.text = $"<color=\"red\">{accountEditWebClient.message}</color>";
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
    /// ログイン成功したときの動作。例えば、Gameシーンへの遷移など。
    /// </summary>
    private void OnAccountEditSuccess()
    {

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
        yield break;
    }
}
