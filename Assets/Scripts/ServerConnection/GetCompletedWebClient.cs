using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetCompletedWebClient : GameWebClient
{

    public string target = "";

    [Serializable]
    public struct GetCompletedResponse
    {
        [SerializeField] public string result;
        [SerializeField] public string error;
        [SerializeField] public DendouModel[] characters;
    }

    public GetCompletedWebClient(HttpRequestMethod requestMethod, string path) : base(requestMethod, path)
    {
    }

    public override bool CheckRequestData()
    {
        return true;
    }

    protected override void HandleGameSuccessData(string response)
    {
        GetCompletedResponse r = JsonUtility.FromJson<GetCompletedResponse>(response);
        base.data = r;
        if (r.characters.Length >= 4)
        {
            if (target == "gachaunit")
            {
                GachaUnitManager.teachers.Clear();
                foreach (DendouModel character in r.characters)
                {
#if UNITY_EDITOR
                    Debug.Log("character:"+character.Name);
#endif
                    GachaUnitManager.teachers.Add(character);
                }
                Manager.manager.StateQueue((int)gamestate.GachaUnit);
            }else if(target == "completed")
            {
                CompletedController.CompletedCharacters.Clear();
                foreach (DendouModel character in r.characters)
                {
#if UNITY_EDITOR
                    Debug.Log("character:" + character.Name);
#endif
                    CompletedController.CompletedCharacters.Add(character);
                }
                Manager.manager.StateQueue((int)gamestate.CompletedCharacters);
            }else if(target == "freeselect")
            {
                FreeSelectManager.CompletedCharacters.Clear();
                foreach (DendouModel character in r.characters)
                {
#if UNITY_EDITOR
                    Debug.Log("character:" + character.Name);
#endif
                    FreeSelectManager.CompletedCharacters.Add(character);
                }
                Manager.manager.StateQueue((int)gamestate.FreeSelect);
            }
            else if (target == "home")
            {
                HomeUtil.isUnlocked = new List<bool>(new bool[32]);
                foreach (DendouModel character in r.characters)
                {
                    HomeUtil.isUnlocked[character.MainCharacterId] = true;
                }
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Connection Error Occured.");
#endif
        }

    }

    protected override void HandleGameSetupWebRequestData(UnityWebRequest www)
    {
        //www.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(" "));
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        SetAuthenticationHeader(www);
    }
}