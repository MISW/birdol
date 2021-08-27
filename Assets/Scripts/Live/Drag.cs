using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IDragHandler,IBeginDragHandler { 
    public Text test;
    public GameObject Middle;
    public bool selected = false;
    public int id = 0;
    int characterId;
    public string area = "";
    public string selectedSkill = "";


    void setArea()
    {
        RectTransform rt;
        rt = transform.parent.gameObject.GetComponent<RectTransform>();
        Image standing = transform.parent.gameObject.GetComponent<Image>();
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
        // ドラッグ中は位置を更新する
        Vector2 parenttransform = eventData.position;
        parenttransform.y -= 150;
        transform.parent.position = parenttransform; 
        setArea();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        LiveController.selectedcharacter = id;
    }
}
