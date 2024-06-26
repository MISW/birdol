using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HomeUtil : MonoBehaviour
{
    int characterSize = 32;
    public GameObject Gallery;
    public GameObject Ikusei;
    public GameObject CharacterImage;
    public GameObject Dialog;
    public GameObject Option;
    public GameObject StandingTester;
    public GameObject AccuntUI;
    int PrevIndex = -1;
    int chara_id;
    int dialogstatus = 0;
    public Text dialogtext;
    public Text chosencharacter;
    public Image CharacterImageSplite;
    public CharacterModel charactermodel;
    public HomeCharacters homeCharacters;
    public int tempteacherid;
    public GameObject prefab;
    public Transform content;
    public Slider volumeSlider;
    public Slider SEvolumeSlider;

    public static List<bool> isUnlocked = new List<bool>();
    Dictionary<string, AudioClip> seclips;

    public void VolumeChangeCheck()
    {
        Common.BGMVol = Common.bgmmaxvol * volumeSlider.value;
        if (volumeSlider.value == 0)
        {
            Common.bgmplayer.mute = true;
        }
        else
        {
            Common.bgmplayer.mute = false;
            Common.bgmplayer.volume = Common.BGMVol/Common.bgmmaxvol;
        }
    }

    public void SEVolumeChangeCheck()
    {
        Common.SEVol = Common.semaxvol * SEvolumeSlider.value;
        if (SEvolumeSlider.value == 0)
        {
            Common.seplayer.mute = true;
            Common.subseplayer.mute = true;
        }
        else
        {
            Common.seplayer.mute = false;
            Common.subseplayer.mute = false;
            Common.seplayer.volume = Common.SEVol/Common.semaxvol;
            Common.subseplayer.volume = Common.SEVol/Common.semaxvol;
        }
    }
    private void Start()
    {
        //解禁状況仮データ
        //for (int i = 0; i <= characterSize; i++) isUnlocked.Add(true);


        Dialog.SetActive(false);
        CharacterImage.SetActive(true);
        volumeSlider.value = Common.BGMVol / Common.bgmmaxvol;
        SEvolumeSlider.value = Common.SEVol / Common.semaxvol;
        volumeSlider.onValueChanged.AddListener(delegate { VolumeChangeCheck(); });
        SEvolumeSlider.onValueChanged.AddListener(delegate { SEVolumeChangeCheck(); });
        json_parser();
        //positionAdjust();
        chara_id = Common.HomeStandingId;
        CharacterListInit();
        CharacterListPushed(chara_id.ToString());
        volumeSlider.value = Common.BGMVol / Common.bgmmaxvol;
        SEvolumeSlider.value = Common.SEVol / Common.semaxvol;
        standingChanger();
        if (Common.mainstoryid != null && Common.mainstoryid != "opening" && Common.mainstoryid != "0")
        {
            Ikusei.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/button_ikuseirestart");
        }

    }

    bool triggerdPlayer = false;
    private void Update()
    {
        if (!triggerdPlayer && SceneManager.GetActiveScene().name == "Home")
        {
            Common.bgmplayer.clip = Common.bundle.LoadAsset<AudioClip>("BG01");
            Common.bgmplayer.Play();
            triggerdPlayer = true;
        }
    }

    public void onButtonPressedScoreAttack()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
        getCompletedWebClient.target = "freeselect";
        StartCoroutine(getCompletedWebClient.Send());

    }

    public void onButtonPressedGallery()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
#if UNITY_EDITOR
        Debug.Log("Pushed Gallery");
#endif
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        GetGalleryWebClient webClient = new GetGalleryWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/gallery?session_id=" + Common.SessionID);
        StartCoroutine(webClient.Send());

    }

    public void onButtonPressedDendou()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
#if UNITY_EDITOR
        Debug.Log("Pushed Dendou");
#endif
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        CompletedController.CompletedCharacters.Clear();
        GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
        getCompletedWebClient.target = "completed";
        StartCoroutine(getCompletedWebClient.Send());
    }

    private IEnumerator NewStory()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Manager.manager.StateQueue((int)gamestate.Story);
    }

    public void onButtonPressedIkusei()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
