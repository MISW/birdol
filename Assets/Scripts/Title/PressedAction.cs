using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressedAction : MonoBehaviour
{

    public void OnClick() {
        //������ς���
        if (Common.UserID == 0)
        {
            Common.loadingCanvas.SetActive(true);
            /*
             * �����Ƀ��[�U�[�̍쐬����API�Ăяo�����ق�����������
            
             * 
             */
            Manager.manager.StateQueue((int)gamestate.Gacha);
        }
        else
        {
            /*
             *�i�s���̃X�g�[���[������΃z�[���ɑJ�ڂ��A�Ȃ���΃K�`���ɑJ�ڂ���A�i����(������v2�̕ۑ��pAPI�̉��C���I����Ă���̂ق����悳����?) 
             */

        }
        
    }
}
