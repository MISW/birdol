using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    #region//public�ϐ�
    [Header("�e�L�����N�^�[�̃X�e�[�^�X")] public float[] characterStatus = new float[15];
    [Header("�L�����N�^�[�̃X�e�[�^�X�̍ő�l")]public float maxStatus; //�X�e�[�^�X����l
    [Header("����Sprite")]public Sprite[] star = new Sprite[6];
    [Header("���̐�")]public int maxStar; //���̐�
    #endregion

    #region//private�ϐ�
    private int currentCharacterNumber = 1;
    private int currentCharacterSong = 0;
    private int currentCharacterVisual = 0;
    private int currentCharacterDance = 0;
    private GameObject currentCharacter = null;
    private Image currentCharacterImage = null;
    private GameObject Star = null;
    private List<Image> SongStarImage = new List<Image>();
    private List<Image> VisualStarImage = new List<Image>();
    private List<Image> DanceStarImage = new List<Image>();
    #endregion

    void Start()
    {
        FindStar();
        ChangeCurrentCharacterImage(1);
        ChangeCurrentCharacterStars(1);
    }

    /// <summary>
    /// �\����ʂ�i�Ԗڂ̃L�����N�^�[�ɐ؂�ւ�
    /// </summary>
    /// <param name="i"></param>
    public void ChangeCurrentCharacter(int i)
    {
        //Debug.Log(i);
        if(currentCharacterNumber != i)
        {
            currentCharacterImage.enabled = false;
            ChangeCurrentCharacterImage(i);
            ChangeCurrentCharacterStars(i);
            currentCharacterNumber = i;
        }
    }

    /// <summary>
    /// i�Ԗڂ̃L�����N�^�[�ɗ����G��ύX
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterImage(int i)
    {
        currentCharacter = GameObject.Find("Character" + i);
        currentCharacterImage = currentCharacter.GetComponent<Image>();
        currentCharacterImage.enabled = true;
    }

    /// <summary>
    /// i�Ԗڂ̃L�����N�^�[�̃X�e�[�^�X�ɍ��킹�Đ��̐���ύX
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterStars(int i)
    {
        //�e�L�����N�^�[�̃X�e�[�^�X��ǂݍ��݁A�X�e�[�^�X�̍ő�l�𒴂��Ă����ꍇ�͂��̒l���X�e�[�^�X�̍ő�l�ɕύX
        currentCharacterSong = (int)Mathf.Min(characterStatus[(i - 1) * 3],maxStatus);
        currentCharacterVisual = (int)Mathf.Min(characterStatus[(i - 1) * 3 + 1],maxStatus);
        currentCharacterDance = (int)Mathf.Min(characterStatus[(i - 1) * 3 + 2],maxStatus);

        SetSongStar(currentCharacterSong);
        SetVisualStar(currentCharacterVisual);
        SetDanceStar(currentCharacterDance);

    }

    /// <summary>
    /// �eSongStar��Sprite��^����ꂽstatus�ɍ��킹�ĕύX
    /// </summary>
    /// <param name="status"></param>
    private void SetSongStar(int status)
    {
        for(int i=0;i<status/5;i++)
        {
            SongStarImage[i].sprite = star[5];
        }
        if(status != maxStatus) SongStarImage[status / 5].sprite = star[status % 5];
        for(int i=status / 5 + 1;i<maxStar;i++)
        {
            SongStarImage[i].sprite = star[0];
        }
    }

    /// <summary>
    /// �eVisualStar��Sprite��^����ꂽstatus�ɍ��킹�ĕύX
    /// </summary>
    /// <param name="status"></param>
    private void SetVisualStar(int status)
    {
        for (int i = 0; i < status / 5; i++)
        {
            VisualStarImage[i].sprite = star[5];
        }
        if (status != maxStatus) VisualStarImage[status / 5].sprite = star[status % 5];
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            VisualStarImage[i].sprite = star[0];
        }
    }

    /// <summary>
    /// �eDanceStar��Sprite��^����ꂽstatus�ɍ��킹�ĕύX
    /// </summary>
    /// <param name="status"></param>
    private void SetDanceStar(int status)
    {
        for (int i = 0; i < status / 5; i++)
        {
            DanceStarImage[i].sprite = star[5];
        }
        if (status != maxStatus) DanceStarImage[status / 5].sprite = star[status % 5];
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            DanceStarImage[i].sprite = star[0];
        }
    }

    /// <summary>
    /// �eStar��Image�R���|�[�l���g��߂܂���
    /// </summary>
    private void FindStar()
    {
        for(int i=1;i<=maxStar;i++)
        {
            Star = GameObject.Find("SongStar" + i);
            SongStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("VisualStar" + i);
            VisualStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("DanceStar" + i);
            DanceStarImage.Add(Star.GetComponent<Image>());
        }
    }
}
