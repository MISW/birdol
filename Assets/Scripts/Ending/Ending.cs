using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    #region//public�ϐ�
    [Header("Character�̃v���n�u")] public GameObject character;
    public GameObject canvas;
    [Header("�e�L�����N�^�[�̃X�e�[�^�X")] public float[] characterStatus = new float[15];
    [Header("�L�����N�^�[�̃X�e�[�^�X�̍ő�l")]public float maxStatus; //�X�e�[�^�X����l
    [Header("����Sprite")]public Sprite[] star = new Sprite[6];
    [Header("���̐�")]public int maxStar; //���̐�
    public ProgressModel[] Characters = new ProgressModel[5];
    #endregion

    #region//private�ϐ�
    private int currentCharacterNumber = 0;
    private int currentCharacterVocal = 0;
    private int currentCharacterVisual = 0;
    private int currentCharacterDance = 0;
    private GameObject Star = null;
    private List<Image> VocalStarImage = new List<Image>();
    private List<Image> VisualStarImage = new List<Image>();
    private List<Image> DanceStarImage = new List<Image>();
    private List<GameObject> CharacterList = new List<GameObject>();
    #endregion

    void Start()
    {
        FindStar();
        SetCharacter();
        ChangeCurrentCharacterStars(0);
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
            CharacterList[currentCharacterNumber].GetComponent<Image>().enabled = false;
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
        CharacterList[i].GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// i�Ԗڂ̃L�����N�^�[�̃X�e�[�^�X�ɍ��킹�Đ��̐���ύX
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterStars(int i)
    {
        //�e�L�����N�^�[�̃X�e�[�^�X��ǂݍ��݁A�X�e�[�^�X�̍ő�l�𒴂��Ă����ꍇ�͂��̒l���X�e�[�^�X�̍ő�l�ɕύX
        currentCharacterVocal = (int)Mathf.Min(Characters[i].vocal,maxStatus);
        currentCharacterVisual = (int)Mathf.Min(Characters[i].visual,maxStatus);
        currentCharacterDance = (int)Mathf.Min(Characters[i].dance,maxStatus);

        SetSongStar(currentCharacterVocal);
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
            VocalStarImage[i].sprite = star[5];
        }
        if(status != maxStatus) VocalStarImage[status / 5].sprite = star[status % 5];
        for(int i=status / 5 + 1;i<maxStar;i++)
        {
            VocalStarImage[i].sprite = star[0];
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
            Star = GameObject.Find("VocalStar" + i);
            VocalStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("VisualStar" + i);
            VisualStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("DanceStar" + i);
            DanceStarImage.Add(Star.GetComponent<Image>());
        }
    }

    private void SetCharacter()
    {
        var parent = canvas.transform;
        for(int i=0;i<5;i++)
        {
            GameObject C = Instantiate(character, parent);
            CharacterList.Add(C);
            CharacterList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            CharacterList[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ending/" + Characters[i].characterId);
            CharacterList[i].GetComponent<Image>().enabled = false;
            if (i == 0) CharacterList[i].GetComponent<Image>().enabled = true;
        }
    }
}
