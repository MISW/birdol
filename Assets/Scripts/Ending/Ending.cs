using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    #region//public�ϐ�
    [Header("Character�̃v���n�u")] public GameObject character;
    public GameObject canvas;
    [Header("�e�L�����N�^�[�̃X�e�[�^�X")] public float[] characterStatus = new float[15];
    [Header("�L�����N�^�[�̃X�e�[�^�X�̍ő�l")] public float maxStatus; //�X�e�[�^�X����l
    [Header("����Sprite")] public Sprite[] star = new Sprite[5];
    [Header("�A�N�e�B�u�X�L���Q�[�W��Sprite")] public Sprite[] activeSkillGaugeSprites = new Sprite[6];
    [Header("�p�b�V�u�X�L���Q�[�W��Sprite")] public Sprite[] passiveSkillGaugeSprites = new Sprite[6];
    [Header("���̐�")] public int maxStar; //���̐�
    public ProgressModel[] Characters = new ProgressModel[5];
    #endregion

    #region//private�ϐ�
    private int currentCharacterNumber = 0;
    private int currentCharacterVocal = 0;
    private int currentCharacterVisual = 0;
    private int currentCharacterDance = 0;
    private GameObject Star = null;
    private Image activeSkillGaugeImage = null;
    private Image passiveSkillGaugeImage = null;
    private List<Image> VocalStarImage = new List<Image>();
    private List<Image> VisualStarImage = new List<Image>();
    private List<Image> DanceStarImage = new List<Image>();
    private List<GameObject> CharacterList = new List<GameObject>();
    private List<GameObject> CharacterButtonList = new List<GameObject>();
    #endregion

    void Start()
    {
        FindStar();
        FindButton();
        FindGauge();
        CharacterButtonList[0].transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        CharacterButtonList[0].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SetCharacter();
        ChangeCurrentCharacterStars(0);
        ChangeCurrentCharacterSKillGauge(0);
    }

    /// <summary>
    /// �\����ʂ�i�Ԗڂ̃L�����N�^�[�ɐ؂�ւ�
    /// </summary>
    /// <param name="i"></param>
    public void ChangeCurrentCharacter(int i)
    {
#if UNITY_EDITOR
        //Debug.Log(i);
#endif
        if (currentCharacterNumber != i)
        {
            CharacterList[currentCharacterNumber].GetComponent<Image>().enabled = false;
            CharacterButtonList[currentCharacterNumber].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            CharacterButtonList[currentCharacterNumber].GetComponent<Image>().color = new Color(110.0f / 255.0f, 110.0f / 255.0f, 110.0f / 255.0f, 1.0f);
            CharacterButtonList[i].transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            CharacterButtonList[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            ChangeCurrentCharacterImage(i);
            ChangeCurrentCharacterStars(i);
            ChangeCurrentCharacterSKillGauge(i);
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
        currentCharacterVocal = (int)Mathf.Min(Common.progresses[i].Vocal, maxStatus);
        currentCharacterVisual = (int)Mathf.Min(Common.progresses[i].Visual, maxStatus);
        currentCharacterDance = (int)Mathf.Min(Common.progresses[i].Dance, maxStatus);

        currentCharacterVocal = (int)(((float)currentCharacterVocal / (float)maxStatus) * 50.0f);
        currentCharacterVisual = (int)(((float)currentCharacterVisual / (float)maxStatus) * 50.0f);
        currentCharacterDance = (int)(((float)currentCharacterDance / (float)maxStatus) * 50.0f);

        SetSongStar(currentCharacterVocal);
        SetVisualStar(currentCharacterVisual);
        SetDanceStar(currentCharacterDance);

    }

    /// <summary>
    /// i�Ԗڂ̃L�����N�^�[�̃X�e�[�^�X�ɍ��킹�ăX�L���Q�[�W��ύX
    /// </summary>
    /// <param name="i"></param>
    private void ChangeCurrentCharacterSKillGauge(int i)
    {
        activeSkillGaugeImage.sprite = activeSkillGaugeSprites[Common.progresses[i].ActiveSkillLevel];
        passiveSkillGaugeImage.sprite = passiveSkillGaugeSprites[Common.progresses[i].PassiveSkillLevel];
    }

    /// <summary>
    /// �eSongStar��Sprite��^����ꂽstatus�ɍ��킹�ĕύX
    /// </summary>
    /// <param name="status"></param>
    private void SetSongStar(int status)
    {
#if UNITY_EDITOR
        //Debug.Log("Vocal" + status);
#endif
        for (int i = 0; i < status / 5; i++)
        {
            VocalStarImage[i].enabled = true;
            VocalStarImage[i].sprite = star[4];
        }
        if (status != 50)
        {
            if (status % 5 == 0)
            {
                VocalStarImage[status / 5].enabled = false;
            }
            else
            {
                VocalStarImage[status / 5].enabled = true;
                VocalStarImage[status / 5].sprite = star[(status % 5) - 1];
            }
        }
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            VocalStarImage[i].enabled = false;
        }
    }

    /// <summary>
    /// �eVisualStar��Sprite��^����ꂽstatus�ɍ��킹�ĕύX
    /// </summary>
    /// <param name="status"></param>
    private void SetVisualStar(int status)
    {
#if UNITY_EDITOR
        //Debug.Log("Visual" + status);
#endif
        for (int i = 0; i < status / 5; i++)
        {
            VisualStarImage[i].enabled = true;
            VisualStarImage[i].sprite = star[4];
        }
        if (status != 50)
        {
            if (status % 5 == 0)
            {
                VisualStarImage[status / 5].enabled = false;
            }
            else
            {
                VisualStarImage[status / 5].enabled = true;
                VisualStarImage[status / 5].sprite = star[(status % 5) - 1];
            }
        }
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            VisualStarImage[i].enabled = false;
        }
    }

    /// <summary>
    /// �eDanceStar��Sprite��^����ꂽstatus�ɍ��킹�ĕύX
    /// </summary>
    /// <param name="status"></param>
    private void SetDanceStar(int status)
    {
#if UNITY_EDITOR
        //Debug.Log("Dance" + status);
#endif
        for (int i = 0; i < status / 5; i++)
        {
            DanceStarImage[i].enabled = true;
            DanceStarImage[i].sprite = star[4];
        }
        if (status != 50)
        {
            if (status % 5 == 0)
            {
                DanceStarImage[status / 5].enabled = false;
            }
            else
            {
                DanceStarImage[status / 5].enabled = true;
                DanceStarImage[status / 5].sprite = star[(status % 5) - 1];
            }
        }
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            DanceStarImage[i].enabled = false;
        }
    }

    /// <summary>
    /// �eStar��Image�R���|�[�l���g��߂܂���
    /// </summary>
    private void FindStar()
    {
        for (int i = 1; i <= maxStar; i++)
        {
            Star = GameObject.Find("VocalStar" + i);
            VocalStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("VisualStar" + i);
            VisualStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("DanceStar" + i);
            DanceStarImage.Add(Star.GetComponent<Image>());
        }
    }

    /// <summary>
    /// �eButton��߂܂���
    /// </summary>
    private void FindButton()
    {
        for (int i = 1; i <= 5; i++)
        {
            CharacterButtonList.Add(GameObject.Find("CharacterButton" + i));
        }
    }

    /// <summary>
    /// �eGauge��߂܂���
    /// </summary>
    private void FindGauge()
    {
        activeSkillGaugeImage = GameObject.Find("ActiveSkillGauge").GetComponent<Image>();
        passiveSkillGaugeImage = GameObject.Find("PassiveSkillGauge").GetComponent<Image>();
    }

    /// <summary>
    /// ���ꂼ��̃L�����N�^�[�摜��MainCharacterID����擾
    /// </summary>
    private void SetCharacter()
    {
        var parent = canvas.transform;
        for (int i = 0; i < 5; i++)
        {
            GameObject C = Instantiate(character, parent);
            CharacterList.Add(C);
            CharacterList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            CharacterList[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/standimage/" + Common.progresses[i].MainCharacterId);
            CharacterButtonList[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Live/Gif/" + Common.progresses[i].MainCharacterId + "/Ch-0");
            CharacterList[i].GetComponent<Image>().enabled = false;
            if (i == 0) CharacterList[i].GetComponent<Image>().enabled = true;
        }
    }

    public void ResetStory()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        Common.mainstoryid = null;
        ProgressService.FetchStory();
        ProgressService.FetchCompletedProgressAndUpdateGameStatus("home");
    }

    bool triggerdPlayer = false;
    void Update()
    {
        if (!triggerdPlayer && SceneManager.GetActiveScene().name == "Ending")
        {
            Common.bgmplayer.clip = (AudioClip)Resources.Load("Music/BG09");
            Common.bgmplayer.Play();
            triggerdPlayer = true;
        }
    }
}
