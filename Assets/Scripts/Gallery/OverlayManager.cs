using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// 各キャラクターをクリックしたときの詳細画面を管理
public class OverlayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject standImageObject;
    [SerializeField]
    private GameObject characterNameObject;

    [SerializeField]
    private GameObject galleryManager;
    private GalleryManager man;

    [SerializeField]
    private GameObject orderNameObject;
    [SerializeField]
    private GameObject familyNameObject;
    [SerializeField]
    private GameObject descObject;

    private const int IMAGE_HEIGHT = 1200;

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

        // inactive状態だとStartが呼ばれない（っぽい）ので初期化しておく
        // しないとNullReferenceになる
        if (man == null) man = galleryManager.GetComponent<GalleryManager>();

        Image im = standImageObject.GetComponent<Image>();
        Sprite sp = Resources.Load<Sprite>(standImagePath + model.id);

        if (sp != null) {
            im.sprite = sp;

            // キャラの立ち絵のサイズによって幅を変えないと見た目が壊れる
            // todo: 位置調整
            float r = sp.texture.width;
            r /= sp.texture.height;

            RectTransform rectTransform = standImageObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(r * IMAGE_HEIGHT, IMAGE_HEIGHT);

            // キャラによってサイズが違うのでスケール変えたりしようかな…？
        }

        // 名前書き換え
        var data = man.GetLine(model.id);
        characterNameObject.GetComponent<Text>().text = data[GalleryManager.NAME_INDEX];
        orderNameObject.GetComponent<Text>().text = data[GalleryManager.ORDER_INDEX];
        familyNameObject.GetComponent<Text>().text = data[GalleryManager.FAMILY_INDEX];
        descObject.GetComponent<Text>().text = data[GalleryManager.DESC_INDEX];

        this.gameObject.SetActive(true);
    }
}
