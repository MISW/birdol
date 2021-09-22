using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour, IDragHandler,IBeginDragHandler,IPointerClickHandler { 
    public bool completedActiveSkill = false;
    public bool completedPassiveSkill = false;
    public bool executingSkill = false;
    public int id;
    public float score = 0;
    public string area = "";
    public Text name;
    public GameObject light;
    public GameObject listchild;
    public ProgressModel characterInf;

    public void finishSkill()
    {
        completedActiveSkill = true;
        Image image = gameObject.GetComponent<Image>();
        Color newcolor = image.color;
        newcolor.r -= (80f / 255f);
        newcolor.g -= (80f / 255f);
        newcolor.b -= (80f / 255f);
        image.color = newcolor;
        Image imagelistchild = listchild.transform.GetChild(0).gameObject.GetComponent<Image>();
        Color newlccolor = image.color;
        newlccolor.r -= (80f / 255f);
        newlccolor.g -= (80f / 255f);
        newlccolor.b -= (80f / 255f);
        imagelistchild.color = newcolor;
    }

    public void setWhite()
    {
        RectTransform rt;
        rt = transform.parent.gameObject.GetComponent<RectTransform>();
        Image standing = transform.parent.gameObject.GetComponent<Image>();
        standing.color = new Color(255f, 255f, 255f);
        
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
        if (rt.localPosition.x > 100)
        {
            standing.sprite = Resources.Load<Sprite>("Images/Live/footlight_da");
            area = "dance";
            
        }
        else if (rt.localPosition.x < -100)
        {
            standing.sprite = Resources.Load<Sprite>("Images/Live/footlight_vo");
            area = "vocal";
            
        }
        else
        {
            standing.sprite = Resources.Load<Sprite>("Images/Live/footlight_vi");
            area = "visual";
        }
       
    }

    public void initImage()
    {
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + characterInf.characterId);
    }

    void Start()
    {
        setArea();
        light.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }


    void Update()
    {
        setArea();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.position.y);
        if (!LiveController.executingSkills&&eventData.position.y<=Screen.height/2.0f+180.0f)
        {// ドラッグ中は位置を更新する
            Vector2 parenttransform = eventData.position;
            parenttransform.y -= 150;
            transform.parent.position = parenttransform;
            setArea();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!LiveController.executingSkills) LiveController.selectedcharacter = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!LiveController.executingSkills) LiveController.selectedcharacter = id;
    }
}
