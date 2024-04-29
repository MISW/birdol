using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class Common : MonoBehaviour
{
    public static CharacterModel[] characters;
    public static Sprite[] standImages = new Sprite[34];
    public static ProgressModel[] progresses = new ProgressModel[5];
    public static DendouModel teacher;

    private const string MAIN_STORY_ID = "MAIN_STORY_ID";
    public static string MainStoryId
    {
        get
        {
            var value = PlayerPrefs.GetString(MAIN_STORY_ID);
            Debug.Log("Get Story To:" + value);
            return value;
        }
        set
        {
            PlayerPrefs.SetString(MAIN_STORY_ID, value);
            PlayerPrefs.Save();
            Debug.Log("Set Story To:" + value);
        }
    }

    private const string LESSON_COUNT = "LESSON_COUNT";
    public static int lessonCount = 5;
    public static int LessonCount
    {
        get
        {
            return PlayerPrefs.GetInt(LESSON_COUNT);
        }
        set
        {
            lessonCount = value;
            PlayerPrefs.SetInt(LESSON_COUNT, lessonCount);
            PlayerPrefs.Save();
        }
    }

    public static int progressId;

    public static GameObject loadingCanvas;
    public static GameObject loadingGif;
    public static AudioSource bgmplayer;
    public static AudioSource seplayer;
    public static AudioSource subseplayer;
    public static Text loadingTips;
    public static List<int> remainingSubstory = new List<int>();
    public static string mom = "ママ";
    private static readonly int[] liveScoreMaxValues = { 600, 900, 1200, 2000, 2400, 3200, 4400, 5000 };

    private const string FREE_LIVE_BGM = "FREE_LIVE_BGM";
    public static string freebgm;
    public static string Freebgm
    {
        get
        {
            if (string.IsNullOrEmpty(freebgm))
            {
                freebgm = PlayerPrefs.GetString(FREE_LIVE_BGM);
            }
            return freebgm;
        }
        set
        {
            if (freebgm != value)
            {
                freebgm = value;
                PlayerPrefs.SetString(FREE_LIVE_BGM, freebgm);
                PlayerPrefs.Save();
            }
        }
    }

    public static float bgmmaxvol = 1.0f;
    private const string BGM_VOLUME = "BGM_VOLUME";
    public static float bgmvol = 0.5f;
    public static float BGMVol
    {
        get
        {
            if (PlayerPrefs.GetFloat(BGM_VOLUME) > 0f)
            {
                bgmvol = PlayerPrefs.GetFloat(BGM_VOLUME);
            }
            return bgmvol;
        }
        set
        {
            if (bgmvol != value)
            {
                bgmvol = value;
                PlayerPrefs.SetFloat(BGM_VOLUME, bgmvol);
                PlayerPrefs.Save();
            }
        }
    }

    public static float semaxvol = 1.0f;
    private const string SE_VOLUME = "SE_VOLUME";
    public static float sevol = 0.5f;
    public static float SEVol
    {
        get
        {
            if (PlayerPrefs.GetFloat(SE_VOLUME) > 0f)
            {
                sevol = PlayerPrefs.GetFloat(SE_VOLUME);
            }
            return sevol;
        }
        set
        {
            if (sevol != value)
            {
                sevol = value;
                PlayerPrefs.SetFloat(SE_VOLUME, sevol);
                PlayerPrefs.Save();
            }
        }
    }

    private const string SKIP_STORY = "SKIP_STORY";
    public static string skipStory;
    public static string SkipStory
    {
        get
        {
            if (string.IsNullOrEmpty(skipStory))
            {
                skipStory = PlayerPrefs.GetString(SKIP_STORY);
            }
            return skipStory;
        }
        set
        {
            if (skipStory != value)
            {
                skipStory = value;
                PlayerPrefs.SetString(SKIP_STORY, skipStory);
                PlayerPrefs.Save();
            }
        }
    }

    public static IEnumerator playBGM()
    {
        bgmplayer.volume = 0f;
        bgmplayer.Play();
        var fixedupdate = new WaitForFixedUpdate();
        float currentvol = 0f;
        float destvol = BGMVol;
        while (currentvol <= destvol)
        {
            currentvol += 0.001f;
            bgmplayer.volume = currentvol;
            yield return fixedupdate;
        }

    }

    public static IEnumerator pauseBGM()
    {
        var fixedupdate = new WaitForFixedUpdate();
        float currentvol = BGMVol;
        while (currentvol > 0f)
        {
            currentvol -= 0.001f;
            bgmplayer.volume = currentvol;
            yield return fixedupdate;
        }
        bgmplayer.Pause();
    }

    public static IEnumerator stopBGM()
    {
        var fixedupdate = new WaitForFixedUpdate();
        float currentvol = BGMVol;
        while (currentvol > 0f)
        {
            currentvol -= 0.001f;
#if UNITY_EDITOR
            Debug.Log("StopVol:"+currentvol);
#endif
            bgmplayer.volume = currentvol;
            yield return fixedupdate;
        }
        bgmplayer.Stop();
        bgmplayer.time = 0f;
    }

    public static Dictionary<string, AudioClip> seclips;

    public static void initSounds()
    {
        seclips = new Dictionary<string, AudioClip>()
        {
            {"okbig1", (AudioClip)Resources.Load("SE/okbig1") },
            {"ikuseistart1", (AudioClip)Resources.Load("SE/ikuseistart1") },
            {"freelive1", (AudioClip)Resources.Load("SE/menu/freelive1") },
            {"zukan1", (AudioClip)Resources.Load("SE/menu/zukan1") },
            {"sudattabirdol1", (AudioClip)Resources.Load("SE/menu/sudattabirdol1") },
            {"ok1", (AudioClip)Resources.Load("SE/ok1") },
            {"cancel1", (AudioClip)Resources.Load("SE/cancel1") },
            {"cancel2", (AudioClip)Resources.Load("SE/cancel2") },
            {"error1", (AudioClip)Resources.Load("SE/error1") },
        };
    }
    public static void initCharacters()
    {
        string json = Resources.Load<TextAsset>("Common/characters").ToString();
        characters = JsonUtility.FromJson<CommonCharacters>(json).characters;
        for (int i = 0; i < 32; i++)
        {
            standImages[i] = Resources.Load<Sprite>("Images/standimage/" + characters[i].id);
        }
    }


    /// <summary>
    /// </summary>
    /// <returns>現在のLiveの目標ハートスコア</returns>
    public static int GetLiveScoreMaxValue()
    {
        try
        {
            int chapter = MainStoryId[0] - '0';
            if (chapter >= 10 || chapter < 1) throw new Exception($"Unexpected mainstoryid: {MainStoryId}");
#if UNITY_EDITOR
            Debug.Log($"chapter: {chapter}, ノルマ: {liveScoreMaxValues[chapter - 1]}");
#endif
            return liveScoreMaxValues[chapter - 1];
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return 0;
        }
    }

    public static string playerName;
    private const string PLAYERPREFS_PLAYER_NAME = "PLAYER_NAME";
    public static string PlayerName
    {
        get
        {
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = PlayerPrefs.GetString(PLAYERPREFS_PLAYER_NAME);
            }
            return playerName;
        }
        set
        {
            if (playerName != value)
            {
                playerName = value;
                PlayerPrefs.SetString(PLAYERPREFS_PLAYER_NAME, playerName);
                PlayerPrefs.Save();
            }
        }
    }

    private const string TRIGGERED_SUB = "TRIGGERED_SUB";
    public static string TriggeredSubStory
    {
        get
        {

            return PlayerPrefs.GetString(TRIGGERED_SUB);
        }
        set
        {
            PlayerPrefs.SetString(TRIGGERED_SUB, value);
            PlayerPrefs.Save();
        }
    }

    public static int homeStandingId = -1;
    private const string PLAYERPREFS_HOMESTANDING_ID = "HOMESTANDING_ID";
    public static int HomeStandingId
    {
        get
        {
            if (homeStandingId == -1)
            {
                homeStandingId = PlayerPrefs.GetInt(PLAYERPREFS_HOMESTANDING_ID);
            }
            return homeStandingId;
        }
        set
        {
            if (homeStandingId != value)
            {
                homeStandingId = value;
                PlayerPrefs.SetInt(PLAYERPREFS_HOMESTANDING_ID, homeStandingId);
                PlayerPrefs.Save();
            }
        }
    }

    void Start()
    {
        initCharacters();
    }
}
