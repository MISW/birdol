using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 各キャラクターをクリックしたときの詳細画面を管理
public class OverlayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject standImageObject;
    [SerializeField]
    private GameObject characterNameObject;

    private const int IMAGE_HEIGHT = 1200;

    void Awake() {

    }

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

        }

        // 名前書き換え

        this.gameObject.SetActive(true);
    }
}
