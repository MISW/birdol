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

    public Sprite[] foot;
    public float maxStatus = 100f;

    public TextMeshProUGUI para;


    //0:Visual 1:Vocal 2:Dance
    public void setParams()
    {
        if (area == "visual")
        {
            string paranum = string.Format("{0:F1}", (float)Common.progresses[id].Visual);
            int dot = paranum.IndexOf(".");
            para.text = $"<size=40>{paranum.Substring(0, dot)}</size><size=25>{paranum.Substring(dot)}</size>";
            para.color = new Color(255f / 255f, 218f / 255f, 92f / 255f);
        }
        else if (area == "vocal")
        {
            string paranum = string.Format("{0:F1}", (float)Common.progresses[id].Vocal);
            int dot = paranum.IndexOf(".");
            para.text = $"<size=40>{paranum.Substring(0, dot)}</size><size=25>{paranum.Substring(dot)}</size>";
            para.color = new Color(255f / 255f, 84f / 255f, 175f / 255f);
        }
        else
        {
            string paranum = string.Format("{0:F1}", (float)Common.progresses[id].Dance);
            int dot = paranum.IndexOf(".");
            para.text = $"<size=40>{paranum.Substring(0, dot)}</size><size=25>{paranum.Substring(dot)}</size>";
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
        if (!LiveController.executingSkills&&eventData.position.y<=Screen.height/2.0f+180.0f)
        {// ドラッグ中は位置を更新する
            Vector2 parenttransform = eventData.position;
            parenttransform.y -= 150;
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
