using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkErrorDialogController : MonoBehaviour
{
    [Header("Timeout choice")]
    [SerializeField] public GameObject networkTimeoutUI;
    public static GameObject NetworkTimeoutCanvas;
    public static GameWebClient gameWebClient;

    [Header("Confirm")]
    [SerializeField] public GameObject confirmUI;
    public static GameObject ConfirmUI;


    private void Awake()
    {
        NetworkTimeoutCanvas = this.networkTimeoutUI;
        ConfirmUI = this.confirmUI;
    }

    //when timeout
    public static void OpenTimeoutDialog(GameWebClient gameWebClient)
    {
        NetworkErrorDialogController.gameWebClient = gameWebClient;
        NetworkTimeoutCanvas.SetActive(true);
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
        NetworkErrorDialogController.NetworkTimeoutCanvas.SetActive(false);
        NetworkErrorDialogController.gameWebClient.Choice_ContinueConnection();
    }

    //when timeout, and return to title scene
    public void OnQuitConnectionButtonClicked()
    {
        NetworkErrorDialogController.NetworkTimeoutCanvas.SetActive(false);
        NetworkErrorDialogController.gameWebClient.Choice_QuitConnection();

        Common.loadingCanvas.SetActive(true);
        Manager.manager.StateQueue((int)gamestate.Title);
    }
}
