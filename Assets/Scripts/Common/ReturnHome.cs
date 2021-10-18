using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHome : MonoBehaviour
{
    // Start is called before the first frame update
    public void returnHome()
    {
        Common.loadingCanvas.SetActive(true);
        Manager.manager.StateQueue((int)gamestate.Home);
    }
}
