using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacter : MonoBehaviour
{
    private GameObject gm;
    private HomeUtil hu;

    void Awake() {
        gm = GameObject.Find("Canvas").transform.Find("Draw").gameObject;
        hu = gm.GetComponent<HomeUtil>();
    }

    public void onButtonPressedCharacterButtoun()
    {
        hu.CharacterListPushed(this.name);
    }
}
