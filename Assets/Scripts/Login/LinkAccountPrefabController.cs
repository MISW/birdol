using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アカウント連携 
/// </summary>
public class LinkAccountPrefabController : MonoBehaviour
{
    [Header("LinkAccount Input")]
    [SerializeField] InputField idInputField;
    [SerializeField] InputField passwordInputField;

    [Header("Display")]
    [SerializeField] GameObject AlertUI;
    [SerializeField] Text AlertText;

    [Header("UI")]
    [SerializeField] GameObject DisplayRootUI;

    private bool isConnectionInProgress = false;
    private bool isAnimating = false;

    private void Start()
    {
    }

    public void OnLinkAccountButtonClicked()
    {
    }

    /// <summary>
    /// LinkAccount Request 
    /// </summary>
    /// <returns></returns>
    //Show (GameObject)gm for (float)duration seconds
    private IEnumerator ShowForWhileCoroutine(float duration, GameObject gm)
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
        //Titleシーンへ遷移
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        Manager.manager.StateQueue((int)gamestate.Title);
    }

    /// <summary>
    /// Generate UUID
    /// </summary>
    private string GenerateGUID()
    {
        System.Guid guid = System.Guid.NewGuid();
        return guid.ToString();
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
