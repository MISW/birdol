using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FreeLiveController : MonoBehaviour
{
    public GameObject startbutton;
    public GameObject Background;
    public GameObject ResultUI;
    public GameObject sailium;
    public GameObject backlight;
    public GameObject curtain;
    int dance = 0;
    int visual = 0;
    int vocal = 0;
    public static int selectedcharacter;

    HashSet<GameObject> sailiumcollections = new HashSet<GameObject>();
    public Text RemainingText;
    float max = 1600;
    public float score = 0;
    public int remainingTurn = 5;
    WaitForSeconds wait = new WaitForSeconds(0.01f);
    float maxscore = 0;
    int maxindex = 0;

    public GameObject[] LiveCharacter=new GameObject[5];
    public GameObject[] CharacterList=new GameObject[5];
    public GameObject[] passiveList = new GameObject[5];
    public GameObject[] CharacterBase = new GameObject[5];
    FreeCharacterController[] characterControllers = new FreeCharacterController[5];

    ProgressModel[] tempProgress = new ProgressModel[5];

    public GameObject backtext;

    public static bool executingSkills = false;

    public static List<ProgressModel> backup = new List<ProgressModel>();
    int SIZE;

    void checkPos()
    {
        dance = 0;
        visual = 0;
        vocal = 0;
        bool active = false;
        GameObject[] objs = new GameObject[SIZE];
        for (int i = 0; i < SIZE; i++) objs[i] = LiveCharacter[i];
        Array.Sort(objs, delegate (GameObject a1, GameObject a2) { return -1*a1.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y
            .CompareTo(a2.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y); });
        int depth = 0;
        for (int i=0;i< SIZE; i++)
        {
            FreeCharacterController c = characterControllers[i];
            string area = c.area;
            if (area == "dance") dance++;
            else if (area == "visual") visual++;
            else if (area == "vocal") vocal++;
            depth++;
            if (c.id == selectedcharacter)
            {
                c.light.SetActive(true);
            }
            else
            {
                c.light.SetActive(false);
            }
            objs[i].transform.parent.gameObject.transform.SetSiblingIndex(i);
        }
        if ((dance <= 2 && visual <= 2 && vocal <= 2))
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
        GameObject[] objs = LiveCharacter;
        listchilds = CharacterList;
        if (Common.characters == null) Common.initCharacters();//Delete On Pro
        for (int i = 0; i < 5; i++)
        {
            if (i<SIZE)
            {
                CharacterModel mainCharacter = Common.characters[tempProgress[i].MainCharacterId];
                CharacterModel subCharacter = Common.characters[tempProgress[i].SupportCharacterId];
                tempProgress[i].ActiveSkillName = mainCharacter.skillname;
                tempProgress[i].ActiveSkillParams = mainCharacter.activeparams;
                tempProgress[i].ActiveSkillScore = mainCharacter.activeskillscore;
                tempProgress[i].ActiveSkillType = mainCharacter.activetype;
                tempProgress[i].ActiveSkillDescription = mainCharacter.activedescription;
                tempProgress[i].BestSkill = mainCharacter.bestskill;
                tempProgress[i].Group = mainCharacter.group;
                tempProgress[i].SupportSkillName = subCharacter.skillname;
                tempProgress[i].PassiveSkillParams = subCharacter.passiveparams;
                tempProgress[i].PassiveSkillScore = subCharacter.passiveskillscore;
                tempProgress[i].PassiveSkillType = subCharacter.passivetype;
                tempProgress[i].PassiveSkillDescription = subCharacter.passivedescription;
                tempProgress[i].PassiveSkillProbability = subCharacter.passiveprobability;
                FreeCharacterController objk = objs[i].GetComponent<FreeCharacterController>();
                characterControllers[i] = objk;
                objk.id = i;
                objk.characterInf = tempProgress[i];
                objk.name.text = tempProgress[i].Name;
                for (int j = 0; j < 6; j++)
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
                if (tempProgress[i].BestSkill == "vocal") listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/Live/Frame_Pink_Edge.png");
                else if (tempProgress[i].BestSkill == "visual") listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/Live/Frame_Yellow_Edge.png");
                else listchilds[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/Live/Frame_Blue_Edge.png");
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
            else
            {
                CharacterBase[i].SetActive(false);
                listchilds[i].SetActive(false);
            }
            
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
        enablePassives();
        backlight.transform.SetSiblingIndex(114514);
    }

    WaitForFixedUpdate fixedupdate = new WaitForFixedUpdate();

    private IEnumerator updateScoreBar(float oldScore, float newScore)
    {
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
            yield return fixedupdate;
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
                activescore += (targetInf.Vocal * characterInf.ActiveSkillScore * characterInf.ActiveSkillLevel) * multiply;
            }
            if (characterInf.ActiveSkillParams.Contains("dance"))
            {
                activescore += (targetInf.Dance * characterInf.ActiveSkillScore * characterInf.ActiveSkillLevel) * multiply;
            }
        }
        return activescore;
    }
    private float execActiveSkill(FreeCharacterController characterObj)
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
            for (int i=0;i< SIZE; i++)
            {
                FreeCharacterController objinf = characterControllers[i];
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

    private void execPassiveSkill(FreeCharacterController characterObj)
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
        if (characterInf.PassiveSkillType == "visual"
            || characterInf.PassiveSkillType == "vocal"
            || characterInf.PassiveSkillType == "dance")
        {
            targetArea = characterInf.PassiveSkillType;
        }
#if UNITY_EDITOR
        Debug.Log("Passive:" + characterInf.SupportCharacterId + " Group:" + characterInf.Group + " SkillScore " + " PassiveType:" + characterInf.PassiveSkillProbability + "PassiveScore:"+characterInf.PassiveSkillScore);
#endif
        for (int i = 0; i < SIZE; i++)
        {
            FreeCharacterController objcc = characterControllers[i];
            if (targetGroup != "" && targetGroup != objcc.characterInf.Group)
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
                objcc.characterInf.Visual *= (1 + characterInf.PassiveSkillLevel * 0.2f) * characterInf.PassiveSkillScore ;
            }
            if (characterInf.PassiveSkillParams.Contains("vocal") || characterInf.PassiveSkillParams == "all")
            {
                objcc.characterInf.Vocal *= (1 +  characterInf.PassiveSkillLevel * 0.2f) * characterInf.PassiveSkillScore;
            }
            if (characterInf.PassiveSkillParams.Contains("dance") || characterInf.PassiveSkillParams == "all")
            {
                objcc.characterInf.Dance *= (1 + characterInf.PassiveSkillLevel * 0.2f) * characterInf.PassiveSkillScore;
            }
            objcc.setParamsFont();
        }
#if UNITY_EDITOR
        Debug.Log("Triggered:"+count);
#endif
    }

    private IEnumerator execSkillofOnePerson(FreeCharacterController characterObj,int index,bool selected)
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
        if (selected&&!characterObj.completedActiveSkill)
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
        var resultchanger = ResultUI.GetComponent<UIchanger>();
        resultchanger.Chara_Image_num = characterControllers[maxindex].characterInf.MainCharacterId;
        resultchanger.Score_num = (int)score;
        ResultUI.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        for (int i = 1;i <= 71; i+=2)
        {
            curatinrect.anchoredPosition = new Vector2(curatinrect.anchoredPosition.x, curatinrect.anchoredPosition.y + i);
            yield return fixedupdate;
        }
    }

    private IEnumerator execSkills()
    {
        backtext.SetActive(false);
        for (int i = 0; i < SIZE; i++)
        {
            yield return execSkillofOnePerson(characterControllers[i],i,i==selectedcharacter);
        }
        for (int i = 0; i < SIZE; i++)
        {
            characterControllers[i].characterInf.Visual = backup[i].Visual;
            characterControllers[i].characterInf.Vocal = backup[i].Vocal;
            characterControllers[i].characterInf.Dance = backup[i].Dance;
            characterControllers[i].setParamsFont();
        }
        remainingTurn--;
        if(!characterControllers[selectedcharacter].completedActiveSkill) characterControllers[selectedcharacter].finishSkill();
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

    private void enablePassives()
    {
        for (int i = 0; i < SIZE; i++)
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

    public void onStartButtonClick()
    {
        executingSkills = true;
        startbutton.SetActive(false);
        StartCoroutine(execSkills());
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        SIZE = backup.Count;
        for(int i = 0; i < SIZE; i++)
        {
            tempProgress[i] = new ProgressModel();
            tempProgress[i].MainCharacterId = backup[i].MainCharacterId;
            tempProgress[i].SupportCharacterId = backup[i].SupportCharacterId;
            tempProgress[i].Visual = backup[i].Visual;
            tempProgress[i].Vocal= backup[i].Vocal;
            tempProgress[i].Dance = backup[i].Dance;
            tempProgress[i].ActiveSkillLevel = backup[i].ActiveSkillLevel;
            tempProgress[i].PassiveSkillLevel = backup[i].PassiveSkillLevel;
        }
        if (Common.characters == null) Common.initCharacters();//Test Only
        initLiveStage();
        checkPos();
    }

    bool triggeredPlayer = false;
    void Update()
    {
        if(!executingSkills)checkPos();
        if (!triggeredPlayer && SceneManager.GetActiveScene().name == "FreeLive")
        {
#if UNITY_ANDROID
            Common.bgmplayer.clip = Common.assetBundle.LoadAsset<AudioClip>(Common.Freebgm);
#else
            Common.bgmplayer.clip = Resources.Load<AudioClip>("Music/"+Common.Freebgm);
#endif
            Common.bgmplayer.Play();
            triggeredPlayer = true;
        }
    }
}
