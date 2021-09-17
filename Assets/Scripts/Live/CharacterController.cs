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
    public bool executingSkill = false;
    public int id;
    public string area = "";
    public Text name;
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
            setWhite();
            return;
        }
        if (rt.localPosition.x > 150)
        {
            standing.color = new Color(151f / 255f, 187f / 255f, 223f / 255f);
            area = "dance";

        }
        else if (rt.localPosition.x < -150)
        {
            standing.color = new Color(246f / 255f, 158f / 255f, 216f / 255f);
            area = "visual";
        }
        else
        {
            standing.color = new Color(198f / 255f, 190f / 255f, 86f / 255f);
            area = "vocal";
        }
       
    }

    public void initImage()
    {
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + characterInf.characterId);
    }

    void Start()
    {
        setArea();
    }


    void Update()
    {
        setArea();
    }

    public void OnDrag(PointerEventData eventData)
    {
        /*
        if (!LiveController.executingSkills&&eventData.position.y<=840)
        {// ドラッグ中は位置を更新する
            
        }*/

        Vector2 parenttransform = eventData.position;
        parenttransform.y -= 150;
        transform.parent.position = parenttransform;
        setArea();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!LiveController.executingSkills) LiveController.selectedcharacter = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!LiveController.executingSkills)LiveController.selectedcharacter = id;
    }
}
