using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LiveController : MonoBehaviour
{
    public GameObject startbutton;
    public GameObject Background;
    public GameObject ResultUI;
    public GameObject sailium;
    public Text testText;
    int dance = 0;
    int visual = 0;
    int vocal = 0;
    public static int selectedcharacter;
    public ProgressModel[] characters;

    HashSet<GameObject> sailiumcollections = new HashSet<GameObject>();
    public GameObject ScoreBar;
    public float max = 100;
    public float score = 0;
    public int remainingTurn = 5;
    float showspeed = 0.01f;

    public static bool executingSkills = false;
    void checkPos()
    {
        dance = 0;
        visual = 0;
        vocal = 0;
        bool active = false;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        Array.Sort(objs, delegate (GameObject a1, GameObject a2) { return -1*a1.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y
            .CompareTo(a2.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y); });
        int depth = 0;
        for (int i=0;i<5;i++)
        {
            string area = objs[i].GetComponent<CharacterController>().area;
            if (area == "dance") dance++;
            else if (area == "visual") visual++;
            else if (area == "vocal") vocal++;
            depth++;
            if (objs[i].GetComponent<CharacterController>().id == selectedcharacter)
            {
                objs[i].GetComponent<CharacterController>().light.SetActive(true);
                active = objs[i].GetComponent<CharacterController>().completedActiveSkill;
            }
            else
            {
                objs[i].GetComponent<CharacterController>().light.SetActive(false);
            }
            objs[i].transform.parent.gameObject.transform.SetSiblingIndex(i);
        }
        if ((dance <= 2 && visual <= 2 && vocal <= 2)&&!active)
        {
            startbutton.SetActive(true);
        }
        else
        {
            startbutton.SetActive(false);
        }
    }

    private GameObject[] listchilds;


    void initLiveStage()
    {
        executingSkills = false;
        selectedcharacter = 0;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        listchilds = GameObject.FindGameObjectsWithTag("CharacterList");
        for (int i = 0; i < 5; i++)
        {
            objs[i].GetComponent<CharacterController>().id = i;
            objs[i].GetComponent<CharacterController>().characterInf = characters[i];
            objs[i].GetComponent<CharacterController>().name.text = characters[i].characterName;
            objs[i].GetComponent<CharacterController>().initImage();
            listchilds[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + characters[i].characterId);
            listchilds[i].transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = ((int)characters[i].visual).ToString();
            listchilds[i].transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = ((int)characters[i].vocal).ToString();
            listchilds[i].transform.GetChild(2).GetChild(2).gameObject.GetComponent<Text>().text = ((int)characters[i].dance).ToString();
            objs[i].GetComponent<CharacterController>().listchild = listchilds[i];

        }
        /*
        for (int y = -250; y < 50; y += 60)
        {
            for (int x = -200; x < 200; x += 40)
            {
                GameObject instance = Instantiate(sailium, Background.transform);
                instance.transform.parent = Background.transform;
                instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(x,y);
                sailiumcollections.Add(instance);
            }
        }*/
        

    }

    private IEnumerator updateScoreBar(float oldScore, float newScore)
    {
        System.Random random = new System.Random();
        while (oldScore < newScore)
        {
            oldScore++;
            if ((int)oldScore % 2 == 0&& sailiumcollections.Count>0)
            {
                GameObject gameObject = sailiumcollections.ElementAt(random.Next(sailiumcollections.Count));
                //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/sailium");
                //sailiumcollections.Remove(gameObject);
            }
            ScoreBar.GetComponent<Scrollbar>().size = oldScore / max;
            yield return new WaitForSeconds(showspeed);
        }
    }

    private IEnumerator execSkillofOnePerson(CharacterController characterObj,int index,bool selected)
    {
        characterObj.executingSkill = true;
        float newscore = score;
        //�p�����[�^�X�R�A
        float parascore;
        ProgressModel characterInf = characterObj.characterInf;
        if (characterObj.area == "visual") parascore = characterInf.visual;
        else if (characterObj.area == "vocal") parascore = characterInf.vocal;
        else parascore = characterInf.dance;
        //�A�N�e�B�u�X�L��
        if (selected)
        {
            if (characterInf.activeSkillType == SkillType.Plus)
            {
                parascore += (characterInf.activeSkillScore * characterInf.activeSkillLevel);
            }
            else
            {
                parascore *= (characterInf.activeSkillScore * characterInf.activeSkillLevel);
            }
            CharacterModel upperInfo = Common.characters[characterInf.characterId];
            testText.text = upperInfo.name + " の " + upperInfo.skillname;
            //listchilds[index].transform.GetChild(0).gameObject.SetActive(true);
        }
        //スコアの更新
        newscore += parascore;
        yield return updateScoreBar(score, newscore);
        score = newscore;
        //yield return new WaitForSeconds(1.0f);
        characterObj.executingSkill = false;
    }

    private IEnumerator execSkills()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        testText.text = "Running...";
        for (int i = 0; i < 5; i++)
        {
            yield return execSkillofOnePerson(objs[i].GetComponent<CharacterController>(),i,i==selectedcharacter);
        }
        remainingTurn--;
        objs[selectedcharacter].GetComponent<CharacterController>().finishSkill();
        if (remainingTurn > 0)
        {
            testText.text = "残りターン数:"+remainingTurn;
            executingSkills = false;
        }else if (score >= max)
        {
            //testText.text = "Mission Complete!";
            var resultchanger = ResultUI.GetComponent<UIchanger>();
            resultchanger.Judge_Image_num = 0;
            resultchanger.Score_num = (int)score;
            resultchanger.Achievement_num = score / max;
            ResultUI.SetActive(true);
        }
        else
        {
            //testText.text = "Mission Failed!";
            var resultchanger = ResultUI.GetComponent<UIchanger>();
            resultchanger.Judge_Image_num = 1;
            resultchanger.Score_num = (int)score;
            resultchanger.Achievement_num = score / max;
            ResultUI.SetActive(true);
        }
    }

    public void onStartButtonClick()
    {
        executingSkills = true;
        startbutton.SetActive(false);
        StartCoroutine(execSkills());
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        if (Common.characters == null) Common.initCharacters();//Test Only
        testText.text = "残りターン数:" + 5;
        initLiveStage();
        checkPos();
    }

    void Update()
    {
        if(!executingSkills)checkPos();
    }
}
