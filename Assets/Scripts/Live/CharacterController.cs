using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerClickHandler
{
    public bool completedActiveSkill = false;
    public bool PassiveSkillenabled = false;
    public bool executingSkill = false;
    public int id;
    public float score = 0;
    public string area = "";
    public Text name;
    public GameObject light;
    public GameObject listchild;
    public ProgressModel characterInf;
    public List<Sprite> gifsprite = new List<Sprite>();
    IEnumerator coroutine;

    public Image backframe;
    public Text SkillName;
    public Text SkillDescription;

    public Sprite[] foot;
    public Sprite[] box1;
    public Sprite[] box2;

    public IEnumerator jump()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        float plus = 0;
        bool adding = true;
        var fixedupdate = new WaitForFixedUpdate();
        Common.subseplayer.PlayOneShot(seclips["haneru1"]);
        for (int i = 10; i >= -10; i--)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + i);
            yield return fixedupdate;
        }
    }


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

    //0:Visual 1:Vocal 2:Dance
    RectTransform rt;
    Image standing;
    Image frame;
    Text para;
    public void setParamsFont()
    {
        if (area == "visual")
        {
            frame.sprite = box1[0];
            para.text = ((int)characterInf.Visual).ToString();
            para.color = new Color(255f / 255f, 218f / 255f, 92f / 255f);
        }
        else if (area == "vocal")
        {
            frame.sprite = box1[1];
            para.text = ((int)characterInf.Vocal).ToString();
            para.color = new Color(255f / 255f, 84f / 255f, 175f / 255f);
        }
        else
        {
            frame.sprite = box1[2];
            para.text = ((int)characterInf.Dance).ToString();
            para.color = new Color(84f / 255f, 198f / 255f, 255f / 255f);
        }
    }

    public void setArea()
    {
        if (executingSkill)
        {
            //setWhite();
            return;
        }

        if (rt.localPosition.x > 100)
        {
            standing.sprite = foot[2];
            area = "dance";

        }
        else if (rt.localPosition.x < -100)
        {
            standing.sprite = foot[1];
            area = "vocal";

        }
        else
        {
            standing.sprite = foot[0];
            area = "visual";
        }
        setParamsFont();


    }

    private IEnumerator updateImg()
    {
        int index = 0;
        var wait = new WaitForSecondsRealtime(0.07f);
        Image character = gameObject.GetComponent<Image>();
        while (true)
        {
            character.sprite = gifsprite[index];
            if (index < 5) index++;
            else index = 0;
#if UNITY_EDITOR
            //Debug.Log("current:"+index);
#endif
            yield return wait;
        }
    }

    public void connectUI()
    {
        frame = listchild.transform.GetChild(2).gameObject.GetComponent<Image>();
        para = listchild.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>();
    }

    public void Awake()
    {
        standing = transform.parent.gameObject.GetComponent<Image>();
        rt = transform.parent.gameObject.GetComponent<RectTransform>();
    }

    public void stopGif()
    {
        StopCoroutine(coroutine);
    }
    public void initImage()
    {
        coroutine = updateImg();
        StartCoroutine(coroutine);
    }

    Dictionary<string, AudioClip> seclips;
    void Start()
    {
        Application.targetFrameRate = 60;
        setArea();
        light.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        seclips = new Dictionary<string, AudioClip>()
        {
            {"tsukamu1", (AudioClip)Resources.Load("SE/live/tsukamu1") },
            {"orosu1", (AudioClip)Resources.Load("SE/live/orosu1") },
            {"haneru1", (AudioClip)Resources.Load("SE/live/haneru1") },
            {"skillkettei1", (AudioClip)Resources.Load("SE/live/skillkettei1") },
        };
    }


    void Update()
    {
        setArea();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!LiveController.executingSkills && eventData.position.y <= Screen.height / 2.0f + 180.0f)
        {// �h���b�O���͈ʒu���X�V����
            Vector2 parenttransform = eventData.position;
            parenttransform.y -= 150;
            //parenttransform.y -= 80;
            transform.parent.position = parenttransform;
            setArea();
        }
    }

    public void SelectMe()
    {
        if (!LiveController.executingSkills)
        {
            LiveController.selectedcharacter = id;
            if (characterInf.BestSkill == "visual") backframe.sprite = box2[0];
            else if (characterInf.BestSkill == "vocal") backframe.sprite = box2[1];
            else if (characterInf.BestSkill == "dance") backframe.sprite = box2[2];
            SkillDescription.text = characterInf.ActiveSkillDescription;
            SkillName.text = characterInf.ActiveSkillName;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectMe();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Common.subseplayer.PlayOneShot(seclips["orosu1"]);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Common.subseplayer.PlayOneShot(seclips["skillkettei1"]);
        SelectMe();
    }
}
