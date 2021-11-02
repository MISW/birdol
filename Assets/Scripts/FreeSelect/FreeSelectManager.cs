using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FreeSelectManager : MonoBehaviour
{
    public GameObject completedPage;
    public GameObject mainPage;
    public GameObject completedTeamName;
    public GameObject completedFirst;
    public GameObject errorDialog;

    public GameObject[] pairLists;

    public static int[] initid;
    public static List<DendouModel> CompletedCharacters = new List<DendouModel>();
    List<GameObject> completedObjects = new List<GameObject>();

    public CharacterModel[] characters=new CharacterModel[10];
    int[] selected;
    public GameObject[] charcterIcons;

    public Button TM01;
    public Button TM02;

    int currentindex = -1;
    int backup = -1;
    int currentselected = -1;
    // Start is called before the first frame update

   

    void Start()
    {
        mainPage.transform.SetSiblingIndex(1);
        selected = new int[CompletedCharacters.Count];
        bool completedinited = false;
        if (Common.Freebgm == "TM01") onTM01();
        else onTM02();
        foreach (DendouModel dendouModel in CompletedCharacters)
        {
            GameObject completedChild = completedFirst;
            if (completedinited)
            {
                completedChild = Instantiate(completedFirst, completedFirst.transform.parent);
            }
            else
            {
                completedinited = true;
            }
#if UNITY_ANDROID
            completedChild.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/charactericon/" + dendouModel.MainCharacterId + ".png");
            completedChild.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/charactericon/" + dendouModel.SupportCharacterId + ".png");
#else
            completedChild.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + dendouModel.MainCharacterId);
            completedChild.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + dendouModel.SupportCharacterId);
#endif
            completedChild.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = dendouModel.Vocal.ToString();
            completedChild.transform.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = dendouModel.Visual.ToString();
            completedChild.transform.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = dendouModel.Dance.ToString();
            completedObjects.Add(completedChild);
        }


    }

    public void SelectCompleted(Button button)
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        if (currentselected != -1) completedObjects[currentselected].transform.GetChild(0).gameObject.SetActive(false);
        currentselected = button.transform.GetSiblingIndex();
        completedTeamName.GetComponent<InputField>().text = CompletedCharacters[currentselected].Name;
        completedObjects[currentselected].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OpenCompletedPage(int index)
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        currentindex = index;
        for (int i = 0; i < CompletedCharacters.Count; i++)
        {
#if UNITY_EDITOR
            Debug.Log("Current: "+ i +"value" + selected[i]);
#endif
            if (selected[i]!=0)
            {
                if (selected[i] - 1 == currentindex)
                {
                    completedObjects[i].GetComponent<Button>().interactable = true;
                    completedObjects[i].transform.GetChild(0).gameObject.SetActive(true);
                    completedTeamName.GetComponent<InputField>().text = CompletedCharacters[i].Name;
                    currentselected = i;
                    backup = i;
                }
                else
                {
                    completedObjects[i].GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                completedObjects[i].GetComponent<Button>().interactable = true;
            }
        }
        completedPage.transform.SetSiblingIndex(1);
    }


    public void FinishCompletedSelect()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Transform pairList = pairLists[currentindex].transform;
        if (currentselected != -1) completedObjects[currentselected].transform.GetChild(0).gameObject.SetActive(false);
        else
        {
            mainPage.transform.SetSiblingIndex(1);
            return;
        }
#if UNITY_ANDROID
        pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/charactericon/" + CompletedCharacters[currentselected].MainCharacterId + ".png");
        pairList.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/charactericon/" + CompletedCharacters[currentselected].SupportCharacterId + ".png");
#else
        pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + CompletedCharacters[currentselected].MainCharacterId);
        pairList.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + CompletedCharacters[currentselected].SupportCharacterId);
#endif
        pairList.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = CompletedCharacters[currentselected].Name;
        pairList.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = CompletedCharacters[currentselected].Vocal.ToString();
        pairList.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = CompletedCharacters[currentselected].Visual.ToString();
        pairList.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = CompletedCharacters[currentselected].Dance.ToString();
        if(currentselected != -1)selected[currentselected] = currentindex+1;
        if (backup != -1) selected[backup] = 0;
        mainPage.transform.SetSiblingIndex(1);
        completedTeamName.GetComponent<InputField>().text = "";
        currentselected = -1;
        currentindex = -1;
        backup = -1;
    }
    public void ClearCompletedSelect()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["cancel1"]);
        Transform pairList = pairLists[currentindex].transform;
        if (currentselected != -1) completedObjects[currentselected].transform.GetChild(0).gameObject.SetActive(false);
#if UNITY_ANDROID
        pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("main");
        pairList.GetChild(1).gameObject.GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>("teacher_sub");
#else
        pairList.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gachaunit/main");
        pairList.GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gachaunit/teacher_sub");
#endif
        pairList.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
        pairList.GetChild(3).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "";
        pairList.GetChild(3).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "";
        pairList.GetChild(3).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
        if(backup != -1)selected[backup] = 0;
        mainPage.transform.SetSiblingIndex(1);
        completedTeamName.GetComponent<InputField>().text = "";
        currentselected = -1;
        currentindex = -1;
        backup = -1;
    }

    private IEnumerator showError(string error)
    {
        Common.subseplayer.PlayOneShot(Common.seclips["error1"]);
        errorDialog.SetActive(true);
        errorDialog.transform.GetChild(0).gameObject.GetComponent<Text>().text = error;
        yield return new WaitForSeconds(2);
        errorDialog.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        errorDialog.SetActive(false);
    }

    public void StartFreeLive()
    {
        FreeLiveController.backup.Clear();
        bool haschar = false;
        for (int i = 0; i < CompletedCharacters.Count; i++)
        {
            if (selected[i]!=0)
            {
                ProgressModel progress = new ProgressModel();
                progress.MainCharacterId = CompletedCharacters[i].MainCharacterId;
                progress.SupportCharacterId = CompletedCharacters[i].SupportCharacterId;
                progress.Name = CompletedCharacters[i].Name;
                progress.Visual = CompletedCharacters[i].Visual;
                progress.Vocal = CompletedCharacters[i].Vocal;
                progress.Dance = CompletedCharacters[i].Dance;
                progress.ActiveSkillLevel = CompletedCharacters[i].ActiveSkillLevel;
                progress.PassiveSkillLevel = CompletedCharacters[i].PassiveSkillLevel;
                FreeLiveController.backup.Add(progress);
                haschar = true;
            }
        }
        if (!haschar)
        {
            StartCoroutine(showError("1体以上のキャラクターを設定してください!"));
            return;
        }
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        Manager.manager.StateQueue((int)gamestate.FreeLive);

    }

    bool triggerdPlayer = false;
    private void Update()
    {
        if (!triggerdPlayer && SceneManager.GetActiveScene().name == "FreeSelect")
        {
#if UNITY_ANDROID
            Common.bgmplayer.clip = Common.assetBundle.LoadAsset<AudioClip>("BG04");
#else
            Common.bgmplayer.clip = Resources.Load<AudioClip>("Music/BG04");
#endif
            Common.bgmplayer.Play();
            triggerdPlayer = true;
        }
    }

    public void onTM01()
    {
        TM01.interactable = false;
        TM02.interactable = true;
        Common.Freebgm = "TM01";
    }

    public void onTM02()
    {
        TM01.interactable = true;
        TM02.interactable = false;
        Common.Freebgm = "TM02";
    }

}
