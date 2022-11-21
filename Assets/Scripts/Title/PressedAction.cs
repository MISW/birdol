using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressedAction : MonoBehaviour
{

    public GameObject NewUpdate;

    private void Start()
    {
        if (Common.hasUpdate)
        {
            NewUpdate.SetActive(true);
        }
        Common.bgmplayer.time = 0;
        Common.bgmplayer.clip = Common.bundle.LoadAsset<AudioClip>("TM01");
        Common.bgmplayer.Play();
    }

    private IEnumerator SignUp()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Manager.manager.StateQueue((int)gamestate.Signup);
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
            Manager.manager.StateQueue((int)gamestate.Signup);
        }
        else //通信自体が失敗。これはGameWebClientの方で対処するためここでは何も書かなくて大丈夫なはず。
        {
        }

    }

    /// <summary>
    /// Refresh signing key using deprecated algorithm.
    /// This quietly execute account link process to regenerate key with new algorithm.
    /// </summary>
    private IEnumerator RefreshSigningKey()
    {
        /* Token Authorization */
        TokenAuthorizeWebClient tokenAuthClient = new TokenAuthorizeWebClient(
            WebClient.HttpRequestMethod.Get, 
            $"/api/{Common.api_version}/auth");

        tokenAuthClient.IsDoBackToTitleIfTokenRefreshError = false;
        yield return tokenAuthClient.Send();

        if (tokenAuthClient.IsAuthorizeSuccess && Common.SessionID != null) // Token Auth Succeeded
        {
            SetAccountLinkWebClient setAccLinkClient = new SetAccountLinkWebClient(
                WebClient.HttpRequestMethod.Put, 
                $"/api/{Common.api_version}/auth");
            
            /* Use randomly generated string for password */
            string passwd = Common.GenerateRondomString(64);
            setAccLinkClient.SetData(passwd);

            if (!setAccLinkClient.CheckRequestData())
            {
                /* This check is absolutely passed
                   Process never enter this block  */

                /* Back to title */

                NetworkErrorDialogController.OpenConfirmDialog(() => {
                    Manager.manager.StateQueue((int)gamestate.Title);
                },
                "不明なエラーが発生しました。\nタイトルへ戻ります。");
            }
            yield return setAccLinkClient.Send();

            if (setAccLinkClient.isSuccess && 
                !setAccLinkClient.isInProgress && 
                setAccLinkClient.isSetAccountLinkSuccess) // Link Preparation Succeeded
            {
                LinkAccountWebClient linkAccClient = new LinkAccountWebClient(
                    WebClient.HttpRequestMethod.Post, 
                    $"/api/{Common.api_version}/user");

                string accountId = Common.DefaultAccountID;
                string uuid = System.Guid.NewGuid().ToString();
                (string privKey, string pubKey) rsaKeyPair = Common.CreateRsaKeyPair();
                linkAccClient.SetData(accountId, passwd, uuid, 
                                      rsaKeyPair.pubKey, rsaKeyPair.privKey);
                if (!linkAccClient.CheckRequestData())
                {
                    /* Back to title */
                    NetworkErrorDialogController.OpenConfirmDialog(() => {
                        Manager.manager.StateQueue((int)gamestate.Title);
                    },
                    "不明なエラーが発生しました。\nタイトルへ戻ります。");
                }
                yield return linkAccClient.Send();

                if (linkAccClient.isSuccess && 
                    !linkAccClient.isInProgress && 
                    linkAccClient.isLinkAccountSuccess) // Link Succeeded
                {
                    Common.DefaultAccountID = accountId;
                    Common.Uuid = uuid;
                    Common.RsaKeyPair = rsaKeyPair;
                    Common.SavedKeyType = Common.KEY_RSA4096; // set keytype
                    /* Default Login Process */
                    yield return LoginAndSync();
                }
                else // Some error occured
                {
                    /* Back to title */
                    NetworkErrorDialogController.OpenConfirmDialog(() => {
                        Manager.manager.StateQueue((int)gamestate.Title);
                    },
                    "エラーが発生しました。\nタイトルへ戻ります。");
                }
            }
            else
            {
                /* Back to title */
                NetworkErrorDialogController.OpenConfirmDialog(() => {
                    Manager.manager.StateQueue((int)gamestate.Title);
                },
                "エラーが発生しました。\nタイトルへ戻ります。");
            }
        }
        else if(!tokenAuthClient.IsAuthorizeSuccess && 
                tokenAuthClient.isSuccess)             // invalid account 
        {
            /* Go to signup scene */
            Manager.manager.StateQueue((int)gamestate.Signup);
        }
    }


    public void OnClick() {
        //ここを変える
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        
        if (Common.hasUpdate)
        {
            //ストアへリダイレクト
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
        else
        {
            Common.loadingCanvas.SetActive(true);
            Common.loadingGif.GetComponent<GifPlayer>().index = 0;
            Common.loadingGif.GetComponent<GifPlayer>().StartGif();
            Common.bgmplayer.Stop();
            Common.bgmplayer.time = 0;
            if (Common.UserID == 0)
            {
                StartCoroutine(SignUp()); //アカウント新規登録(またはアカウント連携)を行うSignupシーンへ遷移する
            }
            else if (Common.SavedKeyType != Common.KEY_RSA4096) // key with deprecated algorithm
            {
                StartCoroutine(RefreshSigningKey());   
            }
            else // default login
            {
                StartCoroutine(LoginAndSync());
            }
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
