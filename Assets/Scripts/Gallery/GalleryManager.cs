using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    private const int ARR_SIZE = 10;
    // 図鑑に表示されるバードルのリスト
    private CharacterModel[] characters = new CharacterModel[ARR_SIZE];
    private bool[] isUnlocked = new bool[ARR_SIZE];

    private GameObject prefabRowNode;
    private GameObject content;  // Scroll ViewのContent

    // キャラごとの説明文はどう保持する？

    // 図鑑の初期化
    void InitList()
    {
        prefabRowNode = (GameObject) Resources.Load("prefab/gallery/RowNode");
        content = GameObject.Find("Content");

        // ceil(キャラの数/2)だけRowNodeをScroll ViewのContent配下に生成する
        // そのあとに画像と名前差し替え
        // ↑できる？
        // キャラ数が奇数なら一番下のRowNodeのCharaNodeRightをinvisibleに

        int Nodes = (ARR_SIZE + 1) / 2;

        for (int i = 0; i < Nodes; i++)
        {
            GameObject row = Instantiate(prefabRowNode);
            row.transform.parent = content.transform;
        }
        
        // 仮データ用意
        for (int i = 0; i < ARR_SIZE; i++)
        {
            var tmp = new CharacterModel();
            tmp.id = i;
            tmp.name = "Data-" + i;

            characters[i] = tmp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
