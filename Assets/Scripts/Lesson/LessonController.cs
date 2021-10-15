using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LessonController : MonoBehaviour
{
    public GameObject startbutton;
    public GameObject Background;
    public GameObject backlight;
    int dance = 0;
    int visual = 0;
    int vocal = 0;
    public static int selectedcharacter;
    public ProgressModel[] characters;
    public int remainingTurn = 5;
    public Text remainingText;
    WaitForSeconds wait = new WaitForSeconds(0.01f);
    public static bool executingSkills = false;
    TeacherController teacher;

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
        for (int i=0;i<6;i++)
        {
            if (objs[i].name=="TeacherCharacter") continue;
            string area = objs[i].GetComponent<LessonCharacterController>().area;
            if (area == "dance") dance++;
            else if (area == "visual") visual++;
            else if (area == "vocal") vocal++;
            depth++;
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
        if (Common.characters == null) Common.initCharacters();//Delete On Pro
        for (int i = 0; i < 5; i++)
        {
            CharacterModel mainCharacter = Common.characters[characters[i].MainCharacterId];
            CharacterModel subCharacter = Common.characters[characters[i].SupportCharacterId];
            characters[i].ActiveSkillName = mainCharacter.skillname;
            characters[i].ActiveSkillParams = mainCharacter.activeparams;
            characters[i].ActiveSkillScore = mainCharacter.activeskillscore;
            characters[i].ActiveSkillType = mainCharacter.activetype;
            characters[i].ActiveSkillDescription = mainCharacter.activedescription;
            characters[i].BestSkill = mainCharacter.bestskill;
            characters[i].SupportSkillName = subCharacter.skillname;
            characters[i].PassiveSkillParams = subCharacter.passiveparams;
            characters[i].PassiveSkillScore = subCharacter.passiveskillscore;
            characters[i].PassiveSkillType = subCharacter.passivetype;
            characters[i].PassiveSkillDescription = subCharacter.passivedescription;
            Common.progresses[i] = characters[i];
            LessonCharacterController objk = objs[i].GetComponent<LessonCharacterController>();
            objk.id = i;
            objk.name.text = characters[i].Name;
            for (int j=0;j<6;j++)
            {
                objk.gifsprite.Add(Resources.Load<Sprite>("Images/Live/Gif/"+mainCharacter.id+"/ch-"+j));
            }
            objk.initImage();
            if (i == 0) objk.SelectMe();
            listchilds[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + characters[i].MainCharacterId);
            if (characters[i].BestSkill == "vocal") listchilds[i].transform.parent.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Pink_Edge");
            else if (characters[i].BestSkill == "visual") listchilds[i].transform.parent.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Yellow_Edge");
            else listchilds[i].transform.parent.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Frame_Blue_Edge");
            objs[i].GetComponent<LessonCharacterController>().listchild = listchilds[i];
            objk.SetupStar();
            objk.setParams();
        }
        //For Test
        DendouModel dendouModel = new DendouModel();
        dendouModel.MainCharacterId = 4;
        dendouModel.SupportCharacterId = 4;
        dendouModel.Name = Common.characters[4].name;
        dendouModel.Vocal = Common.characters[4].vocal;
        dendouModel.Visual = Common.characters[4].visual;
        dendouModel.Dance = Common.characters[4].dance;
        //Init teacher
        teacher = objs[5].GetComponent<TeacherController>();
        teacher.name.text = dendouModel.Name;
        teacher.initPos();
        for (int j = 0; j < 6; j++)
        {
            teacher.gifsprite.Add(Resources.Load<Sprite>("Images/Live/Gif/" + dendouModel.MainCharacterId + "/ch-" + j));
        }
        teacher.initImage();
        backlight.transform.SetSiblingIndex(114514);
    }

    

    private IEnumerator execSkillofOnePerson(LessonCharacterController characterObj,int index)
    {
        characterObj.executingSkill = true;
        int score = 0;
        if(characterObj.area==teacher.area)score = RandomArray.GetRandom(new int[] { 0,1,1,1,2,2,2,3,3,3 });
        else score = RandomArray.GetRandom(new int[] { 0, 1, 2 });
        if (characterObj.area == "visual") Common.progresses[index].Visual+=score;
        else if (characterObj.area == "vocal") Common.progresses[index].Vocal += score;
        else Common.progresses[index].Dance += score;
        yield return null;
        characterObj.setParams();
        characterObj.executingSkill = false;
    }

    private IEnumerator execSkills()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LiveCharacter");
        for (int i = 0; i < 5; i++)
        {
            yield return execSkillofOnePerson(objs[i].GetComponent<LessonCharacterController>(),i);
        }
        remainingTurn--;
        remainingText.text = remainingTurn.ToString();
        if (remainingTurn > 0)
        {
            executingSkills = false;
        }
        else
        {
            //Change Scene
            Common.loadingCanvas.SetActive(true);
            Manager.manager.StateQueue((int)gamestate.Story);
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
        initLiveStage();
        checkPos();
    }

    void Update()
    {
        if(!executingSkills)checkPos();
    }
}
