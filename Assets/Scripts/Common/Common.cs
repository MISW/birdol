using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Common : MonoBehaviour
{
    public static CharacterModel[] characters;

    public static void initCharacters()
    {
        string json = Resources.Load<TextAsset>("common/characters").ToString();
        CommonCharacters result = JsonUtility.FromJson<CommonCharacters>(json);
        characters = result.characters;
    }

    void Start()
    {
        initCharacters();
    }
}
