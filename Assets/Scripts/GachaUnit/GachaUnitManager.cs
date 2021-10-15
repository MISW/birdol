using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class UnitInf
{
    public string unitname = "";
    public int mainselected = -1;
    public int subselected = -1;
}

public class GachaUnitManager : MonoBehaviour
{
    public GameObject teacherPage;
    public GameObject mainPage;
    public GameObject characterPage;
    public GameObject characterTeamName;
    public GameObject teacherTeamName;
    public GameObject teacherFirst;
    public GameObject errorDialog;

    public GameObject[] pairLists;

    public static int[] initid;
    public static List<DendouModel> teachers = new List<DendouModel>();

    public CharacterModel[] characters=new CharacterModel[10];
    UnitInf[] unitInf = new UnitInf[5];
    bool[] selected = new bool[10];
    public GameObject[] charcterIcons;
    List<GameObject> teacherObjects=new List<GameObject>();

    string currentdialog = "";
    int currentindex = -1;
    int currentcharacter = -1;
    int currentteacher = -1;
    // Start is called before the first frame update
   
    void Start()
    {
        mainPage.transform.SetSiblingIndex(2);
        for (int i=0;i<5;i++)
        {
            unitInf[i] = new UnitInf();
        }
        for (int i = 0; i < 10; i++) {
            characters[i] = Common.characters[initid[i]];
            charcterIcons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + characters[i].id);
        }
        bool teacherinited = false;
        foreach (DendouModel dendouModel in teachers)
        {
            GameObject teacherChild = teacherFirst;
            if (teacherinited)
            {
                teacherChild = Instantiate(teacherFirst, teacherFirst.transform.parent);
            }
            else
            {
                teacherinited = true;
            }
            teacherChild.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + dendouModel.MainCharacterId);
            teacherChild.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + dendouModel.SupportCharacterId);
            teacherChild.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = dendouModel.Vocal.ToString();
            teacherChild.transform.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = dendouModel.Visual.ToString();
            teacherChild.transform.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = dendouModel.Dance.ToString();
            teacherObjects.Add(teacherChild);
        }


    }

    public void OpenMainCharacterPage(int index)
    {
        Transform param = characterPage.transform.GetChild(4);
        currentdialog = "maincharacter";
        for (int i=0;i<10;i++)
        {
            if (selected[i])
            {
                if (unitInf[index].mainselected == i)
                {
                    charcterIcons[i].GetComponent<Image>().color = new Color(255f, 255f, 255f);
                    charcterIcons[i].transform.GetChild(0).gameObject.SetActive(true);
                    currentcharacter = i;
                    SetCharacterProfile(i);
                }
                else
                {
                    charcterIcons[i].GetComponent<Image>().color = new Color(125f / 255f, 125f / 255f, 125f / 255f);
                    charcterIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                charcterIcons[i].GetComponent<Image>().color = new Color(255f, 255f, 255f);
                charcterIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        for(int i = 0; i < 3; i++)
        {
            param.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        currentindex = index;
        characterTeamName.GetComponent<InputField>().interactable = true;
        characterTeamName.GetComponent<InputField>().text = unitInf[index].unitname;
        characterPage.transform.SetSiblingIndex(2);
    }

    public void OpenSupportCharacterPage(int index)
    {
        Transform param = characterPage.transform.GetChild(4);
        currentdialog = "supportcharacter";
        for (int i = 0; i < 10; i++)
        {
            if (selected[i])
            {
                if (unitInf[index].subselected == i)
                {
                    charcterIcons[i].GetComponent<Image>().color = new Color(255f, 255f, 255f);
                    charcterIcons[i].transform.GetChild(0).gameObject.SetActive(true);
                    currentcharacter = i;
                    SetCharacterProfile(i);
                }
                else
                {
                    charcterIcons[i].GetComponent<Image>().color = new Color(125f / 255f, 125f / 255f, 125f / 255f);
                    charcterIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                charcterIcons[i].GetComponent<Image>().color = new Color(255f, 255f, 255f);
                charcterIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            param.GetChild(i).GetChild(1).gameObject.SetActive(true);
        }
        currentindex = index;
        characterTeamName.GetComponent<InputField>().interactable = false;
        characterTeamName.GetComponent<InputField>().text = unitInf[index].unitname;
        characterPage.transform.SetSiblingIndex(2);
    }

    public void SetCharacterProfile(int index)
    {
        Transform transform = characterPage.transform;
        if (index == -1)
        {
            transform.GetChild(2).gameObject.GetComponent<Image>().sprite = null;
            transform.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = "";
            transform.GetChild(4).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "";
            transform.GetChild(4).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "";
            transform.GetChild(4).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
        }
        else
        {
            transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + characters[index].id);//SelectedIcon
            transform.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = $"【{characters[index].skillname}】\n";
            if (currentdialog == "maincharacter")
            {
                transform.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text += characters[index].activedescription;//Description
            }
            else if (currentdialog == "supportcharacter")
            {
                transform.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text += characters[index].passivedescription;//Description
            }
            //Params
            transform.GetChild(4).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = characters[index].vocal.ToString();
            transform.GetChild(4).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = characters[index].visual.ToString();
            transform.GetChild(4).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = characters[index].dance.ToString();
        }
        
    }

    public void SelectCharacter(int index)
    {
        if (!selected[index])
        {
            if(currentcharacter != -1)
            {
                charcterIcons[currentcharacter].transform.GetChild(0).gameObject.SetActive(false);
            }
            charcterIcons[index].transform.GetChild(0).gameObject.SetActive(true);
            currentcharacter = index;
            SetCharacterProfile(index);
        }
    }

    public void SelectTeacher(Button button)
    {
        if (currentteacher != -1) teacherObjects[currentteacher].transform.GetChild(0).gameObject.SetActive(false);
        currentteacher = button.transform.GetSiblingIndex();
        teacherTeamName.GetComponent<InputField>().text = teachers[currentteacher].Name;
        teacherObjects[currentteacher].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OpenTeacherPage()
    {
        currentdialog = "teacher";
        teacherTeamName.GetComponent<InputField>().text = (currentteacher != -1) ? teachers[currentteacher].Name : "";
        if(currentteacher >= 0)teacherObjects[currentteacher].transform.GetChild(0).gameObject.SetActive(true);
        teacherPage.transform.SetSiblingIndex(2);
    }

    public void CompleteCharacterSelect()
    {
        Transform pairList = pairLists[currentindex].transform;
        if (currentdialog == "maincharacter" && currentcharacter != -1)
        {
            if(unitInf[currentindex].mainselected!=-1)selected[unitInf[currentindex].mainselected] = false;
            unitInf[currentindex].mainselected = currentcharacter;
            unitInf[currentindex].unitname = characterTeamName.GetComponent<InputField>().text;
            selected[currentcharacter] = true;
            pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + characters[currentcharacter].id);
            pairList.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = unitInf[currentindex].unitname;
            pairList.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = characters[currentcharacter].vocal.ToString();
            pairList.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = characters[currentcharacter].visual.ToString();
            pairList.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = characters[currentcharacter].dance.ToString();
        }
        else if (currentdialog == "supportcharacter"&& currentcharacter!=-1)
        {
            if (unitInf[currentindex].subselected != -1) selected[unitInf[currentindex].subselected] = false;
            unitInf[currentindex].subselected = currentcharacter;
            selected[currentcharacter] = true;
            float sp = 0.5f;
            pairList.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + characters[currentcharacter].id);
            string newvocal = string.Format("{0:F1}", (float)characters[currentcharacter].vocal * sp);
            string newvisual = string.Format("{0:F1}", (float)characters[currentcharacter].visual * sp);
            string newdance = string.Format("{0:F1}", (float)characters[currentcharacter].dance * sp);
            pairList.GetChild(3).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"<size=30>{newvocal.Substring(0,1)}</size><size=20>{newvocal.Substring(1)}</size>";
            pairList.GetChild(3).GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"<size=30>{newvisual.Substring(0, 1)}</size><size=20>{newvisual.Substring(1)}</size>";
            pairList.GetChild(3).GetChild(2).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"<size=30>{newdance.Substring(0, 1)}</size><size=20>{newdance.Substring(1)}</size>";
        }
        SetCharacterProfile(-1);
        characterTeamName.GetComponent<InputField>().interactable = false;
        Debug.Log("SavedMain:" + unitInf[currentindex].mainselected);
        currentdialog = "";
        currentindex = -1;
        currentcharacter = -1;
        mainPage.transform.SetSiblingIndex(5);
    }

    public void CompleteTeacherSelect()
    {
        Transform pairList = pairLists[5].transform;
        if (currentteacher != -1) teacherObjects[currentteacher].transform.GetChild(0).gameObject.SetActive(false);
        pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + teachers[currentteacher].MainCharacterId);
        pairList.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + teachers[currentteacher].MainCharacterId);
        pairList.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = teachers[currentteacher].Name;
        pairList.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = teachers[currentteacher].Vocal.ToString();
        pairList.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = teachers[currentteacher].Visual.ToString();
        pairList.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = teachers[currentteacher].Dance.ToString();
        currentdialog = "";
        mainPage.transform.SetSiblingIndex(5);
    }

    public void ClearCharacterSelect()
    {
        Transform pairList = pairLists[currentindex].transform;
        if (currentdialog == "maincharacter" && currentcharacter != -1)
        {
            if (unitInf[currentindex].mainselected != -1)
            {
                selected[unitInf[currentindex].mainselected] = false;
                unitInf[currentindex].mainselected = -1;
            }
            pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gachaunit/main");
            pairList.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "";
            pairList.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "";
            pairList.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
        }
        else if (currentdialog == "supportcharacter" && currentcharacter != -1)
        {
            if (unitInf[currentindex].subselected != -1)
            {
                selected[unitInf[currentindex].subselected] = false;
                unitInf[currentindex].subselected = -1;
            }
            pairList.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gachaunit/support");
            pairList.GetChild(3).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            pairList.GetChild(3).GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            pairList.GetChild(3).GetChild(2).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }
        SetCharacterProfile(-1);
        currentdialog = "";
        currentindex = -1;
        currentcharacter = -1;
        mainPage.transform.SetSiblingIndex(5);
    }

    public void ClearTeacherSelect()
    {
        Transform pairList = pairLists[5].transform;
        if (currentteacher != -1) teacherObjects[currentteacher].transform.GetChild(0).gameObject.SetActive(false);
        pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gachaunit/teacher");
        pairList.GetChild(1).gameObject.GetComponent<Image>().sprite = null;
        pairList.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
        pairList.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "";
        pairList.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "";
        pairList.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
        currentdialog = "";
        currentteacher = -1;
        mainPage.transform.SetSiblingIndex(5);
    }

    private IEnumerator showError(string error)
    {
        errorDialog.SetActive(true);
        errorDialog.transform.GetChild(0).gameObject.GetComponent<Text>().text = error;
        yield return new WaitForSeconds(2);
        errorDialog.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        errorDialog.SetActive(false);
    }

    public void StartIkusei()
    {
        Common.progresses = new ProgressModel[5];
        for (int i = 0; i < 5; i++)
        {
            if (unitInf[i].mainselected == -1)
            {
                StartCoroutine(showError("メインキャラクターが設定されていないユニットがあります!"));
                return;
            }
            if (unitInf[i].subselected == -1)
            {
                StartCoroutine(showError("サポートキャラクターが設定されていないユニットがあります!"));
                return;
            }
            if (unitInf[i].unitname == "")
            {
                StartCoroutine(showError("編成名が設定されていないユニットがあります!"));
                return;
            }
            float sp = 0.5f;
            Common.progresses[i] = new ProgressModel();
            Common.progresses[i].MainCharacterId = characters[unitInf[i].mainselected].id;
            Common.progresses[i].Name = unitInf[i].unitname;
            Common.progresses[i].Vocal = characters[unitInf[i].mainselected].vocal + characters[unitInf[i].subselected].vocal * sp;
            Common.progresses[i].Visual = characters[unitInf[i].mainselected].visual + characters[unitInf[i].subselected].visual * sp;
            Common.progresses[i].Dance = characters[unitInf[i].mainselected].dance + characters[unitInf[i].subselected].dance * sp;
            Common.progresses[i].ActiveSkillLevel = 1;
            //Common.progresses[i].ActiveSkillType = characters[unitInf[i].mainselected].activetype;
            Common.progresses[i].ActiveSkillScore = characters[unitInf[i].mainselected].activeskillscore;
            Common.progresses[i].SupportCharacterId = characters[unitInf[i].subselected].id;
            Common.progresses[i].PassiveSkillLevel = 1;
            //Common.progresses[i].PassiveSkillType = characters[unitInf[i].mainselected].passivetype;
            Common.progresses[i].PassiveSkillScore = characters[unitInf[i].subselected].passiveskillscore;
        }
        if (currentteacher == -1)
        {
            StartCoroutine(showError("先生バードルが設定されていません!"));
            return;
        }
        Common.teacher = teachers[currentteacher];
        Common.loadingCanvas.SetActive(true);
        CreateProgressWebClient webClient = new CreateProgressWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/gamedata/new");
        webClient.SetData(Common.progresses,new DendouModel[]{Common.teacher});
        StartCoroutine(webClient.Send());
    }
   }
