using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager manager;
    public GameObject loadingCanvas;
    public GameObject downloadingAssetCanvas;
    public GameObject askDownloadDialog;
    public GameObject downloadAssetFailed;
    public GameObject gif;
    public Text tips;
    public Text downloadingProgress;
    private const string downloadingHint = "初期データ(220MB)をダウンロードしています。少々お待ちください。";
    public AudioSource bgmplayer;
    public AudioSource seplayer;
    public AudioSource subseplayer;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Common.loadingCanvas = loadingCanvas;
        Common.loadingGif = gif;
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.CreateRsaKeyPair();//Android�������΍�
        if (manager == null)
        {
            manager = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public void startDownloadAsset()
    {
        askDownloadDialog.SetActive(false);
        StartCoroutine(AssetBundleLoader.DownloadAndCache(downloadingAssetCanvas, downloadAssetFailed, askDownloadDialog,(progress) =>
        {
            downloadingProgress.text = downloadingHint + $"({(int)(progress * 100 + 9)}%)";
        }));
    }

    public void retryDownloadAsset()
    {
        downloadAssetFailed.SetActive(false);
        StartCoroutine(AssetBundleLoader.DownloadAndCache(downloadingAssetCanvas, downloadAssetFailed, askDownloadDialog,(progress) =>
        {
            downloadingProgress.text = downloadingHint + $"({(int)(progress*100+9)}%)";
        }));
    }

    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }


    // Start is called before the first frame update
    void Start()
    {
        Common.loadingTips = tips;
        Common.bgmplayer = bgmplayer;
        Common.bgmplayer.volume = Common.BGMVol/Common.bgmmaxvol;
        Common.seplayer = seplayer;
        Common.seplayer.volume = Common.SEVol;
        Common.subseplayer = subseplayer;
        Common.subseplayer.volume = Common.SEVol/Common.semaxvol;
        StartCoroutine(AssetBundleLoader.DownloadAndCache(downloadingAssetCanvas, downloadAssetFailed, askDownloadDialog,(progress) =>
        {
            downloadingProgress.text = downloadingHint + $"({(int)(progress * 100+9)}%)";
        }));
    }

    int cnt = 15;
    // Update is called once per frame
    void Update()
    {
        if (statequeueflag)
        {
            StartCoroutine(StateChange());
        }
        Updater();
    }
    
    [SerializeField] gamestate forTest;


    public gamestate GameState
    {
        get
        {
            return Now_GameState;
        }
    }

    gamestate Now_GameState = gamestate.Undefined;
    gamestate Pre_GameState = gamestate.Undefined;
    gamestate Next_GameState = gamestate.Undefined;

    public void StateQueue(int to = -1)
    {
        statequeueflag = true;
        if (to == -1)
        {
            Next_GameState = Pre_GameState;
        }
        else
        {
            Next_GameState = (gamestate)to;
        }
    }
    bool statequeueflag = false;

    SceneVisor Visor;

    IEnumerator StateChange()
    {
        SceneVisor Visor1 = GotVisorOnScene();
        if (SceneManager.GetAllScenes().Length > 1)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1).buildIndex);
        }
        AsyncOperation async = SceneManager.LoadSceneAsync((int)Next_GameState, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        async.completed += x =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)Next_GameState));
            loadingCanvas.SetActive(false);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            if (Common.loadingTips.enabled)
            {
                Common.loadingTips.text = "";
                Common.loadingTips.enabled = false;
            }
            Common.loadingGif.GetComponent<GifPlayer>().StopGif();
            Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        };

        statequeueflag = false;

        Pre_GameState = Now_GameState;
        Now_GameState = gamestate.Undefined;
#if UNITY_EDITOR
        Debug.Log("Transition�c");
#endif

        if (Visor1 != null)
        {
            yield return StartCoroutine(Visor1.Finalizer(Next_GameState));
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Visor null");
#endif
        }

        if (Pre_GameState != 0)
        {
#if UNITY_EDITOR
            Debug.Log("unload");
#endif
            // SceneManager.UnloadSceneAsync((int)Pre_GameState);

        }
        yield return new WaitForSeconds(2);
        async.allowSceneActivation = true;
        yield return new WaitUntil(() => SceneManager.GetSceneByBuildIndex((int)Next_GameState).isLoaded);
        SceneVisor Visor2 = GotVisorOnScene();
        if (Visor2 != null)
        {
            yield return StartCoroutine(Visor2.Init(Pre_GameState));
        }
        else
        {
            print("Visor null");
        }
        Visor = Visor2;
        Now_GameState = Next_GameState;

#if UNITY_EDITOR
        Debug.Log($"GameState was Changed from {Pre_GameState} to {Now_GameState}");
#endif
        yield break;
    }
    void Updater()
    {
        if (Now_GameState != gamestate.Undefined)
        {
            Visor.Updater();
        }
    }

    SceneVisor GotVisorOnScene()
    {
        GameObject @object = GameObject.FindGameObjectWithTag("SceneVisor");
        if (@object != null)
        {
            return @object.GetComponent<SceneVisor>();
        }
        return null;
    }

}






//gamestate��SceneIndex�����v�����Ȃ����΂Ȃ��Ȃ�
public enum gamestate
{
    Undefined,
    Title,
    Signup,
    Home,
    CompletedCharacters,
    Gacha,
    Live,
    Lesson,
    Ending,
    Gallery,
    Login,
    Story,
    GachaUnit,
    Failed,
    FreeSelect,
    FreeLive
}
