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
    public const string PLAYERPREFS_ACCESS_TOKEN = "PLAYERPREFS_ACCESS_TOKEN";
    public const string PLAYERPREFS_USER_ID = "PLAYERPREFS_USER_ID";
    public const string PLAYERPREFS_UUID = "PLAYERPREFS_UUID";


    void Start()
    {
        initCharacters();
    }
}
