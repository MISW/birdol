using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour {

    //public
    float speed = 0.1f;
    public int index = 0;
    public int size;
    public string path;

    private Image image;

    void Start() {
        image = GetComponent<Image>();
        IEnumerator coroutine = updateImg();
        StartCoroutine(coroutine);
    }

    private IEnumerator updateImg()
    {
        int index = 0;
        while (true)
        {
            image.sprite = Resources.Load<Sprite>(path + index);
            if (index < size) index++;
            else index = 0;
            Debug.Log("current:"+index);
            yield return new WaitForSeconds(speed);
        }
    }
}