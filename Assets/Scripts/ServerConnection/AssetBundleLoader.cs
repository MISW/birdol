using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoader : MonoBehaviour
{

#if UNITY_ANDROID
    private static string filename = "android";
#elif UNITY_IOS
    private static string filename = "ios";
#else
    private static string filename = "wsa";
#endif

    private static string bundleURL = "https://birdol-client:eHNfw8EXao283mgE6ggv@birdol-asset.misw.jp/Asset/" + filename;

#if !UNITY_ANDROID || UNITY_EDITOR
    private static string savePath = $"{Application.streamingAssetsPath}/" + filename;
#else
    private static string savePath = $"{Application.streamingAssetsPath}/" + filename;
#endif
    public static IEnumerator DownloadAndCache(GameObject downloadingCanvas, GameObject downloadFailedAlert)
    {
        if (!System.IO.File.Exists(savePath))
        {
            using (var request = UnityWebRequest.Get(bundleURL))
            {
                downloadingCanvas.SetActive(true);
                // DownloadHandlerをファイル用のものに差し替える
                request.downloadHandler = new DownloadHandlerFile(savePath);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
                {
                    System.IO.File.Delete(savePath);
                    Debug.LogError("www error: " + request.error);
                    request.Dispose();
                    downloadFailedAlert.SetActive(true);
                    yield break;
                }
                downloadingCanvas.SetActive(false);
            }
        }
        else
        {
            if (new FileInfo(savePath).Length == 0)
            {
                System.IO.File.Delete(savePath);
                yield return DownloadAndCache(downloadingCanvas, downloadFailedAlert);
                yield break;
            }
        }
        // すでにディスクへの書き込みが終わっているのでAssetBundle.LoadFromFileでも取得できる
        Common.bundle = AssetBundle.LoadFromFile(savePath);
        if (Common.bundle == null)
        {
            System.IO.File.Delete(savePath);
            yield return DownloadAndCache(downloadingCanvas, downloadFailedAlert);
            yield break;
        }
        Common.initCharacters();
        Common.initSounds();
        downloadingCanvas.SetActive(false);
        CheckVersionWebClient checkUpdate = new CheckVersionWebClient(WebClient.HttpRequestMethod.Post, $"/api/{Common.api_version}/cli/version");
#if UNITY_ANDROID
        checkUpdate.SetData("Android", Common.version, "20220322");
#elif UNITY_IOS
        checkUpdate.SetData("iOS",Common.version,"20220322");
#else
        checkUpdate.SetData("Win",Common.version,"20220322");
#endif
        yield return checkUpdate.Send();

    }

}
