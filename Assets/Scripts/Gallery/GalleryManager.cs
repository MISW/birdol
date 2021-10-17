using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/**
To-do:
素材がResourcesに実装されたらBgをbackground2に変える
「さくいん」テキストをアイコンに差し替え
**/

/// <summary>
/// Galleryシーンを管理するクラス
/// </summary>
public class GalleryManager : MonoBehaviour
{
    private const int ARR_SIZE = 32;

    /// <summary>
    /// 図鑑に表示されるバードルのリスト setter/getter有
    /// </summary>
    private static CharacterModel[] characters = new CharacterModel[ARR_SIZE];
    /// <summary>
    /// キャラの解禁済みフラグ setter/getter有
    /// </summary>
    private static bool[] isUnlocked = new bool[ARR_SIZE];

    [SerializeField]
    private GameObject rowNodeOriginal;

    // キャラごとの説明文など
    private TextAsset infoCSV;
    private static List<string[]> csvDatas = new List<string[]>();

    // CSVにアクセスするとき用
    public const int NAME_INDEX = 1;
    public const int ORDER_INDEX = 2;
    public const int FAMILY_INDEX = 3;
    public const int DESC_INDEX = 6;
    public const int CLASS_INDEX = 7;

    /// <summary>
    /// 図鑑データを読み込む
    /// </summary>
    private void LoadCSV() {
        infoCSV = Resources.Load<TextAsset>("GalleryData/info");
        StringReader reader = new StringReader(infoCSV.text);

        while (reader.Peek() != -1) {
            string line = reader.ReadLine();  // 一行ずつ読み込み
            csvDatas.Add(line.Split(','));
        }
    }

    /// <summary>
    /// 図鑑の初期化処理
    /// </summary>
    private void InitList()
    {
        // 仮データ用意
        for (int i = 0; i < ARR_SIZE; i++)
        {
            var tmp = new CharacterModel();
            tmp.id = i;
            tmp.name = GetLine(i)[NAME_INDEX];

            // isUnlocked[i] = true;
            characters[i] = tmp;
        }

        rowNodeOriginal.SetActive(true);
        GameObject content = rowNodeOriginal.transform.parent.gameObject;

        // ceil(キャラの数/2)だけRowNodeをScroll ViewのContent配下に生成する
        int Nodes = (ARR_SIZE + 1) / 2;

        for (int i = 0; i < Nodes-1; i++)
        {
            GameObject row = Instantiate(rowNodeOriginal);
            row.transform.SetParent(content.transform, false);
        }

        // 顔アイコンと名前を各Nodeにセット
        for (int i = 0; i < Nodes; i++) {
            GameObject row = content.transform.GetChild(i).gameObject;
            GameObject left = row.transform.Find("CharaNodeLeft").gameObject;
            GameObject right = row.transform.Find("CharaNodeRight").gameObject;

            // キャラ数が奇数なら一番下のRowNodeのCharaNodeRightをinvisibleに
            if (ARR_SIZE % 2 == 1 && i == Nodes-1) right.SetActive(false);

            SetCharacterData(left, 2*i);
            SetCharacterData(right, 2*i+1);
        }
    }

    /// <summary>
    /// Nodeにキャラクターのデータをセットする
    /// </summary>
    /// <param name="node">操作したいCharaNode</param>
    /// <param name="id">NodeにセットしたいキャラクターのID</param>
    private void SetCharacterData(GameObject node, int id) {
        CharaNodeManager nodeManager = node.GetComponent<CharaNodeManager>();
        nodeManager.SetCharacter(characters[id]);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadCSV();
        InitList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// キャラクターのIdからCharacterModelを取得する
    /// </summary>
    public static CharacterModel GetCharacter(int id) {
        return characters[id];
    }

    /// <summary>
    /// 指定したIdにCharacterModelを設定する
    /// </summary>
    public static void SetCharacter(int id, CharacterModel model) {
        characters[id] = model;
    }

    /// <summary>
    /// キャラクターのIdから解禁状況を取得する
    /// </summary>
    public static bool GetIsUnlocked(int id) {
        return isUnlocked[id];
    }

    /// <summary>
    /// 指定したIdのキャラクターの解禁状況を設定する
    /// </summary>
    public static void SetIsUnlocked(int id, bool value) {
        isUnlocked[id] = value;
    }

    /// <summary>
    /// 指定したIdのキャラクターの図鑑データを取得する
    /// </summary>
    public static string[] GetLine(int id) {
        return csvDatas[id+1];
    }

    /// <summary>
    /// 戻るボタンの挙動
    /// </summary>
    /// 
    public void OnBackClicked()
    {
        Debug.Log("Pushed Gallery");
        Common.loadingCanvas.SetActive(true);
        Manager.manager.StateQueue((int)gamestate.Home);
    }

}
