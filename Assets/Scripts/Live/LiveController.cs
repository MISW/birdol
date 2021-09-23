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
    public GameObject backlight;
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

    float maxscore = 0;
    int maxindex = 0;

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
            CharacterController objk = objs[i].GetComponent<CharacterController>();
            objk.id = i;
            objk.characterInf = characters[i];
            objk.name.text = characters[i].Name;
            objk.initImage();
            listchilds[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + characters[i].MainCharacterId);
            if (characters[i].BestSkill == "vocal") listchilds[i].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Pink_Edge");
            else if (characters[i].BestSkill == "visual") listchilds[i].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Yellow_Edge");
            else listchilds[i].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Blue_Edge");
            objs[i].GetComponent<CharacterController>().listchild = listchilds[i];
            objk.setParamsFont();

        }
        float baseheight = Screen.height + 220f;
        int xins = 90;
        float scale = 1.0f;
        int layer = 0;
        for (int y = 60; y < 480; y += 80)
        {
            for (int x = -270; x <= 270; x += xins)
            {
                GameObject instance = Instantiate(sailium, Background.transform);
                instance.transform.parent = Background.transform;
                instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(x,y);
                instance.GetComponent<Sailium>().layer = layer;
                instance.transform.localScale *= scale;
                sailiumcollections.Add(instance);
            }
            layer++;
            xins -= 10;
            scale -= 0.1f;
        }
        backlight.transform.SetSiblingIndex(114514);
    }

    private IEnumerator updateScoreBar(float oldScore, float newScore)
    {
        System.Random random = new System.Random();
        float kb = (float)max / 60.0f;
        float k = 0;
        while (oldScore < newScore)
        {
            oldScore++;
            k++;
            if (k>=kb&& sailiumcollections.Count>0)
            {
                GameObject gameObject = sailiumcollections.ElementAt(random.Next(sailiumcollections.Count));
                gameObject.GetComponent<Sailium>().enableSailium();
                sailiumcollections.Remove(gameObject);
                k = 0;
            }
            ScoreBar.GetComponent<Scrollbar>().size = oldScore / max;
            yield return new WaitForSeconds(showspeed);
        }
    }


    private float ApplyActiveSkill(ProgressModel targetInf,ProgressModel characterInf)
    {
        float activescore = 0;
        if (characterInf.ActiveSkillParams == "all")
        {
            activescore += (targetInf.Visual + targetInf.Vocal + targetInf.Dance) * characterInf.ActiveSkillScore;
        }
        else
        {
            if (characterInf.ActiveSkillParams.Contains("visual"))
            {
                activescore += (targetInf.Visual * characterInf.ActiveSkillScore);

            }
            if (characterInf.ActiveSkillParams.Contains("vocal"))
            {
                activescore += (targetInf.Vocal * characterInf.ActiveSkillScore);
            }
            if (characterInf.ActiveSkillParams.Contains("dance"))
            {
                activescore += (targetInf.Dance * characterInf.ActiveSkillScore);
            }
        }
        return activescore;
    }
    private float execActiveSkill(CharacterController characterObj)
    {
        float activescore = 0;
        ProgressModel characterInf = characterObj.characterInf;
        int count = 0;
        if(characterInf.ActiveSkillType == "self")
        {
            activescore += ApplyActiveSkill(characterInf,characterInf);
        }
        else
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
            foreach(GameObject obj in objs)
            {
                CharacterController objinf = obj.GetComponent<CharacterController>();
                if (characterInf.ActiveSkillType=="samearea"&&characterObj.area != objinf.area)
                {
                    continue;
                }
                count++;
                activescore += ApplyActiveSkill(objinf.characterInf, characterInf);
            }
        }
        return activescore;
    }

    private void execPassiveSkill(CharacterController characterObj)
    {
        ProgressModel characterInf = characterObj.characterInf;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        int count = 0;
        for(int i = 0; i < 5;i++)
        {
            CharacterController objcc = objs[i].GetComponent<CharacterController>();
            ProgressModel objinf = objcc.characterInf;
            if ((characterInf.PassiveSkillType.Contains("group")&&characterInf.Group!=objinf.Group) || 
                (characterInf.PassiveSkillType != "all" && !characterInf.PassiveSkillType.Contains("group") && objcc.area != characterObj.area))
            {
                continue;
            }
            count++;
            if (characterInf.PassiveSkillParams.Contains("visual") || characterInf.PassiveSkillParams == "all")
            {
                objinf.Visual *= characterInf.PassiveSkillScore;
            }
            if (characterInf.PassiveSkillParams.Contains("vocal") || characterInf.PassiveSkillParams == "all")
            {
                objinf.Vocal *= characterInf.PassiveSkillScore;
            }
            if (characterInf.PassiveSkillParams.Contains("dance") || characterInf.PassiveSkillParams == "all")
            {
                objinf.Dance *= characterInf.PassiveSkillScore;
            }
            objcc.setParamsFont();
        }
        Debug.Log("Triggered:"+count);
    }

    private IEnumerator execSkillofOnePerson(CharacterController characterObj,int index,bool selected)
    {
        characterObj.executingSkill = true;
        float newscore = score;
        //�p�����[�^�X�R�A
        //パッシブスキルの発動
        ProgressModel characterInf = characterObj.characterInf;
        if (RandomArray.Probability(characterInf.PassiveSkillProbability*100.0f))
        {
            listchilds[index].transform.GetChild(1).gameObject.SetActive(true);
            execPassiveSkill(characterObj);
        }
        else
        {
            listchilds[index].transform.GetChild(1).gameObject.SetActive(false);
        }
        float parascore;
        float activeskillscore = 0;
        if (characterObj.area == "visual") parascore = characterInf.Visual;
        else if (characterObj.area == "vocal") parascore = characterInf.Vocal;
        else parascore = characterInf.Dance;
        //アクティブスキルの発動
        if (selected)
        {
            activeskillscore = execActiveSkill(characterObj);
            testText.text = characterInf.Name + " の " + characterInf.ActiveSkillName;
            //listchilds[index].transform.GetChild(0).gameObject.SetActive(true);
        }
        //スコアの更新
        characterObj.score += parascore + activeskillscore;
        if (characterObj.score > maxscore)
        {
            maxscore = characterObj.score;
            maxindex = index;
        }
        newscore += (parascore+activeskillscore);
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
            resultchanger.Chara_Image_num = GameObject.FindGameObjectsWithTag("LiveCharacter")[maxindex].GetComponent<CharacterController>().characterInf.MainCharacterId;
            resultchanger.Score_num = (int)score;
            resultchanger.Achievement_num = score / max;
            ResultUI.SetActive(true);
        }
        else
        {
            //testText.text = "Mission Failed!";
            var resultchanger = ResultUI.GetComponent<UIchanger>();
            resultchanger.Judge_Image_num = 1;
            resultchanger.Chara_Image_num = GameObject.FindGameObjectsWithTag("LiveCharacter")[maxindex].GetComponent<CharacterController>().characterInf.MainCharacterId;
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
