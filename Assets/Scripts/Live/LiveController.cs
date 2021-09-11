using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LiveController : MonoBehaviour
{
    public GameObject startbutton;
    public GameObject light;
    public Text testText;
    int dance = 0;
    int visual = 0;
    int vocal = 0;
    public static int selectedcharacter;
    public ProgressModel[] characters;

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
            objs[i].transform.parent.gameObject.transform.SetSiblingIndex(i);
            depth++;
            if (objs[i].GetComponent<CharacterController>().id == selectedcharacter)
            {
                Vector2 position = new Vector2(0,300);
                light.transform.parent = objs[i].transform;
                light.transform.localPosition = position;
                light.transform.SetSiblingIndex(i);
                active = objs[i].GetComponent<CharacterController>().completedActiveSkill;

            }
        }
        //Debug.Log("Dance:"+dance+ " Visual:" + visual+" Vocal:"+vocal);
        if ((dance <= 2 && visual <= 2 && vocal <= 2)&&!active)
        {
            startbutton.SetActive(true);
        }
        else
        {
            startbutton.SetActive(false);
        }
    }

    
    void initLiveStage()
    {
        executingSkills = false;
        selectedcharacter = 0;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        for (int i = 0; i < 5; i++)
        {
            objs[i].GetComponent<CharacterController>().id = i;
            objs[i].GetComponent<CharacterController>().characterInf = characters[i];
            objs[i].GetComponent<CharacterController>().initImage();
        }
    }
    private IEnumerator updateScoreBar(float oldScore, float newScore)
    {
        while (oldScore < newScore)
        {
            oldScore++;
            ScoreBar.GetComponent<Scrollbar>().size = oldScore / max;
            yield return new WaitForSeconds(showspeed);
        }
    }

    private IEnumerator execSkillofOnePerson(CharacterController characterObj,bool selected)
    {
        characterObj.executingSkill = true;
        float newscore = score;
        //パラメータスコア
        float parascore;
        ProgressModel characterInf = characterObj.characterInf;
        if (characterObj.area == "visual") parascore = characterInf.visual;
        else if (characterObj.area == "vocal") parascore = characterInf.vocal;
        else parascore = characterInf.dance;
        //アクティブスキル
        if (selected)
        {
            if (characterInf.activeSkillType == SkillType.Plus)
            {
                score += (characterInf.activeSkillScore * characterInf.activeSkillLevel);
            }
            else
            {
                parascore *= (characterInf.activeSkillScore * characterInf.activeSkillLevel);
            }
            CharacterModel upperInfo = Common.characters[characterInf.characterId];
            testText.text = upperInfo.name + " の " + upperInfo.skillname;
        }
        //パッシブスキル(後で実装)
        newscore += parascore;
        Debug.Log("ID:"+characterObj.id+"Old:" + score + " New:" + newscore + "Active:" + selected);
        yield return updateScoreBar(score, newscore);
        score = newscore;
        yield return new WaitForSeconds(1.0f);
        characterObj.executingSkill = false;
    }

    private IEnumerator execSkills()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        testText.text = "Running...";
        for (int i = 0; i < 5; i++)
        {
            yield return execSkillofOnePerson(objs[i].GetComponent<CharacterController>(),i==selectedcharacter);
        }
        remainingTurn--;
        objs[selectedcharacter].GetComponent<CharacterController>().finishSkill();
        if (remainingTurn > 0)
        {
            testText.text = "残りターン数:"+remainingTurn;
            executingSkills = false;
        }else if (score >= 80)
        {
            testText.text = "Mission Complete!";
        }
        else
        {
            testText.text = "Mission Failed!";
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
        if (Common.characters == null) Common.initCharacters();//Test Only
        testText.text = "残りターン数:" + 5;
        initLiveStage();
        light.SetActive(true);
        checkPos();
    }

    void Update()
    {
        if(!executingSkills)checkPos();
    }
}
