using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressedAction : MonoBehaviour
{
    

    private void Start()
    {
        Common.bgmplayer.time = 0;
        Common.bgmplayer.clip = (AudioClip)Resources.Load("Music/TM01");
        Common.bgmplayer.Play();
    }


    IEnumerator LoginAndSync()
    {
        TokenAuthorizeWebClient tokenAuthorizeWebClient = new TokenAuthorizeWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/auth");
        tokenAuthorizeWebClient.IsDoBackToTitleIfTokenRefreshError = false; //これがtrueのままだと、SignupシーンではなくTitleシーンへ遷移してしまう
        yield return tokenAuthorizeWebClient.Send();
        if (tokenAuthorizeWebClient.IsAuthorizeSuccess && Common.SessionID != null) //ログイン成功。アクセストークンが認証されたかつSessionIDが保存されたことを確かめている。
        {
            GetCompletedWebClient getCompletedWebClient = new GetCompletedWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/complete?session_id=" + Common.SessionID);
            getCompletedWebClient.target = "home";
            yield return getCompletedWebClient.Send();
            GetStoryWebClient getStoryWebClient = new GetStoryWebClient(WebClient.HttpRequestMethod.Get, $"/api/{Common.api_version}/gamedata/story?session_id=" + Common.SessionID);
            yield return getStoryWebClient.Send();
        }
        else if(!tokenAuthorizeWebClient.IsAuthorizeSuccess && tokenAuthorizeWebClient.isSuccess) //通信は成功したが、ログイン失敗 -> アカウント未生成と判断し、Signupシーンへ遷移する。
        {
            Common.loadingCanvas.SetActive(true);
            Common.loadingGif.GetComponent<GifPlayer>().index = 0;
            Common.loadingGif.GetComponent<GifPlayer>().StartGif();
            Common.bgmplayer.Stop();
            Common.bgmplayer.time = 0;
            Manager.manager.StateQueue((int)gamestate.Signup);
        }
        else //通信自体が失敗。これはGameWebClientの方で対処するためここでは何も書かなくて大丈夫なはず。
        {
        }
        
    }


    public void OnClick() {
        //ここを変える
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        if (Common.UserID == 0)
        {
            Manager.manager.StateQueue((int)gamestate.Signup); //アカウント新規登録(またはアカウント連携)を行うSignupシーンへ遷移する
        }
        else
        {
            StartCoroutine(LoginAndSync());
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
