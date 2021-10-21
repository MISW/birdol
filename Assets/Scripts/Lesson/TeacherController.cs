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
        var wait = new WaitForSeconds(0.07f);
        while (true)
        {
            gameObject.GetComponent<Image>().sprite = gifsprite[index];
            if (index < 5) index++;
            else index = 0;
            //Debug.Log("current:"+index);
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

    public void initPos()
    {
        area = RandomArray.GetRandom(new string[] { "visual", "vocal", "dance" });
        RectTransform rt;
        rt = transform.parent.gameObject.GetComponent<RectTransform>();
        Image standing = transform.parent.gameObject.GetComponent<Image>();
        if (area == "dance")
        {
            Vector2 pos = new Vector2();
            pos.x = Random.Range(102, 125);
            pos.y = Random.Range(-350, 0);
            transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }else if (area == "vocal")
        {
            Vector2 pos = new Vector2();
            pos.x = Random.Range(-125, -102);
            pos.y = Random.Range(-350, 0);
            transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }else if (area == "visual")
        {
            Vector2 pos = new Vector2();
            pos.x = Random.Range(-100, 100);
            pos.y = Random.Range(-250, 0);
            transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

}
