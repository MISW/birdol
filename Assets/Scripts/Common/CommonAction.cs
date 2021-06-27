using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonAction : MonoBehaviour
{

    public string scene;

    public void OnClick() { 
        SceneManager.LoadScene(scene);
    }
}
