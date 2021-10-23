using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dialog;

    public void OnYesClicked()
    {
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Manager.manager.StateQueue((int)gamestate.Home);
    }

    public void OnNoClicked()
    {
        dialog.SetActive(false);
    }

}
