using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// - TokenAuthorize to update session:
///     - if Success, jump to menu scene in game. 
///     - If Failed, the user is new to this game.The user need to Signup or LinkAccount! 
/// </summary>
public class TokenAuthorizePrefabController : MonoBehaviour
{
    private void Awake()
    {
        if (string.IsNullOrEmpty(Common.AccessToken)) //ログインを試みるためのアクセストークンが保存されていない
        {
            Debug.Log("アクセストークンが保存されていません。アカウントの作成(またはアカウント連携)が必要です。");
        }
        else //アクセストークンを用いてログインを試みる
        {
            StartCoroutine(Authorize());
        }
    }

    private IEnumerator Authorize()
    {
        TokenAuthorizeWebClient webClient = new TokenAuthorizeWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/auth");
        yield return StartCoroutine(webClient.Send());
        if (webClient.IsAuthorizeSuccess)
        {
            //TODO: Menuシーンへ遷移 
        }
        else
        {
            Debug.Log("アクセストークンによるログインに失敗したため、アカウント作成(またはアカウント連携)が必要です。");
            yield break;
        }
        yield break;
    }
}
