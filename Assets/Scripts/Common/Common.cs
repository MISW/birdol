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
    public static string mainstoryid;
    public static int lessonCount = 5;
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
    public static bool hasUpdate = false;
    public static AssetBundle bundle;

    public static string buildCode = "20221113";

    public static IEnumerator initGame(GameObject downloadingCanvas)
    {
        Common.initCharacters();
        Common.initSounds();
        downloadingCanvas.SetActive(false);
        Debug.Log("App version " + Application.version);
        CheckVersionWebClient checkUpdate = new CheckVersionWebClient(WebClient.HttpRequestMethod.Post, $"/api/{Common.api_version}/cli/version");
#if UNITY_ANDROID
        checkUpdate.SetData("Android", Application.version, Common.buildCode);
#elif UNITY_IOS
        checkUpdate.SetData("iOS", Application.version, Common.buildCode);
#else
        checkUpdate.SetData("Win",Application.version,Common.buildCode);
#endif
        yield return checkUpdate.Send();
    }

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
        while(currentvol > 0f)
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
            {"okbig1", bundle.LoadAsset<AudioClip>("okbig1") },
            {"ikuseistart1", bundle.LoadAsset<AudioClip>("ikuseistart1") },
            {"freelive1", bundle.LoadAsset<AudioClip>("freelive1") },
            {"zukan1", bundle.LoadAsset<AudioClip>("zukan1") },
            {"sudattabirdol1", bundle.LoadAsset<AudioClip>("sudattabirdol1") },
            {"ok1", bundle.LoadAsset<AudioClip>("ok1") },
            {"cancel1", bundle.LoadAsset<AudioClip>("cancel1") },
            {"cancel2", bundle.LoadAsset<AudioClip>("cancel2") },
            {"error1", bundle.LoadAsset<AudioClip>("error1") },
        };
    }
    public static void initCharacters()
    {
        string json = Resources.Load<TextAsset>("Common/characters").ToString();
        characters = JsonUtility.FromJson<CommonCharacters>(json).characters;
        for (int i=0;i<32;i++)
        {
            standImages[i] = bundle.LoadAsset<Sprite>(characters[i].id.ToString());
        }
    }

    public static void initProgress()
    {
        string json = Resources.Load<TextAsset>("Live/testdata").ToString();
        progresses = JsonUtility.FromJson<ProgressData>(json).progresses;
    }


    /// <summary>
    /// </summary>
    /// <returns>現在のLiveの目標ハートスコア</returns>
    public static int GetLiveScoreMaxValue()
    {
        try
        {
            int chapter = mainstoryid[0]-'0';
            if (chapter >= 10 || chapter < 1) throw new Exception($"Unexpected mainstoryid: {mainstoryid}");
#if UNITY_EDITOR
            Debug.Log($"chapter: {chapter}, ノルマ: {liveScoreMaxValues[chapter - 1]}");
#endif
            return liveScoreMaxValues[chapter-1];
        }catch(Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return 0;
        }
    }

    //通信関連
    public const string api_version = "v2"; //"v1" or "v2"
    public const string protocol = "https"; //"http" や "https" など
    public const string hostname = "project-birdol.com";
    public const string port = "443";
    public const int timeout = 4; //通信タイムアウトの秒数
    public const bool allowAllCertification = true; //trueの場合、オレオレ証明書を含め全ての証明書を認証し通信する。httpsプロトコル使用時に注意。
    public const string salt = "Ll7Iy0r9zWslDniwgUXeS0KM9xke4zeg"; //固定ソルト

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

    //PlayerPrefsに保存
    //ユーザID
    private const string PLAYERPREFS_USER_ID = "PLAYERPREFS_USER_ID";
    private static uint userID=0;
    public static uint UserID
    {
        get
        {
            if (userID == 0)
            {
                userID = PlayerPrefs.GetUint(PLAYERPREFS_USER_ID);
            }
            return userID;
        }
        set
        {
            if (userID != value)
            {
                userID = value;
                PlayerPrefs.SetUint(PLAYERPREFS_USER_ID, value);
                PlayerPrefs.Save();
            }
        }
    }
    //アクセストークン
    private const string PLAYERPREFS_ACCESS_TOKEN = "PLAYERPREFS_ACCESS_TOKEN";
    private static string accessToken;
    public static string AccessToken
    {
        get
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = PlayerPrefs.GetString(PLAYERPREFS_ACCESS_TOKEN);
            }
            return accessToken;
        }
        set
        {
            if (accessToken != value)
            {
                accessToken = value;
                PlayerPrefs.SetString(PLAYERPREFS_ACCESS_TOKEN, accessToken);
                PlayerPrefs.Save();
            }
        }
    }
    //リフレッシュトークン
    private const string PLAYERPREFS_REFRESH_TOKEN = "PLAYERPREFS_REFRESH_TOKEN";
    private static string refreshToken;
    public static string RefreshToken
    {
        get
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                refreshToken = PlayerPrefs.GetString(PLAYERPREFS_REFRESH_TOKEN);
            }
            return refreshToken;
        }
        set
        {
            if (refreshToken != value)
            {
                refreshToken = value;
                PlayerPrefs.SetString(PLAYERPREFS_REFRESH_TOKEN, refreshToken);
                PlayerPrefs.Save();
            }
        }
    }
    //デバイスID
    private const string PLAYERPREFS_UUID = "PLAYERPREFS_UUID";
    private static string uuid;
    public static string Uuid
    {
        get
        {
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = PlayerPrefs.GetString(PLAYERPREFS_UUID);
            }
            return uuid;
        }
        set
        {
            if (uuid != value)
            {
                uuid = value;
                PlayerPrefs.SetString(PLAYERPREFS_UUID, uuid);
                PlayerPrefs.Save();
            }
        }
    }
    //セッションID
    private const string PLAYERPREFS_SESSION_ID = "PLAYERPREFS_SESSION_ID";
    private static string sessionID;
    public static string SessionID
    {
        get
        {
            if (string.IsNullOrEmpty(sessionID))
            {
                sessionID = PlayerPrefs.GetString(PLAYERPREFS_SESSION_ID);
            }
            return sessionID;
        }
        set
        {
            if (sessionID != value)
            {
                sessionID = value;
                PlayerPrefs.SetString(PLAYERPREFS_SESSION_ID, sessionID);
                PlayerPrefs.Save();
            }
        }
    }
    //Signup時にサーバから受け取るアカウントID。手動で設定するアカウントIDをデバイスに保存することは現在想定していない。
    private const string PLAYERPREFS_DEFALT_ACCOUNT_ID = "PLAYERPREFS_DEFALT_ACCOUNT_ID";
    private static string defaultAccountID;
    public static string DefaultAccountID {
        get
        {
            if (string.IsNullOrEmpty(defaultAccountID))
            {
                defaultAccountID = PlayerPrefs.GetString(PLAYERPREFS_DEFALT_ACCOUNT_ID);
            }
            return defaultAccountID;
        }
        set
        {
            defaultAccountID = value;
            PlayerPrefs.SetString(PLAYERPREFS_DEFALT_ACCOUNT_ID, defaultAccountID);
            PlayerPrefs.Save();
        }
    }

    //RSA Key Pair: 公開鍵と秘密鍵
    //private const string PLAYERPREFS_RSA_PUBLIC_KEY = "PLAYERPREFS_RSA_PUBLIC_KEY";  PrivateKeyにPublicKeyも含まれているため保存する必要ない
    private const string PLAYERPREFS_RSA_PRIVATE_KEY = "PLAYERPREFS_RSA_PRIVATE_KEY";
    private static (string privateKey, string publicKey) rsaKeyPair; //(privateKey: 秘密鍵, publicKey: 公開鍵)
    public static (string privateKey, string publicKey) RsaKeyPair
    {
        get
        {
            if (string.IsNullOrEmpty(rsaKeyPair.privateKey) )
            {
                rsaKeyPair.privateKey = PlayerPrefs.GetString(PLAYERPREFS_RSA_PRIVATE_KEY);
                //rsaKeyPair.publicKey = PlayerPrefs.GetString(PLAYERPREFS_RSA_PUBLIC_KEY);
            }
            return rsaKeyPair;
        }
        set
        {
            rsaKeyPair = value;
            PlayerPrefs.SetString(PLAYERPREFS_RSA_PRIVATE_KEY, rsaKeyPair.privateKey);
            //PlayerPrefs.SetString(PLAYERPREFS_RSA_PUBLIC_KEY, rsaKeyPair.publicKey);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// RSA 秘密鍵 公開鍵 生成
    /// </summary>
    public static (string privateKey, string publicKey) CreateRsaKeyPair()
    {
        int size = 1024;
        RSACryptoServiceProvider csp = new RSACryptoServiceProvider(size);

        string publicKey = csp.ToXmlString(false);
        string privateKey = csp.ToXmlString(true);
        publicKey = StrToBase64Str(publicKey);
        privateKey= StrToBase64Str(privateKey);
        (string privateKey, string publicKey) keyPair = (privateKey: privateKey, publicKey: publicKey);
#if UNITY_EDITOR
        Debug.Log($"private_key: { StrFromBase64Str(keyPair.privateKey) }");
        Debug.Log($"public_key: { StrFromBase64Str(keyPair.publicKey) }");
#endif

        return keyPair;
    }

    /// <summary> ランダム文字列の生成 </summary>
    /// <param name="len">文字列の長さ</param>
    /// <returns></returns>
    public static string GenerateRondomString(int len)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string t = "";
        for(int i = 0; i < len; i++)
        {
            t += characters[UnityEngine.Random.Range(0, characters.Length)];
        }
        return t;
    }

    public static string StrToBase64Str(string text)
    {
        string str = "";
        try
        {
            str = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e);
#endif
        }
        return str;
    }
    public static string StrFromBase64Str(string text)
    {
        string str = "";
        try
        {
            str = Encoding.UTF8.GetString( Convert.FromBase64String(text) );
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e);
#endif
        }
        return str;
    }

    void Start()
    {
        initCharacters();
    }
}
