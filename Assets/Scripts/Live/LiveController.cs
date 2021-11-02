using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LiveController : MonoBehaviour
{
    public GameObject startbutton;
    public GameObject Background;
    public GameObject ResultUI;
    public GameObject sailium;
    public GameObject backlight;
    public GameObject curtain;

    public GameObject Heart;
    public GameObject scoreBar;
    
    int dance = 0;
    int visual = 0;
    int vocal = 0;
    public static int selectedcharacter;

    HashSet<GameObject> sailiumcollections = new HashSet<GameObject>();
    public Image Achievement;
    public GameObject Achieved;
    public Text RemainingText;
    public float max;
    public float score = 0;
    public int remainingTurn = 5;
    WaitForSeconds wait = new WaitForSeconds(0.01f);
    float maxscore = 0;
    int maxindex = 0;

    public GameObject[] LiveCharacter=new GameObject[5];
    public GameObject[] CharacterList=new GameObject[5];
    public GameObject[] passiveList = new GameObject[5];
    CharacterController[] characterControllers = new CharacterController[5];

    ProgressModel[] tempProgress = new ProgressModel[5];

    public GameObject backtext;

    public static bool executingSkills = false;

    bool achievedScore = false;

    void checkPos()
    {
        dance = 0;
        visual = 0;
        vocal = 0;
        bool active = false;
        GameObject[] objs = new GameObject[5];
        for (int i = 0; i < 5; i++) objs[i] = LiveCharacter[i];
        Array.Sort(objs, delegate (GameObject a1, GameObject a2) { return -1*a1.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y
            .CompareTo(a2.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y); });
        int depth = 0;
        for (int i=0;i<5;i++)
        {
            CharacterController c = characterControllers[i];
            string area = c.area;
            if (area == "dance") dance++;
            else if (area == "visual") visual++;
            else if (area == "vocal") vocal++;
            depth++;
            if (c.id == selectedcharacter)
            {
                c.light.SetActive(true);
                active = c.completedActiveSkill;
            }
            else
            {
                c.light.SetActive(false);
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
    Dictionary<string, AudioClip> seclips;

    void initLiveStage()
    {
        max = Common.GetLiveScoreMaxValue();
        int chapter = Common.mainstoryid[0] - '0';
        if (chapter >= 7)
        {
            Heart.SetActive(false);
            scoreBar.SetActive(false);
        }
        executingSkills = false;
        selectedcharacter = 0;
        GameObject[] objs = LiveCharacter;
        listchilds = CharacterList;
        if (Common.characters == null) Common.initCharacters();//Delete On Pro
        for (int i = 0; i < 5; i++)
        {
            CharacterModel mainCharacter = Common.characters[Common.progresses[i].MainCharacterId];
            CharacterModel subCharacter = Common.characters[Common.progresses[i].SupportCharacterId];
            tempProgress[i] = new ProgressModel();
            tempProgress[i].Name = Common.progresses[i].Name;
            tempProgress[i].MainCharacterId = Common.progresses[i].MainCharacterId;
            tempProgress[i].SupportCharacterId = Common.progresses[i].SupportCharacterId;
            tempProgress[i].Visual = Common.progresses[i].Visual;
            tempProgress[i].Vocal = Common.progresses[i].Vocal;
            tempProgress[i].Dance = Common.progresses[i].Dance;
            tempProgress[i].Group = mainCharacter.group;
            tempProgress[i].ActiveSkillLevel = Common.progresses[i].ActiveSkillLevel;
            tempProgress[i].PassiveSkillLevel = Common.progresses[i].PassiveSkillLevel;
            tempProgress[i].ActiveSkillName = mainCharacter.skillname;
            tempProgress[i].ActiveSkillParams = mainCharacter.activeparams;
            tempProgress[i].ActiveSkillScore = mainCharacter.activeskillscore;
            tempProgress[i].ActiveSkillType = mainCharacter.activetype;
            tempProgress[i].ActiveSkillDescription = mainCharacter.activedescription;
            tempProgress[i].BestSkill = mainCharacter.bestskill;
            tempProgress[i].SupportSkillName = subCharacter.skillname;
            tempProgress[i].PassiveSkillParams = subCharacter.passiveparams;
            tempProgress[i].PassiveSkillScore = subCharacter.passiveskillscore;
            tempProgress[i].PassiveSkillType = subCharacter.passivetype;
            tempProgress[i].PassiveSkillDescription = subCharacter.passivedescription;
            tempProgress[i].PassiveSkillProbability = subCharacter.passiveprobability;
            CharacterController objk = objs[i].GetComponent<CharacterController>();
            characterControllers[i] = objk;
#if UNITY_EDITOR
            Debug.Log("Group:"+tempProgress[i].Group);
            Debug.Log("PassiveType:" + tempProgress[i].PassiveSkillType);
#endif
            objk.id = i;
            objk.characterInf = tempProgress[i];
            objk.name.text = tempProgress[i].Name;
            for (int j=0;j<6;j++)
            {
                //Change Here
#if UNITY_ANDROID
                objk.gifsprite.Add(Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/Live/Gif/" + tempProgress[i].MainCharacterId + "/ch-" + j + ".png"));
#else
                objk.gifsprite.Add(Resources.Load<Sprite>("Images/Live/Gif/" + tempProgress[i].MainCharacterId + "/ch-" + j));
#endif
            }
            objk.initImage();
            if (i == 0) objk.SelectMe();
#if UNITY_ANDROID
            listchilds[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/charactericon/" + tempProgress[i].MainCharacterId + ".png");
            if (tempProgress[i].BestSkill == "vocal") listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Frame_Pink_Edge");
            else if (tempProgress[i].BestSkill == "visual") listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Frame_Yellow_Edge");
            else listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Frame_Blue_Edge");
#else
            listchilds[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + tempProgress[i].MainCharacterId);
            if (tempProgress[i].BestSkill == "vocal") listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Pink_Edge");
            else if (tempProgress[i].BestSkill == "visual") listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Yellow_Edge");
            else listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Blue_Edge");
#endif
            objk.listchild = listchilds[i];
            objk.connectUI();
            objk.setParamsFont();
        }
        float baseheight = Screen.height + 220f;
        int xins = 100;
        float scale = 1.0f;
        int layer = 0;
        for (int y = 80; y < 520; y += 100)
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
        seclips = new Dictionary<string, AudioClip>()
        {
#if UNITY_ANDROID
            {"gaugemax1", Common.assetBundle.LoadAsset<AudioClip>("gaugemax1") },
            {"up1", Common.assetBundle.LoadAsset<AudioClip>("up1") },
            {"resultsuccess1", Common.assetBundle.LoadAsset<AudioClip>("resultsuccess1") },
            {"resultfault1", Common.assetBundle.LoadAsset<AudioClip>("resultfault1") },
#else
            {"gaugemax1", Resources.Load<AudioClip>("SE/live/gaugemax1") },
            {"up1", Resources.Load<AudioClip>("SE/live/up1") },
            {"resultsuccess1", Resources.Load<AudioClip>("SE/live/resultsuccess1") },
            {"resultfault1", Resources.Load<AudioClip>("SE/live/resultfault1") },
#endif
        };
        enablePassives();
        backlight.transform.SetSiblingIndex(114514);
    }

    WaitForFixedUpdate fixedupdate = new WaitForFixedUpdate();

    private IEnumerator updateScoreBar(float oldScore, float newScore)
    {
        if (Achieved.active) yield break;
        System.Random random = new System.Random();
        float kb = (float)max / 60.0f;
        float k = 0;
        int plus = (int)max/100;
        
        while (oldScore < newScore)
        {
            oldScore+=plus;
            k+=plus;
            if (k>=kb&& sailiumcollections.Count>0)
            {
                GameObject gameObject = sailiumcollections.ElementAt(random.Next(sailiumcollections.Count));
                gameObject.GetComponent<Sailium>().enableSailium();
                sailiumcollections.Remove(gameObject);
                k = 0;
            }
            if(scoreBar.active)Achievement.fillAmount = oldScore / max;
            yield return fixedupdate;
        }
        if (newScore / max >= 1.0f && !achievedScore)
        {
            Common.subseplayer.PlayOneShot(seclips["gaugemax1"]);
            achievedScore = true;
            if (Heart.active&&!Achieved.active)
            {
                Achieved.SetActive(true);
            }
        }
    }


    private float ApplyActiveSkill(ProgressModel targetInf,ProgressModel characterInf)
    {
        float activescore = 0;
        float multiply = 0.5f;
        if (characterInf.ActiveSkillParams == "all")
        {
            activescore += (targetInf.Visual + targetInf.Vocal + targetInf.Dance) * characterInf.ActiveSkillScore * characterInf.ActiveSkillLevel * multiply;
        }
        else
        {
            if (characterInf.ActiveSkillParams.Contains("visual"))
            {
                activescore += (targetInf.Visual * characterInf.ActiveSkillScore * characterInf.ActiveSkillLevel) * multiply;

            }
            if (characterInf.ActiveSkillParams.Contains("vocal"))
            {
                activescore += (targetInf.Vocal * characterInf.ActiveSkillScore * characterInf.ActiveSkillLevel)* multiply;
            }
            if (characterInf.ActiveSkillParams.Contains("dance"))
            {
                activescore += (targetInf.Dance * characterInf.ActiveSkillScore * characterInf.ActiveSkillLevel) * multiply;
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
            GameObject[] objs = LiveCharacter;
            for (int i=0;i<5;i++)
            {
                CharacterController objinf = characterControllers[i];
                if (characterInf.ActiveSkillType == "samearea" && characterObj.area != objinf.area)
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
        GameObject[] objs = LiveCharacter;
        int count = 0;
        string targetGroup = "";
        if (characterInf.PassiveSkillType.Contains("group"))
        {
            targetGroup = characterInf.PassiveSkillType.Replace("group", "");
         }
        string targetArea = "";
        if(characterInf.PassiveSkillType == "visual"
            || characterInf.PassiveSkillType == "vocal"
            || characterInf.PassiveSkillType == "dance")
        {
            targetArea = characterInf.PassiveSkillType;
        }
#if UNITY_EDITOR
        Debug.Log("Passive:" + characterInf.SupportCharacterId + " Group:" + characterInf.Group + " SkillScore " + characterInf.PassiveSkillScore + " PassiveType:" + characterInf.PassiveSkillType + " PassiveScore:" + characterInf.PassiveSkillScore);
#endif
        for (int i = 0; i < 5;i++)
        {
            CharacterController objcc = characterControllers[i];
            ProgressModel objinf = objcc.characterInf;
            if (targetGroup != "" && targetGroup != objinf.Group)
            {
                continue;
            }
            if (targetArea != "" && targetArea != objcc.area)
            {
                continue;
            }
            count++;
            if (characterInf.PassiveSkillParams.Contains("visual") || characterInf.PassiveSkillParams == "all")
            {
                objcc.characterInf.Visual *= (1 + characterInf.PassiveSkillLevel * 0.2f) * characterInf.PassiveSkillScore;
            }
            if (characterInf.PassiveSkillParams.Contains("vocal") || characterInf.PassiveSkillParams == "all")
            {
                objcc.characterInf.Vocal *= (1 + characterInf.PassiveSkillLevel * 0.2f) * characterInf.PassiveSkillScore;
            }
            if (characterInf.PassiveSkillParams.Contains("dance") || characterInf.PassiveSkillParams == "all")
            {
                objcc.characterInf.Dance *= (1 + characterInf.PassiveSkillLevel * 0.2f) * characterInf.PassiveSkillScore;
            }
#if UNITY_EDITOR
            Debug.Log("Applied:" + objinf.MainCharacterId);
#endif
            objcc.setParamsFont();
        }
    }

    private IEnumerator execSkillofOnePerson(CharacterController characterObj,int index,bool selected)
    {
        characterObj.executingSkill = true;
        float newscore = score;
        ProgressModel characterInf = characterObj.characterInf;
       
        float parascore;
        float activeskillscore = 0;
        if (characterObj.area == "visual") parascore = characterInf.Visual;
        else if (characterObj.area == "vocal") parascore = characterInf.Vocal;
        else parascore = characterInf.Dance;
        //アクティブスキルの発動
        if (selected)
        {
            activeskillscore = execActiveSkill(characterObj);
            //testText.text = characterInf.Name + " の " + characterInf.ActiveSkillName;
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
        yield return characterObj.jump();
        yield return updateScoreBar(score, newscore);
        score = newscore;
        //yield return new WaitForSeconds(1.0f);
        characterObj.executingSkill = false;
    }

    private IEnumerator finishLive()
    {
        var fixedupdate = new WaitForFixedUpdate();
        RectTransform curatinrect = curtain.GetComponent<RectTransform>();
        for (int i = 71; i > 0; i-=2)
        {
            curatinrect.anchoredPosition = new Vector2(curatinrect.anchoredPosition.x, curatinrect.anchoredPosition.y - i);
            yield return fixedupdate;
        }
        if (score >= max)
        {
            //testText.text = "Mission Complete!";
            var resultchanger = ResultUI.GetComponent<UIchanger>();
            resultchanger.Judge_Image_num = 0;
            resultchanger.Chara_Image_num = characterControllers[maxindex].characterInf.MainCharacterId;
            resultchanger.Score_num = (int)score;
            resultchanger.Achievement_num = score / max;
            ResultUI.SetActive(true);
        }
        else
        {
            //testText.text = "Mission Failed!";
            var resultchanger = ResultUI.GetComponent<UIchanger>();
            resultchanger.Judge_Image_num = 1;
            resultchanger.Chara_Image_num = characterControllers[maxindex].characterInf.MainCharacterId;
            resultchanger.Score_num = (int)score;
            resultchanger.Achievement_num = score / max;
            ResultUI.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(1f);
        if (score >= max)
        {
            Common.subseplayer.PlayOneShot(seclips["resultsuccess1"]);
        }
        else
        {
            Common.subseplayer.PlayOneShot(seclips["resultfault1"]);
        }
        for (int i = 1;i <= 71; i+=2)
        {
            curatinrect.anchoredPosition = new Vector2(curatinrect.anchoredPosition.x, curatinrect.anchoredPosition.y + i);
            yield return fixedupdate;
        }
    }

    private void enablePassives()
    {
        for (int i = 0; i < 5; i++)
        {
            if (RandomArray.Probability(tempProgress[i].PassiveSkillProbability / 2 * tempProgress[i].PassiveSkillLevel * 100.0f))
            {
                passiveList[i].SetActive(true);
                execPassiveSkill(characterControllers[i]);
            }
            else
            {
                passiveList[i].SetActive(false);
            }
        }
    }

    private IEnumerator execSkills()
    {
        backtext.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            yield return execSkillofOnePerson(characterControllers[i],i,i==selectedcharacter);
        }
        for (int i = 0; i < 5; i++)
        {
            characterControllers[i].characterInf.Visual = Common.progresses[i].Visual;
            characterControllers[i].characterInf.Vocal = Common.progresses[i].Vocal;
            characterControllers[i].characterInf.Dance = Common.progresses[i].Dance;
            characterControllers[i].setParamsFont();
        }
        remainingTurn--;
        characterControllers[selectedcharacter].finishSkill();
        RemainingText.text = remainingTurn.ToString();
        executingSkills = false;
        backtext.SetActive(true);
        if (remainingTurn == 0)
        {
            yield return finishLive();
        }
        else
        {
            enablePassives();
        }
#if UNITY_EDITOR
        Debug.Log("CurrentScore:"+score+" Max:"+max);
#endif
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
        initLiveStage();
        checkPos();
    }

    bool triggeredPlayer = false;
    void Update()
    {
        if(!executingSkills)checkPos();
        if (!triggeredPlayer && SceneManager.GetActiveScene().name == "Live")
        {
            String filename = "TM01";
            int storyid = int.Parse(Common.mainstoryid.Substring(0, 1));
            if (storyid == 4 || storyid == 7) filename = "TM02";
#if UNITY_ANDROID
            Common.bgmplayer.clip = Common.assetBundle.LoadAsset<AudioClip>(filename);
#else
            Common.bgmplayer.clip = Resources.Load<AudioClip>("Music/" + filename);
#endif
            Common.bgmplayer.Play();
            triggeredPlayer = true;
        }
    }
}
