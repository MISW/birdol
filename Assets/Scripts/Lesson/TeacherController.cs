using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeacherController : MonoBehaviour
{

    public List<Sprite> gifsprite = new List<Sprite>();
    IEnumerator coroutine;
    public string area = "";
    public Text name;
    public Sprite[] foot;

    private IEnumerator updateImg()
    {
        int index = 1;
        var wait = new WaitForSecondsRealtime(0.07f);
        while (true)
        {
            gameObject.GetComponent<Image>().sprite = gifsprite[index];
            if (index < 5) index++;
            else index = 0;
#if UNITY_EDITOR
            //Debug.Log("current:"+index);
#endif
            yield return wait;
        }
    }

    public void stopGif()
    {
        StopCoroutine(coroutine);
    }
    public void initImage()
    {
        coroutine = updateImg();
        StartCoroutine(coroutine);
        //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + characterInf.MainCharacterId);
    }

    public void updatePos()
    {
        area = RandomArray.GetRandom(new string[] { "visual", "vocal", "dance" });
        RectTransform rt;
        rt = transform.parent.gameObject.GetComponent<RectTransform>();
        Image standing = transform.parent.gameObject.GetComponent<Image>();
        if (area == "dance")
        {
            Vector2 pos = transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition;
            pos.x = 180;
            transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }else if (area == "vocal")
        {
            Vector2 pos = transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition;
            pos.x = -180;
            transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }else if (area == "visual")
        {
            Vector2 pos = transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition;
            pos.x = 0;
            transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

}
