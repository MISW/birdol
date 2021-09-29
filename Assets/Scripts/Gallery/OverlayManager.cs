using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 各キャラクターをクリックしたときの詳細画面を管理
public class OverlayManager : MonoBehaviour
{
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

    public void OpenOverlay(CharacterModel model) {
        // オーバーレイを開いたときに発火
        // 渡されたキャラクターのデータに書き換える
    }
}
