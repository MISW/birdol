using System;
using System.Collections;
using System.Collections.Generic;
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

    public static IEnumerator DownloadAndCache(GameObject downloadingCanvas, GameObject downloadFailedAlert)
    {
        UnityWebRequest www = UnityWebRequest.Get(bundleURL + ".manifest?");

        // wait for load to finish
        yield return www.SendWebRequest();

        // if received error, exit
        if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
        {
            www.Dispose();
            www = null;
            yield break;
        }

        // create empty hash string
        Hash128 hashString = (default(Hash128));// new Hash128(0, 0, 0, 0);

        // check if received data contains 'ManifestFileVersion'
        if (www.downloadHandler.text.Contains("ManifestFileVersion"))
        {
            // extract hash string from the received data, TODO should add some error checking here
            var hashRow = www.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
            hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());

            if (hashString.isValid == true)
            {
                // we can check if there is cached version or not
                if (Caching.IsVersionCached(bundleURL, hashString) == false)
                {
                    downloadingCanvas.SetActive(true);
                }
            }
            else
            {
                // invalid loaded hash, just try loading latest bundle
                yield break;
            }
        }
        else
        {
            yield break;
        }

        www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL, hashString, 0);
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            www.Dispose();
            downloadFailedAlert.SetActive(true);
            www = null;
            
            yield break;
        }

        // get bundle from downloadhandle
        Common.bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;

        if (Common.bundle == null)
        {
            downloadFailedAlert.SetActive(true);
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
