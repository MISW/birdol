using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHome : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject dialog;
    public void returnHome()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["cancel2"]);
        dialog.SetActive(true);
    }
}
