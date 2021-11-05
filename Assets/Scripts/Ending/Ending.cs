using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    #region//public変数
    [Header("Characterのプレハブ")] public GameObject character;
    public GameObject canvas;
    [Header("各キャラクターのステータス")] public float[] characterStatus = new float[15];
    [Header("キャラクターのステータスの最大値")]public float maxStatus; //ステータス上限値
    [Header("星のSprite")]public Sprite[] star = new Sprite[5];
    [Header("アクティブスキルゲージのSprite")] public Sprite[] activeSkillGaugeSprites = new Sprite[6];
    [Header("パッシブスキルゲージのSprite")] public Sprite[] passiveSkillGaugeSprites = new Sprite[6];
    [Header("星の数")]public int maxStar; //星の数
    public ProgressModel[] Characters = new ProgressModel[5];
    #endregion

    #region//private変数
    private int currentCharacterNumber = 0;
    private int currentCharacterVocal = 0;
    private int currentCharacterVisual = 0;
    private int currentCharacterDance = 0;
    private GameObject Star = null;
    private Image activeSkillGaugeImage = null;
    private Image passiveSkillGaugeImage = null;
    private List<Image> VocalStarImage = new List<Image>();
    private List<Image> VisualStarImage = new List<Image>();
    private List<Image> DanceStarImage = new List<Image>();
    private List<GameObject> CharacterList = new List<GameObject>();
    private List<GameObject> CharacterButtonList = new List<GameObject>();
    #endregion

    void Start()
    {
        FindStar();
        FindButton();
        FindGauge();
        CharacterButtonList[0].transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        CharacterButtonList[0].GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,1.0f);
        SetCharacter();
        ChangeCurrentCharacterStars(0);
        ChangeCurrentCharacterSKillGauge(0);
    }

    /// <summary>
    /// 表示画面をi番目のキャラクターに切り替え
    /// </summary>
    /// <param name="i"></param>
    public void ChangeCurrentCharacter(int i)
    {
#if UNITY_EDITOR
        //Debug.Log(i);
#endif
        if (currentCharacterNumber != i)
        {
            CharacterList[currentCharacterNumber].GetComponent<Image>().enabled = false;
            CharacterButtonList[currentCharacterNumber].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            CharacterButtonList[currentCharacterNumber].GetComponent<Image>().color = new Color(110.0f/255.0f, 110.0f / 255.0f, 110.0f / 255.0f, 1.0f);
            CharacterButtonList[i].transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            CharacterButtonList[i].GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,1.0f);
            ChangeCurrentCharacterImage(i);
            ChangeCurrentCharacterStars(i);
            ChangeCurrentCharacterSKillGauge(i);
            currentCharacterNumber = i;
        }
    }

    /// <summary>
    /// i番目のキャラクターに立ち絵を変更
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterImage(int i)
    {
        CharacterList[i].GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// i番目のキャラクターのステータスに合わせて星の数を変更
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterStars(int i)
    {
        //各キャラクターのステータスを読み込み、ステータスの最大値を超えていた場合はその値をステータスの最大値に変更
        currentCharacterVocal = (int)Mathf.Min(Common.progresses[i].Vocal,maxStatus);
        currentCharacterVisual = (int)Mathf.Min(Common.progresses[i].Visual,maxStatus);
        currentCharacterDance = (int)Mathf.Min(Common.progresses[i].Dance,maxStatus);

        currentCharacterVocal = (int)(((float)currentCharacterVocal / (float)maxStatus) * 50.0f);
        currentCharacterVisual = (int)(((float)currentCharacterVisual / (float)maxStatus) * 50.0f);
        currentCharacterDance = (int)(((float)currentCharacterDance / (float)maxStatus) * 50.0f);

        SetSongStar(currentCharacterVocal);
        SetVisualStar(currentCharacterVisual);
        SetDanceStar(currentCharacterDance);

    }

    /// <summary>
    /// i番目のキャラクターのステータスに合わせてスキルゲージを変更
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterSKillGauge(int i)
    {
        activeSkillGaugeImage.sprite = activeSkillGaugeSprites[Common.progresses[i].ActiveSkillLevel];
        passiveSkillGaugeImage.sprite = passiveSkillGaugeSprites[Common.progresses[i].PassiveSkillLevel];
    }

    /// <summary>
    /// 各SongStarのSpriteを与えられたstatusに合わせて変更
    /// </summary>
    /// <param name="status"></param>
    private void SetSongStar(int status)
    {
#if UNITY_EDITOR
        //Debug.Log("Vocal" + status);
#endif
        for (int i=0;i<status/5;i++)
        {
            VocalStarImage[i].enabled = true;
            VocalStarImage[i].sprite = star[4];
        }
        if (status != 50)
        {
            if (status % 5 == 0)
            {
                VocalStarImage[status / 5].enabled = false;
            }
            else
            {
                VocalStarImage[status / 5].enabled = true;
                VocalStarImage[status / 5].sprite = star[(status % 5)-1];
            }
        }
        for(int i=status / 5 + 1;i<maxStar;i++)
        {
            VocalStarImage[i].enabled = false;
        }
    }

    /// <summary>
    /// 各VisualStarのSpriteを与えられたstatusに合わせて変更
    /// </summary>
    /// <param name="status"></param>
    private void SetVisualStar(int status)
    {
#if UNITY_EDITOR
        //Debug.Log("Visual" + status);
#endif
        for (int i = 0; i < status / 5; i++)
        {
            VisualStarImage[i].enabled = true;
            VisualStarImage[i].sprite = star[4];
        }
        if (status != 50)
        {
            if (status % 5 == 0)
            {
                VisualStarImage[status / 5].enabled = false;
            }
            else
            {
                VisualStarImage[status / 5].enabled = true;
                VisualStarImage[status / 5].sprite = star[(status % 5)-1];
            }
        }
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            VisualStarImage[i].enabled = false;
        }
    }

    /// <summary>
    /// 各DanceStarのSpriteを与えられたstatusに合わせて変更
    /// </summary>
    /// <param name="status"></param>
    private void SetDanceStar(int status)
    {
#if UNITY_EDITOR
        //Debug.Log("Dance" + status);
#endif
        for (int i = 0; i < status / 5; i++)
        {
            DanceStarImage[i].enabled = true;
            DanceStarImage[i].sprite = star[4];
        }
        if (status != 50)
        {
            if (status % 5 == 0)
            {
                DanceStarImage[status / 5].enabled = false;
            }
            else
            {
                DanceStarImage[status / 5].enabled = true;
                DanceStarImage[status / 5].sprite = star[(status % 5)-1];
            }
        }
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            DanceStarImage[i].enabled = false;
        }
    }

    /// <summary>
    /// 各StarのImageコンポーネントを捕まえる
    /// </summary>
    private void FindStar()
    {
        for(int i=1;i<=maxStar;i++)
        {
            Star = GameObject.Find("VocalStar" + i);
            VocalStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("VisualStar" + i);
            VisualStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("DanceStar" + i);
            DanceStarImage.Add(Star.GetComponent<Image>());
        }
    }

    /// <summary>
    /// 各Buttonを捕まえる
    /// </summary>
    private void FindButton()
    {
        for(int i=1;i<=5;i++)
        {
            CharacterButtonList.Add(GameObject.Find("CharacterButton" + i));
        }
    }

    /// <summary>
    /// 各Gaugeを捕まえる
    /// </summary>
    private void FindGauge()
    {
        activeSkillGaugeImage = GameObject.Find("ActiveSkillGauge").GetComponent<Image>();
        passiveSkillGaugeImage = GameObject.Find("PassiveSkillGauge").GetComponent<Image>();
    }

    /// <summary>
    /// それぞれのキャラクター画像をMainCharacterIDから取得
    /// </summary>
    private void SetCharacter()
    {
        var parent = canvas.transform;
        for(int i=0;i<5;i++)
        {
            GameObject C = Instantiate(character, parent);
            CharacterList.Add(C);
            CharacterList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
#if UNITY_ANDROID
            CharacterList[i].GetComponent<Image>().sprite = Common.assetBundle.LoadAsset<Sprite>(Common.progresses[i].MainCharacterId.ToString());
            CharacterButtonList[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Assets/Resources/Images/Live/Gif/" + Common.progresses[i].MainCharacterId + "/Ch-0");
#else
            CharacterList[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + Common.progresses[i].MainCharacterId);
            CharacterButtonList[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Gif/" + Common.progresses[i].MainCharacterId + "/Ch-0");
#endif
            CharacterList[i].GetComponent<Image>().enabled = false;
            if (i == 0) CharacterList[i].GetComponent<Image>().enabled = true;
        }
    }

    IEnumerator ReloadHome()
    {
        GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
        getCompletedWebClient.target = "home";
        yield return getCompletedWebClient.Send();
        GetStoryWebClient getStoryWebClient = new GetStoryWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/story?session_id=" + Common.SessionID);
        yield return getStoryWebClient.Send();
    }

    public void ResetStory()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        Common.mainstoryid = null;
        StartCoroutine(ReloadHome());
    }

    bool triggerdPlayer = false;
    void Update()
    {
        if (!triggerdPlayer && SceneManager.GetActiveScene().name == "Ending")
        {
#if UNITY_ANDROID
            Common.bgmplayer.clip = Common.assetBundle.LoadAsset<AudioClip>("BG09");
#else
            Common.bgmplayer.clip = Resources.Load<AudioClip>("Music/BG09");
#endif
            Common.bgmplayer.Play();
            triggerdPlayer = true;
        }
    }
}
