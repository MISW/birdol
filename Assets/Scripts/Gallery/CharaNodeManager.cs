using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaNodeManager : MonoBehaviour
{
    private CharacterModel character = null;
    private bool isUnlocked = false;

    private GameObject overlay;
    private OverlayManager overlayManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake() {
        // GameObject.FindだとinactiveなGameObjectを探せないのでこうなっている
        var tmp = GameObject.Find("Canvas");
        overlay = tmp.transform.Find("Overlay").gameObject;
        overlayManager = overlay.GetComponent<OverlayManager>();
    }

    // キャラをタップしたときの処理
    // オーバーレイ画面を開く
    public void OnClick() {
#if UNITY_EDITOR
        // Debug.Log("clicked " + this.character.id);
#endif

        if (this.isUnlocked) overlayManager.OpenOverlay(this.character);
    }

    // setterにキャラアイコン変更の処理を付けている
    private const string galleryPathBase = "Images/gallery/";
    private const string lockedPath = galleryPathBase + "face/locked";  // 未解禁キャラクター用
    public void SetCharacter(CharacterModel model) {
        this.character = model;

        GameObject faceObject = this.transform.Find("Panel/Face").gameObject;
        GameObject nameObject = this.transform.Find("Panel/Text").gameObject;

        Image faceImage = faceObject.GetComponent<Image>();
#if UNITY_ANDROID
        Sprite faceSprite = Common.assetBundle.LoadAsset<Sprite>("Assets/Resources/Images/charactericon/" + model.id + ".png");
        Sprite lockedSprite = Common.assetBundle.LoadAsset<Sprite>("loacked");
#else
        Sprite faceSprite = Resources.Load<Sprite>("Images/charactericon/" + model.id);
        Sprite lockedSprite = Resources.Load<Sprite>(lockedPath);
#endif

        Text nameText = nameObject.GetComponent<Text>();

        this.isUnlocked = GalleryManager.GetIsUnlocked(model.id);
        if (!this.isUnlocked) {
            faceImage.sprite = lockedSprite;
            nameText.text = "???";
        }
        else {
            if (faceSprite != null) faceImage.sprite = faceSprite;
            nameText.text = model.name;
        }
    }

    // オーバーレイに表示する
    public CharacterModel GetCharacter() {
        return this.character;
    }
}