#if UNITY_EDITOR
        Debug.Log("Pushed Ikusei");
#endif
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        if (Common.mainstoryid == null || Common.mainstoryid == "opening" || Common.mainstoryid == "0")
        {
            Common.mainstoryid = "opening";
            StartCoroutine(NewStory());
        }
        else
        {
            GetCharacterWebClient webClient = new GetCharacterWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/character?session_id=" + Common.SessionID);
            StartCoroutine(webClient.Send());
        }
    }

    public void onButtonPressedCharacterImage()
    {
#if UNITY_EDITOR
        Debug.Log("Pushed CharaImage");
#endif
        DialogTextChanger();
        Dialog.SetActive(true);
        dialogstatus++;
        //Close Dialog after 5s
        StartCoroutine(DelayCoroutine(5.0f, () =>
        {
            DialogCloser();
        }));

    }

    public void onButtonPressedDialog()
    {
#if UNITY_EDITOR
        Debug.Log("Pushed Dialog");
#endif
        //Dialog.SetActive(false);
    }

    public void onButtonPressedOption()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
#if UNITY_EDITOR
        Debug.Log("Pushed Option");
#endif

    }
    public void onButtonPressedStandingTester()
    {
#if UNITY_EDITOR
        Debug.Log("StandingTester");
#endif
        Dialog.SetActive(false);
        chara_id++;
        if (chara_id >= characterSize) chara_id = 0;
        json_parser();
        //positionAdjust();
    }


    void positionAdjust()
    {
#if UNITY_EDITOR
        Debug.Log(homeCharacters.Characters[chara_id].id + ": " + homeCharacters.Characters[chara_id].name);
#endif
        Transform tr = CharacterImage.transform;
        Vector3 currentPos = tr.localPosition;
        Vector3 currentScale = tr.localScale;

        float scaletmp = homeCharacters.Characters[chara_id].charascale;
        currentScale = new Vector3(scaletmp, scaletmp, scaletmp);
        currentPos.x = homeCharacters.Characters[chara_id].charaposx;
        currentPos.y = homeCharacters.Characters[chara_id].charaposy;

        tr.localPosition = currentPos;
        tr.localScale = currentScale;
    }


    void json_parser()
    {
        //Load Json file
        string json_tmp = Resources.Load<TextAsset>("HomeData/CharaText").ToString();
        homeCharacters = JsonUtility.FromJson<HomeCharacters>(json_tmp);
    }

    void standingChanger()
    {
        chara_id = Common.HomeStandingId;
        //standing select
        CharacterImageSplite.sprite = Common.bundle.LoadAsset<Sprite>(chara_id.ToString());
        chosencharacter.text = "選択中　：　" + homeCharacters.Characters[chara_id].name;
    }

    void CharacterListInit()
    {
        for (int i = 0; i < characterSize; i++)
        {
            if (isUnlocked[i])
            {
                GameObject node = Instantiate(prefab) as GameObject;
                node.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + i);
                node.name = i.ToString();
                node.transform.SetParent(content);
                node.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void CharacterListPushed(string s)
    {

        foreach (Transform child in content.transform)
        {
            child.transform.Find("Selected").gameObject.SetActive(false);
        }
        content.transform.Find(s).gameObject.transform.Find("Selected").gameObject.SetActive(true);
        Common.HomeStandingId = int.Parse(s);
        standingChanger();
    }

    void DialogTextChanger()
    {
        int rand_num, TextMaxSize;
        do
        {
            TextMaxSize = homeCharacters.Characters[chara_id].text.Length;
            rand_num = (int)(UnityEngine.Random.value * TextMaxSize);

        } while ((TextMaxSize != 1 && rand_num == PrevIndex) || rand_num >= TextMaxSize);

        PrevIndex = rand_num;
        string tmpserifu = homeCharacters.Characters[chara_id].text[rand_num];
        dialogtext.text = tmpserifu.Replace("[mama]", Common.mom).Replace("[player]", Common.PlayerName);
    }

    void DialogCloser()
    {
        if (dialogstatus != 0) dialogstatus--;
        if (dialogstatus == 0) Dialog.SetActive(false);
    }

    private IEnumerator DelayCoroutine(float seconds, UnityAction callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}
