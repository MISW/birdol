using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressedAction : MonoBehaviour
{

    private void Start()
    {
        Common.bgmplayer.time = 0;
        Common.bgmplayer.clip = (AudioClip)Resources.Load("Music/TM01");
        Common.bgmplayer.Play();
    }

    public void OnClick()
    {
        //ここを変える
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        if (Common.PlayerName == "" || Common.PlayerName == null)
        {
            Manager.manager.StateQueue((int)gamestate.Signup);
        }
        else
        {
            ProgressService.FetchStory();
            ProgressService.FetchCompletedProgressAndUpdateGameStatus("home");
        }
    }
}
