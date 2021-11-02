using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkErrorDialogController : MonoBehaviour
{
    [Header("Choice")]
    [SerializeField] public GameObject choiceWhenConnErrorUI;
    public static GameObject ChoiceWhenConnErrorUI;
    public static GameWebClient gameWebClient;

    [Header("Confirm")]
    [SerializeField] public GameObject confirmUI;
    public static GameObject ConfirmUI;

    public static bool EnableExit = false;


    private void Awake()
    {
        ChoiceWhenConnErrorUI = this.choiceWhenConnErrorUI;
        ConfirmUI = this.confirmUI;
    }

    //when timeout
    public static void OpenTimeoutDialog(GameWebClient gameWebClient)
    {
        NetworkErrorDialogController.gameWebClient = gameWebClient;
        NetworkErrorDialogController.EnableExit = gameWebClient.IsExitOnFailed;
        ChoiceWhenConnErrorUI.SetActive(true);
    }

    //Open Confirm UI, If agree, do action()
    public static void OpenConfirmDialog(Action action, string text= "通信に失敗しました。\nタイトルに戻ります。")
    {
        ConfirmUI.SetActive(true);
        Text t = ConfirmUI.GetComponentInChildren<Text>();
        t.text = text;
        Button b = ConfirmUI.GetComponentInChildren<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => {
            action();
            ConfirmUI.SetActive(false);
        });
    }


    //When timeout, and send again
    public void OnContinueConnectionButtonClicked()
    {
        EnableExit = false;
        NetworkErrorDialogController.ChoiceWhenConnErrorUI.SetActive(false);
        NetworkErrorDialogController.gameWebClient.Choice_ContinueConnection();
    }

    //when timeout, and return to title scene
    public void OnQuitConnectionButtonClicked()
    {
        if (EnableExit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
            return;
        }
        EnableExit = false;
        NetworkErrorDialogController.ChoiceWhenConnErrorUI.SetActive(false);
        NetworkErrorDialogController.gameWebClient.Choice_QuitConnection();

        if (!Common.loadingCanvas.active)
        {
            Common.loadingCanvas.SetActive(true);
            Common.loadingGif.GetComponent<GifPlayer>().index = 0;
            Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        }
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        Manager.manager.StateQueue((int)gamestate.Title);
    }
}
