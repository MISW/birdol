using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompletedController : MonoBehaviour
{
    
    public static List<DendouModel> CompletedCharacters = new List<DendouModel>();
    public GameObject Container;
    public GameObject HorizontalChild;
    public GameObject Dialog;

    public Text TeamName;
    public Image StandImage;
    public Sprite[] star = new Sprite[5];

    private List<Image> VocalStarImage = new List<Image>();
    private List<Image> VisualStarImage = new List<Image>();
    private List<Image> DanceStarImage = new List<Image>();

    public Image activeSkillGaugeImage;
    public Image passiveSkillGaugeImage;

    [Header("アクティブスキルゲージのSprite")] public Sprite[] activeSkillGaugeSprites = new Sprite[6];
    [Header("パッシブスキルゲージのSprite")] public Sprite[] passiveSkillGaugeSprites = new Sprite[6];

    int maxStar = 10;
    public float maxStatus = 100;

    private void ChangeCurrentCharacterSKillGauge(int i)
    {
        
    }

    public void OpenDialog(Button button)
    {
        Common.subseplayer.PlayOneShot(Common.seclips["ok1"]);
        int index = button.transform.GetSiblingIndex() + button.transform.parent.GetSiblingIndex() * 5;
        DendouModel character = CompletedCharacters[index];
        StandImage.sprite = Common.standImages[character.MainCharacterId];
        TeamName.text = character.Name;
        activeSkillGaugeImage.sprite = activeSkillGaugeSprites[character.ActiveSkillLevel];
        passiveSkillGaugeImage.sprite = passiveSkillGaugeSprites[character.PassiveSkillLevel];
        ChangeCurrentCharacterStars(index);
        Dialog.transform.SetSiblingIndex(114514);
    }

    public void CloseDialog()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["cancel2"]);
        StandImage.GetComponent<Image>().sprite = null;
        TeamName.text = "";
        Dialog.transform.SetSiblingIndex(0);
    }

    private void ChangeCurrentCharacterStars(int i) {
        int currentCharacterVocal = (int)Mathf.Min(CompletedCharacters[i].Vocal, maxStatus);
        int currentCharacterVisual = (int)Mathf.Min(CompletedCharacters[i].Visual, maxStatus);
        int currentCharacterDance = (int)Mathf.Min(CompletedCharacters[i].Dance, maxStatus);

        currentCharacterVocal = (int) (((float) currentCharacterVocal / (float) maxStatus) * 50.0f);
        currentCharacterVisual = (int) (((float) currentCharacterVisual / (float) maxStatus) * 50.0f);
        currentCharacterDance = (int) (((float) currentCharacterDance / (float) maxStatus) * 50.0f);

        SetSongStar(currentCharacterVocal);
        SetVisualStar(currentCharacterVisual);
        SetDanceStar(currentCharacterDance);
    }

        

    

    /// <summary>
    /// 各SongStarのSpriteを与えられたstatusに合わせて変更
    /// </summary>
    /// <param name="status"></param>
    private void SetSongStar(int status)
    {
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
    /// 各VisualStarのSpriteを与えられたstatusに合わせて変更
    /// </summary>
    /// <param name="status"></param>
    private void SetVisualStar(int status)
    {
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
    /// 各DanceStarのSpriteを与えられたstatusに合わせて変更
    /// </summary>
    /// <param name="status"></param>
    private void SetDanceStar(int status)
    {
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
    /// 各StarのImageコンポーネントを捕まえる
    /// </summary>
    private void FindStar()
    {
        for (int i = 1; i <= maxStar; i++)
        {
            GameObject Star = GameObject.Find("VocalStar" + i);
            VocalStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("VisualStar" + i);
            VisualStarImage.Add(Star.GetComponent<Image>());
            Star = GameObject.Find("DanceStar" + i);
            DanceStarImage.Add(Star.GetComponent<Image>());
        }
    }

    private IEnumerator returnToHome()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Manager.manager.StateQueue((int)gamestate.Home);
    }


    public void ReturnToHome()
    {
        Common.subseplayer.PlayOneShot(Common.seclips["cancel2"]);
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        CompletedCharacters.Clear();
        StartCoroutine(returnToHome());
        
    }
    // Start is called be
    void Start()
    {
        if(Common.characters==null)Common.initCharacters();
        int index = 0;
        GameObject horizontal = HorizontalChild;
        for (int i = 0; i < CompletedCharacters.Count; i++)
        {
            if(index == 0) horizontal = Instantiate(HorizontalChild, Container.transform);
            DendouModel dendouModel = CompletedCharacters[i];
            horizontal.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + dendouModel.MainCharacterId);
            if (index == 4)index = 0;
            else index++;
        }
        Destroy(HorizontalChild);
        FindStar();
    }

    bool triggerdPlayer = false;
    private void Update()
    {
        if (!triggerdPlayer && SceneManager.GetActiveScene().name == "CompletedCharacters")
        {
            Common.bgmplayer.clip = (AudioClip)Resources.Load("Music/BG03");
            Common.bgmplayer.Play();
            triggerdPlayer = true;
        }
    }

}
