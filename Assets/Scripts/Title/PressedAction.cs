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

            Manager.manager.StateQueue((int)gamestate.Signup); //アカウント新規登録(またはアカウント連携)を行うSignupシーンへ遷移する
        }
        else
        {
            /*
             *進行中のストーリーがあればホームに遷移し、なければガチャに遷移する、進捗が(ここはv2の保存用APIの改修が終わってからのほうがよさそう?) 
             */
            Debug.LogError("開発中(未実装)です。PlayerPrefsデータを初期化してください。 Click \"Edit>Clear All PlayerPrefs\" in Unity.");
        }

#if false
            /*
             * トークン認証(セッション更新)について、例をここにあげておきます。使わなければ消しておいてください。 (b^-^)b
             * 
             */
            TokenAuthorizeWebClient webClient = new TokenAuthorizeWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/auth");
            StartCoroutine(webClient.Send());
            while (webClient.isInProgress) continue; //認証の結果待ち。他に、StartCoroutine(webClient.Send())の前にyield returnをつけるというやり方もあります。
            if (webClient.IsAuthorizeSuccess) //認証(更新)成功
            {
                //HomeシーンかGachaシーンへ遷移
            }
            else //失敗
            {
                Debug.Log("トークン認証に失敗しました。アカウント作成またはアカウント連携が必要です。");
                //Signupシーンへ遷移すると良さそう
                yield break;
            }
            yield break;
#endif

    }
}
