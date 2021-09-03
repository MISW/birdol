using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RunGacha : MonoBehaviour
{
    //�������jA
    List<CharacterModel> R3, R2, R1; //�������z�񂾂ƃG�f�B�^�\�ł�����Ȃ��̂ŁA���A�x���Ƃɔz����쐬
    float[] probVec = { 0.1f, 0.25f, 0.65f };
    //�������jB
    //List<CharacterModel> Kuji = new List<CharacterModel>();
    GameObject result10; //10連の結果を全部表示するオブジェクト
    GameObject resultImage; //画面いっぱいに映る立ち絵
    GameObject[] gachaobjs = new GameObject[10];
    public Text rarityNameLabel;
    int resultIndex; //結果を0から9まで
    int[] result = new int[10]; //結果
    bool isResultShowing;

    void Awake()
    {
        result10 = GameObject.Find("Panel");
        resultImage = GameObject.Find("Image");
        gachaobjs = GameObject.FindGameObjectsWithTag("Gacha");
    }

    void Start()
    {
        resultIndex = 0;
        result10.SetActive(false);
        resultImage.SetActive(false);
        isResultShowing = false;
        rarityNameLabel.text = "";

        R3 = new List<CharacterModel>();
        R2 = new List<CharacterModel>();
        R1 = new List<CharacterModel>();
        if (Common.characters == null) Common.initCharacters();//Test Only
        for (int i = 0; i < 20; i++)
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
        /* if (Input.GetMouseButtonUp(0))
         {
             if (isResultShowing)
             {
                 NextResult();
             }
             else
             {
                 onButtonPressed10();
             }
         }*/

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isResultShowing)
                {
                    NextResult();
                }
                else
                {
                    onButtonPressed10();
                }
            }
        }
    }



    public void onButtonPressed10()
    {
        result10.SetActive(false);

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
                    resR = j; //���I���A�x������i0���ō��A4���Œ�ł��邱�Ƃɒ��Ӂj
                    break;
                }
            }
            switch (resR)
            {
                case 0:
                    res = UnityEngine.Random.Range(0, R3.Count);
                    gachacharacter = R3[res];
                    gachaobj.GetComponentInChildren<Text>().text = "SSR";
                    gachaobj.GetComponentInChildren<Text>().color = Color.red;
                    break;

                case 1:
                    res = UnityEngine.Random.Range(0, R2.Count);
                    gachacharacter = R2[res];
                    gachaobj.GetComponentInChildren<Text>().text = "SR";
                    gachaobj.GetComponentInChildren<Text>().color = Color.yellow;
                    break;

                default:
                    res = UnityEngine.Random.Range(0, R1.Count);
                    gachaobj.GetComponentInChildren<Text>().text = "R";
                    gachaobj.GetComponentInChildren<Text>().color = Color.black;
                    gachacharacter = R1[res];
                    break;
            }

            result[resultIndex] = gachacharacter.id;
            resultIndex++;

            /*
            int index= UnityEngine.Random.Range(0, 100);
            gachacharacter = Kuji[index];
            switch (gachacharacter.rarity)
            {
                case 3:
                    gachaobj.GetComponentInChildren<Text>().text = "SSR";
                    gachaobj.GetComponentInChildren<Text>().color = Color.red;
                    break;

                case 2:
                    gachaobj.GetComponentInChildren<Text>().text = "SR";
                    gachaobj.GetComponentInChildren<Text>().color = Color.yellow;
                    break;

                default:
                    gachaobj.GetComponentInChildren<Text>().text = "R";
                    gachaobj.GetComponentInChildren<Text>().color = Color.black;
                    break;
            }*/
            gachaobj.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/gacha/" + gachacharacter.id);
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
            resultImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/gacha/" + result[resultIndex]); //立ち絵がまだないので現時点ではアイコンで代用。立ち絵が追加され次第そちらに変更。
            Debug.Log(naaa.name);
            string rare;
            switch (naaa.rarity)
            {
                case 3:
                    rare = "SSR";
                    rarityNameLabel.color = Color.red;
                    break;

                case 2:
                    rare = "SR";
                    rarityNameLabel.color = Color.yellow;
                    break;

                default:
                    rare = "R";
                    rarityNameLabel.color = Color.black;
                    break;
            }
            rarityNameLabel.text = rare + "\n" + naaa.name;
        }

        resultIndex++;

        if (resultIndex == 11)
        {
            rarityNameLabel.text = "";
            isResultShowing = false;
            resultImage.SetActive(false);
            result10.SetActive(true);
            resultIndex = 0;
        }
    }
}
