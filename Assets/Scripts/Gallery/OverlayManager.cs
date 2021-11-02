using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 各キャラクターをクリックしたときの詳細画面を管理するクラス
/// </summary>
public class OverlayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject standImageObject;
    [SerializeField]
    private GameObject characterNameObject;

    [SerializeField]
    private GameObject orderNameObject;
    [SerializeField]
    private GameObject familyNameObject;
    [SerializeField]
    private GameObject descObject;
    [SerializeField]
    private GameObject groupNameObject;
    [SerializeField]
    private GameObject descBackgroundObject;

    private const int IMAGE_HEIGHT_MIN = 600;
    private const int BG_WIDTH = 600;
    private const int BG_HEIGHT_MIN = 200;

    private const int FONT_SIZE_MAX = 32;
    private const int FONT_SIZE_MIN = 24;

    private float deviceRatio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // キャラ画面をクリックすると図鑑画面に戻る
        if (Input.GetMouseButton(0)) {
            Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
            this.gameObject.SetActive(false);
        }
    }

    private const string pathToResources = "Assets/Resources";
    private const string standImagePath = "Images/standimage/";
    public void OpenOverlay(CharacterModel model) {
        // オーバーレイを開いたときに発火
        // 渡されたキャラクターのデータに書き換える
#if UNITY_EDITOR
        Debug.Log(model.id + ": " + model.name);
#endif
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Image im = standImageObject.GetComponent<Image>();
#if UNITY_ANDROID
        Sprite sp = Common.assetBundle.LoadAsset<Sprite>(pathToResources + standImagePath + model.id + ".png");
#else
        Sprite sp = Resources.Load<Sprite>(standImagePath + model.id);
#endif

        deviceRatio = Screen.currentResolution.height;
        deviceRatio /= Screen.currentResolution.width;

        if (sp != null) {
            im.sprite = sp;
        }

        // 名前書き換え
        var data = GalleryManager.GetLine(model.id);
        characterNameObject.GetComponent<Text>().text = data[GalleryManager.NAME_INDEX];
        orderNameObject.GetComponent<Text>().text = data[GalleryManager.ORDER_INDEX];
        familyNameObject.GetComponent<Text>().text = data[GalleryManager.FAMILY_INDEX];

        string groupStr = data[GalleryManager.CLASS_INDEX];
        groupNameObject.GetComponent<Text>().text = "【" + groupStr + "】";

        Text textDesc = descObject.GetComponent<Text>();

        string raw = data[GalleryManager.DESC_INDEX];
        textDesc.text = raw;

        // ---- 無理やり説明文のサイズの可変にしてる

        textDesc.fontSize = FONT_SIZE_MIN + (int)Math.Round((FONT_SIZE_MAX - FONT_SIZE_MIN) * (deviceRatio - 1.0));
        RectTransform descBg = descBackgroundObject.GetComponent<RectTransform>();
        descBg.sizeDelta = new Vector2(BG_WIDTH, deviceRatio * BG_HEIGHT_MIN);

        // ----

        this.gameObject.SetActive(true);
    }
}
