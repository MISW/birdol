using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSprite : MonoBehaviour
{
    public Sprite SpriteNext;

    public void changeSprite()
    {
        this.gameObject.GetComponent<Image>().sprite = SpriteNext;
    }

}