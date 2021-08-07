using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// 参考
/// https://kan-kikuchi.hatenablog.com/entry/playModeStateChanged
/// </summary>

//[InitializeOnLoad]//エディター起動時にコンストラクタが呼ばれるように
public static class SceneTester
{

    /// <summary>
    /// コンストラクタ(InitializeOnLoad属性によりエディター起動時に呼び出される)
    /// </summary>
    static SceneTester()
    {
        //playModeStateChangedイベントにメソッド登録
       // EditorApplication.playModeStateChanged += OnTestPlayStarted;

    }

    /*
    //プレイモードが変更された
    private static void OnTestPlayStarted(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Debug.LogWarning("TestModeによる起動:" + SceneManager.GetActiveScene().name);

                SceneVisor Visor = null;

                GameObject @object = GameObject.FindGameObjectWithTag("SceneVisor");
                if (@object != null)
                {
                    Visor = @object.GetComponent<SceneVisor>();
                }
                else
                {
                    Debug.LogError("シーンには必ず一つのSceneVisorを設置することが必要");
                }

                Visor.TestFunction();



            }
        }


    }*/

}
