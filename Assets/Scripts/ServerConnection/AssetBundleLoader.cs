using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoader
{

    private static string bundleURL = "https://birdol-client:eHNfw8EXao283mgE6ggv@birdol-cdn.nanamiiiii.dev/android";
    public static IEnumerator DownloadAndCache(GameObject downloadingCanvas)
    {
        // Wait for the Caching system to be ready
        while (!Caching.ready)
        {
            yield return null;
        }

        UnityWebRequest www = UnityWebRequest.Get(bundleURL + ".manifest");
        Debug.Log("Loading manifest:" + bundleURL + ".manifest");

        // wait for load to finish
        yield return www.SendWebRequest();

        // if received error, exit
        if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("www error: " + www.error);
            www.Dispose();
            yield break;
        }


        // create empty hash string
        Hash128 hashString = (default(Hash128));// new Hash128(0, 0, 0, 0);

        // check if received data contains 'ManifestFileVersion'
        if (www.downloadHandler.text.Contains("ManifestFileVersion"))
        {
            // extract hash string from the received data, TODO should add some error checking here
            var hashRow = www.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
            Debug.Log("Hash:"+hashRow);
            hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());

            if (hashString.isValid == true)
            {
                // we can check if there is cached version or not
                if (Caching.IsVersionCached(bundleURL, hashString) == true)
                {
                    Debug.Log("Bundle with this hash is already cached!");
                }
                else
                {
                    downloadingCanvas.SetActive(true);
                    Debug.Log("No cached version founded for this hash..");
                }
            }
            else
            {
                // invalid loaded hash, just try loading latest bundle
                Debug.LogError("Invalid hash:" + hashString);
                yield break;
            }

        }
        else
        {
            Debug.LogError("Manifest doesn't contain string 'ManifestFileVersion': " + bundleURL + ".manifest");
            yield break;
        }

        // now download the actual bundle, with hashString parameter it uses cached version if available
        www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);

        // wait for load to finish
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            Debug.LogError("www error: " + www.error);
            www.Dispose();
            www = null;
            yield break;
        }

        // get bundle from downloadhandler
        Common.bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
        Common.initCharacters();
        Common.initSounds();
        downloadingCanvas.SetActive(false);
        CheckVersionWebClient checkUpdate = new CheckVersionWebClient(WebClient.HttpRequestMethod.Post, $"/api/{Common.api_version}/cli/version");
#if UNITY_ANDROID
        checkUpdate.SetData("Android", Common.version, "20220322");
#elif UNITY_IPHONE
            checkUpdate.SetData("iOS",Common.version,"20220322");
#else
            checkUpdate.SetData("Win",Common.version,"20220322");
#endif
        yield return checkUpdate.Send();
    }

}
