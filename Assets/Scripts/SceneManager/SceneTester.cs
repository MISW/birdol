using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// �Q�l
/// https://kan-kikuchi.hatenablog.com/entry/playModeStateChanged
/// </summary>

//[InitializeOnLoad]//�G�f�B�^�[�N�����ɃR���X�g���N�^���Ă΂��悤��
public static class SceneTester
{

    /// <summary>
    /// �R���X�g���N�^(InitializeOnLoad�����ɂ��G�f�B�^�[�N�����ɌĂяo�����)
    /// </summary>
    static SceneTester()
    {
        //playModeStateChanged�C�x���g�Ƀ��\�b�h�o�^
       // EditorApplication.playModeStateChanged += OnTestPlayStarted;

    }

    /*
    //�v���C���[�h���ύX���ꂽ
    private static void OnTestPlayStarted(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Debug.LogWarning("TestMode�ɂ��N��:" + SceneManager.GetActiveScene().name);

                SceneVisor Visor = null;

                GameObject @object = GameObject.FindGameObjectWithTag("SceneVisor");
                if (@object != null)
                {
                    Visor = @object.GetComponent<SceneVisor>();
                }
                else
                {
                    Debug.LogError("�V�[���ɂ͕K�����SceneVisor��ݒu���邱�Ƃ��K�v");
                }

                Visor.TestFunction();



            }
        }


    }*/

}
