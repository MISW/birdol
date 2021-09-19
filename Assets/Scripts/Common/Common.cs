using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Common : MonoBehaviour
{
    public static CharacterModel[] characters;
    public static ProgressModel[] progresses;

    public static void initCharacters()
    {
        string json = Resources.Load<TextAsset>("Common/characters").ToString();
        characters = JsonUtility.FromJson<CommonCharacters>(json).characters;
    }

    public static void initProgress()
    {
        string json = Resources.Load<TextAsset>("Live/testdata").ToString();
        progresses = JsonUtility.FromJson<ProgressData>(json).progresses;
    }

    public static void syncProgress()
    {

    }

    //通信関連
    public const string protocol = "http"; //"http" や "https" など 
    public const string hostname = "localhost";
    public const string port = "80";
    public const int timeout = 4; //通信タイムアウトの秒数 
    public const bool allowAllCertification = true; //trueの場合、オレオレ証明書を含め全ての証明書を認証し通信する。httpsプロトコル使用時に注意。
    public const string salt = "Ll7Iy0r9zWslDniwgUXeS0KM9xke4zeg"; //固定ソルト

    //PlayerPrefsに保存
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
    //Signup時に自動で生成されるパスワード。手動で設定するパスワードをデバイスに保存することは現在想定していない。
    private const string PLAYERPREFS_DEFAULT_PASSWORD = "PLAYERPREFS_DEFAULT_PASSWORD";
    private static string defaultPassword;
    public static string DefaultPassword 
    { 
        get
        {
            if (string.IsNullOrEmpty(defaultPassword))
            {
                
                    defaultPassword = PlayerPrefs.GetString(PLAYERPREFS_DEFAULT_PASSWORD);
            }
            return defaultPassword;
        }
        set
        {
            defaultPassword = value;
            PlayerPrefs.SetString(PLAYERPREFS_DEFAULT_PASSWORD, defaultPassword);
            PlayerPrefs.Save();
        }
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
            t += characters[Random.Range(0, characters.Length)];
        }
        return t;
    }

    void Start()
    {
        initCharacters();
    }
}
