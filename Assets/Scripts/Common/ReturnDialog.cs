using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dialog;

    public void Start()
    {

    }

    private IEnumerator returnToHome()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Manager.manager.StateQueue((int)gamestate.Home);
    }

    public void OnYesClicked()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        StartCoroutine(returnToHome());
    }

    public void OnNoClicked()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["cancel2"]);
        dialog.SetActive(false);
    }

}
