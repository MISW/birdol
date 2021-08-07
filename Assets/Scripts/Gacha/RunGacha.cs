using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RunGacha : MonoBehaviour
{
    //実装方針A
    List<CharacterModel> R3,R2,R1; //多次元配列だとエディタ―でいじれないので、レア度ごとに配列を作成
    float[] probVec = { 0.1f, 0.25f, 0.65f};
    //実装方針B
    //List<CharacterModel> Kuji = new List<CharacterModel>();


    void Start()
    {
        
        R3 = new List<CharacterModel>();
        R2 = new List<CharacterModel>();
        R1 = new List<CharacterModel>();
        if (Common.characters == null) Common.initCharacters();//Test Only
        for (int i=0;i<20;i++)
        {   //実装方針A
            CharacterModel character = Common.characters[i];
            if(character.rarity == 1)
            {
                R1.Add(character);
                
            }else if (character.rarity == 2)
            {
                R2.Add(character);

            }
            else
            {
                R3.Add(character);
            }
            //実装方針B
            /*
            for (int j = 0; j < 5; j++)
            {
                Kuji.Add(character);
            }*/

        }

       // Kuji = Kuji.OrderBy(a => Guid.NewGuid()).ToList();
    }

    

    public void onButtonPressed10()
    {
        int resR = 3; //当選レア度
        int res; //当選アイテム番号
        foreach (GameObject gachaobj in GameObject.FindGameObjectsWithTag("Gacha"))
        {
            CharacterModel gachacharacter;
            float f = UnityEngine.Random.Range(0, 1f);
            float prob = 0;
            for (int j = 0; j < 3; j++)
            {
                prob += probVec[j];
                if (f <= prob)
                {
                    resR = j; //当選レア度を決定（0が最高、4が最低であることに注意）
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
        
    }
}
