using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RunGacha : MonoBehaviour
{
    List<CharacterModel> R3, R2, R1;
    CharacterModel cm;
    float[] probVec = { 0.1f, 0.25f, 0.65f };
    [SerializeField] GameObject result10, resultImageObj, incubator, hinge, overPanelObj, particleObj, skipBtn;
    GameObject[] gachaobjs = new GameObject[10];
    int resultIndex;
    int[] result = new int[10];
    bool isResultShowing, isSkip, isSkippable;
    [SerializeField] Text nameLabel, skillLabel;
    [SerializeField] Image rareImg, nameBox, backGround, overPanel, resultImage;
    [SerializeField] SpriteRenderer charDot, upperEgg, underEgg;
    [SerializeField] Sprite bgImage;
    [SerializeField] Sprite[] rareSprites = new Sprite[3];
    [SerializeField] Sprite[] backGrounds = new Sprite[4];
    [SerializeField] Sprite[] upperEggs = new Sprite[3], underEggs = new Sprite[3];
    [SerializeField] MeshRenderer[] eggs = new MeshRenderer[10];
    [SerializeField] Material[] eggMats = new Material[3];


    void Start()
    {
        gachaobjs = GameObject.FindGameObjectsWithTag("Gacha");
        setNameAlpha(0);
        overPanel.color = new Color(255, 255, 255, 0);
        skillLabel.text = "";
        float ratio = Screen.currentResolution.height / Screen.currentResolution.width;
        //resultImage.rectTransform.localScale = new Vector3(ratio, ratio, 1) * 1.07f;
        backGround.rectTransform.localScale = new Vector3(ratio, ratio, 1) * 1.37f;

        GameObject[] eggsobj = GameObject.FindGameObjectsWithTag("GachaEgg");

        for (int i = 0; i < 10; i++)
        {
            eggs[i] = eggsobj[i].GetComponent<MeshRenderer>();
        }

        resultIndex = 0;
        result10.SetActive(false);
        resultImageObj.SetActive(false);
        incubator.SetActive(false);
        isResultShowing = false;
        isSkip = false;
        isSkippable = false;
        nameLabel.text = "";


        R3 = new List<CharacterModel>();
        R2 = new List<CharacterModel>();
        R1 = new List<CharacterModel>();
        if (Common.characters == null) Common.initCharacters();//Test Only
        for (int i = 0; i < 32; i++)
        {
            CharacterModel character = Common.characters[i];
            if (character.rarity == 1)
            {
                R1.Add(character);

            }
            else if (character.rarity == 2)
            {
                R2.Add(character);

            }
            else
            {
                R3.Add(character);
            }
        }
    }

    bool triggerdPlayer = false;

    void Update()
    {
        if (!triggerdPlayer && SceneManager.GetActiveScene().name == "Gacha")
        {
            Common.bgmplayer.clip = (AudioClip)Resources.Load("Music/BG08");
            Common.bgmplayer.Play();
            triggerdPlayer = true;
        }
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (!result10.activeSelf && !isResultShowing)
                {
                    Destroy(GameObject.Find("Tap"));
                    onButtonPressed10();
                    incubator.SetActive(true);
                    skipBtn.SetActive(true);
                    StartCoroutine("slideIncubator");
                }
                else if (isResultShowing && !isSkip && isSkippable)
                {
                    isSkip = true;
                }
            }
        }
    }

    IEnumerator slideIncubator()
    {
        backGround.sprite = bgImage;
        isSkippable = false;
        float t = 0;
        while (t <= 1)
        {
            t += 0.025f;
            incubator.transform.position = new Vector3(-8, QuadPassEase(-3, -0.6f, 0.8f, t), 0);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);

        t = 0;

        while (t <= 1)
        {
            t += 0.025f;
            incubator.transform.position = new Vector3(QuadEase(-8, 3, t), QuadEase(-0.6f, -0.2f, t), 0);
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine("openIncubator");
    }

    IEnumerator openIncubator()
    {
        float t = 0;
        yield return new WaitForSeconds(1);

        while (t <= 1)
        {
            t += 0.02f;
            hinge.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 20), Quaternion.Euler(0, 0, -70), t);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine("slideEgg");
    }

    IEnumerator slideEgg()
    {
        cm = Common.characters[result[resultIndex]];
        setEggAlpha(1);
        backGround.color = new Color(255, 255, 255, 1);
        charDot.transform.position = new Vector3(0, 6, 0);
        upperEgg.transform.localPosition = new Vector3(0, 1.667f, -0.1f);
        underEgg.transform.localPosition = new Vector3(0, 1.667f, -0.1f);
        upperEgg.sprite = upperEggs[cm.rarity - 1];
        underEgg.sprite = underEggs[cm.rarity - 1];
        setNameAlpha(0);
        nameLabel.text = "";
        isSkip = false;
        isSkippable = true;
        charDot.sprite = Resources.Load<Sprite>("Images/Live/Gif/" + cm.id + "/ch-0");

        backGround.sprite = backGrounds[cm.rarity];
        float t = 0;
        while (t <= 1)
        {
            if (isSkip)
            {
                charDot.transform.position = Vector3.zero;
                isSkip = false;
                break;
            }

            t += 0.02f;
            charDot.transform.position = new Vector3(0, QuadEase(6, -0.2f, t), 0);
            yield return new WaitForFixedUpdate();
        }
        incubator.SetActive(false);
        isSkippable = false;
        yield return new WaitForSeconds(1.5f);
        StartCoroutine("breakEgg");
    }

    IEnumerator breakEgg()
    {
        isSkippable = true;
        float t = 0;
        while (t <= 1)
        {
            if (isSkip)
            {
                upperEgg.transform.localPosition = new Vector3(0, 8.625f, -0.1f);
                underEgg.transform.localPosition = new Vector3(0, -5.275f, -0.1f);
                isSkip = false;
                break;
            }

            t += 0.02f;
            upperEgg.transform.localPosition = new Vector3(0, QuadEase(1.667f, 8.625f, t), -0.1f);
            underEgg.transform.localPosition = new Vector3(0, QuadEase(1.667f, -5.375f, t), -0.1f);
            yield return new WaitForFixedUpdate();
        }
        isSkippable = false;

        yield return new WaitForSeconds(1);

        particleObj.SetActive(true);
        if (cm.rarity == 1)
        {
            StartCoroutine("whiteOutAndShowChar");
            while (t >= 0)
            {
                t -= 0.05f;
                setEggAlpha(t);
                yield return new WaitForFixedUpdate();
            }
            backGround.color = new Color(255, 255, 255, 0);

        }
        else
        {
            StartCoroutine("charText");
            while (t >= 0)
            {
                t -= 0.05f;
                setEggAlpha(t);
                backGround.color = new Color(255, 255, 255, t);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    IEnumerator charText()
    {
        skillLabel.text = cm.skillname;
        yield return new WaitForSeconds(3);
        StartCoroutine("whiteOutAndShowChar");
    }

    IEnumerator whiteOutAndShowChar()
    {
        overPanelObj.SetActive(true);

        float t = 0;
        while (t <= 1)
        {
            t += 0.05f;
            overPanel.color = new Color(255, 255, 255, t);
            yield return new WaitForFixedUpdate();
        }

        NextResult();
        resultImageObj.SetActive(true);
        setNameAlpha(1);
        skillLabel.text = "";

        while (t >= 0)
        {
            t -= 0.05f;
            overPanel.color = new Color(255, 255, 255, t);
            yield return new WaitForFixedUpdate();
        }

        isSkippable = true;

        while (!isSkip)
        {
            yield return null;
        }

        particleObj.SetActive(false);
        resultImageObj.SetActive(false);

        if (resultIndex < 10)
        {
            StartCoroutine("slideEgg");
            isSkip = false;
        }
        else
        {
            Skip();

            yield break;
        }
    }

    public void onButtonPressed10()
    {
        result10.SetActive(false);

        int resR = 3;
        int res;
        foreach (GameObject gachaobj in gachaobjs)
        {
            CharacterModel gachacharacter;
            float f = UnityEngine.Random.Range(0, 1f);
            float prob = 0;
            for (int j = 0; j < 3; j++)
            {
                prob += probVec[j];
                if (f <= prob)
                {
                    resR = j;
                    break;
                }
            }
            switch (resR)
            {
                case 0:
                    res = UnityEngine.Random.Range(0, R3.Count);
                    gachacharacter = R3[res];
                    break;

                case 1:
                    res = UnityEngine.Random.Range(0, R2.Count);
                    gachacharacter = R2[res];
                    break;

                default:
                    res = UnityEngine.Random.Range(0, R1.Count);
                    gachacharacter = R1[res];
                    break;
            }

            result[resultIndex] = gachacharacter.id;
            resultIndex++;

            gachaobj.transform.Find("Rarity").gameObject.GetComponentInChildren<Image>().sprite = rareSprites[gachacharacter.rarity - 1];
            gachaobj.transform.Find("Icon").gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + gachacharacter.id);
        }

        for (int i = 0; i < 10; i++)
        {
            CharacterModel a = Common.characters[result[i]];
            eggs[i].material = eggMats[a.rarity - 1];
        }

        resultIndex = 0;
        isResultShowing = true;
    }

    public void NextResult()
    {
        CharacterModel cm = Common.characters[result[resultIndex]];
        resultImage.sprite = Resources.Load<Sprite>("Images/gacha/CharImg/" + result[resultIndex]);
        backGround.sprite = backGrounds[cm.rarity];
        nameLabel.text = cm.name;
        setNameAlpha(1);
        rareImg.sprite = rareSprites[cm.rarity - 1];
        resultIndex++;
    }

    public void Hikinaoshi(GameObject obj)
    {
        obj.SetActive(false);
        hinge.transform.rotation = Quaternion.Euler(0, 0, 20);
        resultIndex = 0;
        resultImageObj.SetActive(false);
    }

    public void Skip()
    {
        StopAllCoroutines();
        setNameAlpha(0);
        backGround.color = new Color(255, 255, 255, 1);
        backGround.sprite = bgImage;
        nameLabel.text = "";
        skillLabel.text = "";
        isResultShowing = false;
        resultImageObj.SetActive(false);
        result10.SetActive(true);
        resultIndex = 0;
        overPanelObj.SetActive(false);
        skipBtn.SetActive(false);
        incubator.SetActive(false);
        particleObj.SetActive(false);
        setEggAlpha(0);
    }

    void setNameAlpha(int a)
    {
        rareImg.color = new Color(255, 255, 255, a);
        nameBox.color = new Color(255, 255, 255, a);
    }

    void setEggAlpha(float a)
    {
        charDot.color = new Color(255, 255, 255, a);
        upperEgg.color = new Color(255, 255, 255, a);
        underEgg.color = new Color(255, 255, 255, a);
    }
    public void GotoGachaUnit()
    {
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        GachaUnitManager.initid = result;
        GachaUnitManager.teachers.Clear();
        //ここで殿堂入りバードル一覧を取得するAPIを呼び出す
        GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
        getCompletedWebClient.target = "gachaunit";
        StartCoroutine(getCompletedWebClient.Send());
        //↑Temporary Code
    }

    float QuadEase(float y1, float y2, float t)
    {
        t = Mathf.Clamp01(t);
        float y = (y2 - y1) * (2 * t - t * t) + y1;
        return y;
    }

    float QuadPassEase(float y1, float y2, float t0, float t)
    {
        t = Mathf.Clamp01(t);
        float y = (y2 - y1) * (2 * t0 * t - t * t) / (2 * t0 - 1) + y1;
        return y;
    }
}
