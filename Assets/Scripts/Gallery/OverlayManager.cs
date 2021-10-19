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
    private const int BG_HEIGHT_MIN = 200;

    private const int FONT_SIZE_MAX = 32;
    private const int FONT_SIZE_MIN = 24;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // キャラ画面をクリックすると図鑑画面に戻る
        if (Input.GetMouseButton(0)) {
            this.gameObject.SetActive(false);
        }
    }

    private const string standImagePath = "Images/standimage/";
    public void OpenOverlay(CharacterModel model) {
        // オーバーレイを開いたときに発火
        // 渡されたキャラクターのデータに書き換える
        Debug.Log(model.id + ": " + model.name);

        Image im = standImageObject.GetComponent<Image>();
        Sprite sp = Resources.Load<Sprite>(standImagePath + model.id);

        float deviceRatio = Screen.currentResolution.height;
        deviceRatio /= Screen.currentResolution.width;

        if (sp != null) {
            im.sprite = sp;

            // キャラの立ち絵のサイズによって幅を変えないと見た目が壊れる
            // todo: 位置調整
            float r = sp.texture.width;
            r /= sp.texture.height;

            RectTransform rectTransform = standImageObject.GetComponent<RectTransform>();
            Vector2 baseSizeMin = new Vector2(r * IMAGE_HEIGHT_MIN, IMAGE_HEIGHT_MIN);

            // deviceRatioが1.0のとき600、2.0のとき1200
            // キャラクターの立ち絵のはみ出し防止
            baseSizeMin *= deviceRatio;

            // キャラによってサイズが違うのでスケール変えたりしようかな…？
            Transform tr = standImageObject.transform;
            Vector3 currentPos = new Vector3(-80, 0, 0);
            Vector3 currentScale = new Vector3(1.0f, 1.0f, 1.0f);

            switch (model.id) {
                case 0:
                case 4:
                    currentPos.y += (50 * deviceRatio);
                    currentScale *= 1.1f;
                    break;
                case 1:
                    currentPos.y += (20 * deviceRatio);
                    break;
                case 5:
                    currentPos.y += (125 * deviceRatio);
                    currentScale *= 1.3f;
                    break;
                case 7:
                    currentPos.y += (15 * deviceRatio);
                    break;
                case 8:
                case 9:
                case 12:
                case 20:
                case 22:
                    currentScale *= 0.9f;
                    break;
                case 11:
                    currentPos.x -= 50;
                    currentScale *= 1.5f;
                    break;
                case 13:
                    currentScale.x *= -1;
                    break;
                case 15:
                    currentPos.y += (40 * deviceRatio);
                    currentScale *= 1.1f;
                    break;
                case 18:
                    currentPos.y += (40 * deviceRatio);
                    break;
                case 19:
                    currentScale.x *= -1;
                    break;
                case 25:
                case 31:
                    currentPos.y += (25 * deviceRatio);
                    break;
                case 27:
                    currentScale *= 1.3f;
                    break;
                case 30:
                    currentPos.y += (50 * deviceRatio);
                    break;
                default:
                    break;
            }

            tr.localPosition = currentPos;
            tr.localScale = currentScale;

            rectTransform.sizeDelta = baseSizeMin;
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
        descBg.sizeDelta = new Vector2(600, deviceRatio * BG_HEIGHT_MIN);

        // ----

        this.gameObject.SetActive(true);
    }
}
