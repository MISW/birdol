using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour {

    //public
    float speed = 0.1f;
    public int index = 0;
    int size = 35;
    public string path;

    private Image image;
    IEnumerator coroutine;

    void Start() {
        //StartGif();
    }

    public void StartGif()
    {
        image = GetComponent<Image>();
        coroutine = updateImg();
        StartCoroutine(coroutine);
    }

    public void StopGif()
    {
        StopCoroutine(coroutine);
    }
    private IEnumerator updateImg()
    {
        int index = 1;
        int gifid = new System.Random().Next(0, 32);
        var waittime = new WaitForSecondsRealtime(speed);
        while (true)
        {
            image.sprite = Resources.Load<Sprite>(path + gifid + "/" + index);
            if (index < size) index++;
            else index = 1;
#if UNITY_EDITOR
            Debug.Log("current:"+index);
#endif
            yield return waittime;
        }
    }
}