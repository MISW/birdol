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

    private int GenerateRandomIntExclude4()
    {
        var exclude = new HashSet<int>() { 4};
        var range = Enumerable.Range(0, 32).Where(i => !exclude.Contains(i));

        var rand = new System.Random();
        int index = rand.Next(0, 32 - exclude.Count);
        return range.ElementAt(index);
    }

    private IEnumerator updateImg()
    {
        int index = 1;
        int gifid = GenerateRandomIntExclude4();
        var waittime = new WaitForSecondsRealtime(speed);
        while (true)
        {
#if UNITY_ANDROID
            image.sprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/" + path + gifid + "/" + index + ".png");
#else
            image.sprite = Resources.Load<Sprite>(path + gifid + "/" + index);
#endif
            if (index < size) index++;
            else index = 1;
#if UNITY_EDITOR
            Debug.Log("current:"+index);
#endif
            yield return waittime;
        }
    }
}