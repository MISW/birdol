using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUtil : MonoBehaviour
{
    [SerializeField] public GameObject DisplayRootUI;

    private bool isAnimating=false;

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
