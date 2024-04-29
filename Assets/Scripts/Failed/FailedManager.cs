using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailedManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] texts = new GameObject[4];
    bool playing = false;

    string[] tips = {
    "【TIPS】先生の能力が高いほど、チームの能力が上がりやすくなるんだとか……",
    "【TIPS】レッスンで、先生がいるエリアは能力が上がりやすくなるんだとか……",
    "【TIPS】組は、生息地や見られる場所によって振り分けられているんだとか……",
    "【TIPS】歌は鳴き声の綺麗さ、姿は目立つかどうか、踊はすばしっこさを表しているんだとか……",
    "【TIPS】属性は、得意な能力とスキルの特性を表しているんだとか……",
    "【TIPS】ライブステージは歌、姿、踊の三種類のエリアに分かれているんだとか……",
    "【TIPS】バードルがいるエリアの種類によって、♡スコアに加算される能力が変わるんだとか……",
    "【TIPS】ミニイベントの選択肢によって、上がるスキルレベルの種類が変わるんだとか……"
    };

    private IEnumerator GoToEnding()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Manager.manager.StateQueue((int)gamestate.Ending);
    }

    public void OnClick()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingTips.enabled = true;
        Common.loadingTips.text = RandomArray.GetRandom(tips);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        Common.MainStoryId = null;
        StartCoroutine(GoToEnding());
    }

    IEnumerator fadeIn(GameObject text)
    {
        RectTransform rect = text.GetComponent<RectTransform>();
        Image image = text.GetComponent<Image>();
        var wait = new WaitForFixedUpdate();
        for (int i = 0; i < 51; i++)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 1);
            Color color = image.color;
            color.a += 5f / 255f;
            image.color = color;
            yield return wait;
        }

    }

    IEnumerator play()
    {
        var waittime = new WaitForFixedUpdate();
        for (int i = 0; i < 4; i++)
        {
            yield return fadeIn(texts[i]);
            yield return waittime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing && SceneManager.GetActiveScene().name == "Failed")
        {
            playing = true;
            StartCoroutine(play());
        }
    }
}
