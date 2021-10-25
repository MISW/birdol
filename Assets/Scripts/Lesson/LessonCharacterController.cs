using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LessonCharacterController : MonoBehaviour, IDragHandler,IBeginDragHandler,IPointerClickHandler { 
    public bool completedActiveSkill = false;
    public bool completedPassiveSkill = false;
    public bool executingSkill = false;
    public int id;
    public string area = "";
    public Text name;
    public GameObject listchild;
    public List<Sprite> gifsprite = new List<Sprite>();
    IEnumerator coroutine;

    public Image backframe;
    public Image frame;
    public Text para;

    public Sprite[] foot;
    public Sprite[] box1;
    public float maxStatus = 100f;

    public IEnumerator jump()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        float plus = 0;
        bool adding = true;
        var fixedupdate = new WaitForFixedUpdate();
        for (int i = 10; i >= -10; i--)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + i);
            yield return fixedupdate;
        }
    }

    public void connectUI()
    {
        frame = listchild.transform.GetChild(1).gameObject.GetComponent<Image>();
        para = listchild.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
    }

    //0:Visual 1:Vocal 2:Dance
    public void setParams()
    {
        if (area == "visual")
        {
            frame.sprite = box1[0];
            para.text = ((int)Common.progresses[id].Visual).ToString();
            para.color = new Color(255f / 255f, 218f / 255f, 92f / 255f);
        }
        else if (area == "vocal")
        {
            frame.sprite = box1[1];
            para.text = ((int)Common.progresses[id].Vocal).ToString();
            para.color = new Color(255f / 255f, 84f / 255f, 175f / 255f);
        }
        else
        {
            frame.sprite = box1[2];
            para.text = ((int)Common.progresses[id].Dance).ToString();
            para.color = new Color(84f / 255f, 198f / 255f, 255f / 255f);
        }


    }
    public void setArea()
    {
        RectTransform rt;
        rt = transform.parent.gameObject.GetComponent<RectTransform>();
        Image standing = transform.parent.gameObject.GetComponent<Image>();
        if (executingSkill)
        {
            //setWhite();
            return;
        }
        string oldarea = area;
        string newarea;
        if (rt.localPosition.x > 100)
        {
            standing.sprite = foot[2];
            newarea = "dance";
            
        }
        else if (rt.localPosition.x < -100)
        {
            standing.sprite = foot[1];
            newarea = "vocal";
            
        }
        else
        {
            standing.sprite = foot[0];
            newarea = "visual";
        }
        area = newarea;
        if(oldarea!=newarea)setParams();
    }

    private IEnumerator updateImg()
    {
        int index = 0;
        var wait = new WaitForSecondsRealtime(0.07f);
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

    void Start()
    {
        Application.targetFrameRate = 60;
    }


    void Update()
    {
        if(para!=null)setArea();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.position.y<=Screen.height/2.0f+289.0f)
        {// ドラッグ中は位置を更新する
            Vector2 parenttransform = eventData.position;
            //parenttransform.y -= 150;
            parenttransform.y -= 80;
            transform.parent.position = parenttransform;
            setArea();
        }
    }

    public void SelectMe()
    {
        if (!LiveController.executingSkills)
        {
            LiveController.selectedcharacter = id;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectMe();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectMe();
    }
}
