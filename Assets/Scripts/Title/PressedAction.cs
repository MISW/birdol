using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressedAction : MonoBehaviour
{

    public void OnClick() {
        //ここを変える
        if (Common.UserID == 0)
        {
            Common.loadingCanvas.SetActive(true);
            /*
             * ここにユーザーの作成するAPI呼び出したほうがいいかも
            
             * 
             */
            Manager.manager.StateQueue((int)gamestate.Gacha);
        }
        else
        {
            /*
             *進行中のストーリーがあればホームに遷移し、なければガチャに遷移する、進捗が(ここはv2の保存用APIの改修が終わってからのほうがよさそう?) 
             */

        }
        
    }
}
