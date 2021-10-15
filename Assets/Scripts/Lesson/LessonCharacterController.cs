using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    public int maxStar = 10;
    public float maxStatus = 100f;

    private List<Image> StarImages = new List<Image>();
    public Sprite[] star = new Sprite[6];

    public void SetupStar()
    {
        for (int i = 0; i < maxStar; i++)
        {
            GameObject Star = GameObject.Find(id+"Star-" + i);
            StarImages.Add(Star.GetComponent<Image>());
        }
    }



    //0:Visual 1:Vocal 2:Dance
    public void setParams()
    {
        int currentParams = 0;
        if (area == "visual") currentParams = (int)(((float)Common.progresses[id].Visual / (float)maxStatus) * 50.0f);
        else if (area == "vocal") currentParams = (int)(((float)Common.progresses[id].Vocal / (float)maxStatus) * 50.0f);
        else if (area == "dance") currentParams = (int)(((float)Common.progresses[id].Dance / (float)maxStatus) * 50.0f);
        for (int i = 0; i < currentParams / 5; i++)
        {
            StarImages[i].sprite = star[5];
        }
        if (currentParams != 50) StarImages[currentParams / 5].sprite = star[currentParams % 5];
        for (int i = currentParams / 5 + 1; i < maxStar; i++)
        {
            StarImages[i].sprite = star[0];
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
        if(oldarea!=newarea&&StarImages.Count>0)setParams();
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
        setArea();
    }


    void Update()
    {
        setArea();
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
