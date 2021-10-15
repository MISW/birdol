using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RunGacha : MonoBehaviour
{
    //�������jA
    List<CharacterModel> R3, R2, R1; //�������z�񂾂ƃG�f�B�^�\�ł�����Ȃ��̂ŁA���A�x���Ƃɔz����쐬
    float[] probVec = { 0.1f, 0.25f, 0.65f };
    //�������jB
    //List<CharacterModel> Kuji = new List<CharacterModel>();
    GameObject result10, resultImage;
    GameObject[] gachaobjs = new GameObject[10];
    CanvasScaler cs;
    int resultIndex; //結果を0から9まで
    int[] result = new int[10]; //結果
    bool isResultShowing;
    Text nameLabel;
    Image rareImg, nameBox;

    void Awake()
    {
        result10 = GameObject.Find("GachaResults");
        resultImage = GameObject.Find("Image");
        gachaobjs = GameObject.FindGameObjectsWithTag("Gacha");
        nameLabel = GameObject.Find("NameLabel").GetComponent<Text>();
        rareImg = GameObject.Find("Rare").GetComponent<Image>();
        nameBox = GameObject.Find("NameBox").GetComponent<Image>();
        rareImg.color = new Color(255, 255, 255, 0);
        nameBox.color = new Color(255, 255, 255, 0);

        cs = GameObject.Find("Canvas").GetComponent<CanvasScaler>();

        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
    }

    void Start()
    {
        resultIndex = 0;
        result10.SetActive(false);
        resultImage.SetActive(false);
        isResultShowing = false;
        nameLabel.text = "";


        R3 = new List<CharacterModel>();
        R2 = new List<CharacterModel>();
        R1 = new List<CharacterModel>();
        if (Common.characters == null) Common.initCharacters();//Test Only
        for (int i = 0; i < 32; i++)
        {   //�������jA
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
            //�������jB
            /*
            for (int j = 0; j < 5; j++)
            {
                Kuji.Add(character);
            }*/

        }

        // Kuji = Kuji.OrderBy(a => Guid.NewGuid()).ToList();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isResultShowing)
                {
                    NextResult();
                }
                else if (!result10.activeSelf)
                {
                    onButtonPressed10();
                }
            }
        }
    }



    public void onButtonPressed10()
    {
        result10.SetActive(false);

        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

        int resR = 3; //���I���A�x
        int res; //���I�A�C�e���ԍ�
        foreach (GameObject gachaobj in gachaobjs/*GameObject.FindGameObjectsWithTag("Gacha")*/)
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
                    /*   gachaobj.GetComponentInChildren<Text>().text = "SSR";
                       gachaobj.GetComponentInChildren<Text>().color = Color.red;*/
                    break;

                case 1:
                    res = UnityEngine.Random.Range(0, R2.Count);
                    gachacharacter = R2[res];
                    /*   gachaobj.GetComponentInChildren<Text>().text = "SR";
                       gachaobj.GetComponentInChildren<Text>().color = Color.yellow;*/
                    break;

                default:
                    res = UnityEngine.Random.Range(0, R1.Count);
                    gachacharacter = R1[res];
                    /*   gachaobj.GetComponentInChildren<Text>().text = "R";
                       gachaobj.GetComponentInChildren<Text>().color = Color.black;*/
                    break;
            }

            result[resultIndex] = gachacharacter.id;
            resultIndex++;

            /*
            int index= UnityEngine.Random.Range(0, 100);
            gachacharacter = Kuji[index];
            */
            switch (gachacharacter.rarity)
            {
                case 1:
                    gachaobj.transform.Find("Rarity").gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/gacha/R");
                    break;

                case 2:
                    gachaobj.transform.Find("Rarity").gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/gacha/SR");
                    break;

                default:
                    gachaobj.transform.Find("Rarity").gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/gacha/SSR");
                    break;
            }



            gachaobj.transform.Find("Icon").gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + gachacharacter.id);
        }

        resultImage.SetActive(true);

        resultIndex = 0;
        NextResult();
        isResultShowing = true;
    }

    public void NextResult()
    {
        if (resultIndex < 10)
        {
            CharacterModel naaa = Common.characters[result[resultIndex]];
            resultImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gacha/CharImg/" + result[resultIndex]);

            nameLabel.text = naaa.name;
            nameBox.color = new Color(255, 255, 255, 255);


            switch (naaa.rarity)
            {
                case 3:
                    rareImg.sprite = Resources.Load<Sprite>("Images/gacha/SSR");
                    break;

                case 2:
                    rareImg.sprite = Resources.Load<Sprite>("Images/gacha/SR");
                    break;

                default:
                    rareImg.sprite = Resources.Load<Sprite>("Images/gacha/R");
                    break;
            }
            rareImg.color = new Color(255, 255, 255, 255);

        }

        resultIndex++;

        if (resultIndex == 11)
        {
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            rareImg.color = new Color(255, 255, 255, 0);
            nameBox.color = new Color(255, 255, 255, 0);
            nameLabel.text = "";
            isResultShowing = false;
            resultImage.SetActive(false);
            result10.SetActive(true);
            resultIndex = 0;
        }
    }

    public void Hikinaoshi(GameObject obj)
    {
        obj.SetActive(false);
        resultIndex = 0;
    }

    public void GotoGachaUnit()
    {
        Common.loadingCanvas.SetActive(true);
        GachaUnitManager.initid = result;
        GachaUnitManager.teachers.Clear();
        //ここで殿堂入りバードル一覧を取得するAPIを呼び出す
        for (int i = 0; i < 4; i++)
        {
            DendouModel dendouModel = new DendouModel();
            dendouModel.MainCharacterId = i;
            dendouModel.SupportCharacterId = i;
            dendouModel.Name = Common.characters[i].name;
            dendouModel.Vocal = Common.characters[i].vocal;
            dendouModel.Visual = Common.characters[i].visual;
            dendouModel.Dance = Common.characters[i].dance;
            GachaUnitManager.teachers.Add(dendouModel);
        }
        //↑Temporary Code
        Manager.manager.StateQueue((int)gamestate.GachaUnit);
    }
}
