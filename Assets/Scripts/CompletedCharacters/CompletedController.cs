using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletedController : MonoBehaviour
{
    
    public static List<DendouModel> CompletedCharacters = new List<DendouModel>();
    public GameObject Container;
    public GameObject HorizontalChild;
    public GameObject Dialog;

    public Text TeamName;
    public Text ActiveSkillLevel;
    public Text PassiveSkillLevel;
    public Image StandImage;
    public Sprite[] star = new Sprite[6];

    private List<Image> VocalStarImage = new List<Image>();
    private List<Image> VisualStarImage = new List<Image>();
    private List<Image> DanceStarImage = new List<Image>();

    int maxStar = 10;
    public float maxStatus = 100;

    public void OpenDialog(Button button)
    {
        int index = button.transform.GetSiblingIndex() + button.transform.parent.GetSiblingIndex() * 5;
        DendouModel character = CompletedCharacters[index];
        StandImage.sprite = Common.standImages[index];
        TeamName.text = character.Name;
        ActiveSkillLevel.text = "Lv." + character.ActiveSkillLevel;
        PassiveSkillLevel.text = "Lv." + character.PassiveSkillLevel;
        ChangeCurrentCharacterStars(index);
        Dialog.transform.SetSiblingIndex(114514);
    }

    public void CloseDialog()
    {
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
            VocalStarImage[i].sprite = star[5];
        }
        if (status != 50) VocalStarImage[status / 5].sprite = star[status % 5];
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            VocalStarImage[i].sprite = star[0];
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
            VisualStarImage[i].sprite = star[5];
        }
        if (status != 50) VisualStarImage[status / 5].sprite = star[status % 5];
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            VisualStarImage[i].sprite = star[0];
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
            DanceStarImage[i].sprite = star[5];
        }
        if (status != 50) DanceStarImage[status / 5].sprite = star[status % 5];
        for (int i = status / 5 + 1; i < maxStar; i++)
        {
            DanceStarImage[i].sprite = star[0];
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

   
    public void ReturnToHome()
    {
        Common.loadingCanvas.SetActive(true);
        CompletedCharacters.Clear();
        Manager.manager.StateQueue((int)gamestate.Home);
        
    }
    // Start is called be
    void Start()
    {
        if(Common.characters==null)Common.initCharacters();
        int index = 0;
        GameObject horizontal = HorizontalChild;
        for (int i = 0; i < 32; i++)
        {
            if(index == 0) horizontal = Instantiate(HorizontalChild, Container.transform);
            DendouModel dendouModel = new DendouModel();
            dendouModel.MainCharacterId = i;
            dendouModel.SupportCharacterId = i;
            dendouModel.Name = Common.characters[i].name;
            dendouModel.Vocal = Common.characters[i].vocal;
            dendouModel.Visual = Common.characters[i].visual;
            dendouModel.Dance = Common.characters[i].dance;
            CompletedCharacters.Add(dendouModel);
            horizontal.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/charactericon/" + dendouModel.MainCharacterId);
            if (index == 4)index = 0;
            else index++;
        }
        Destroy(HorizontalChild);
        FindStar();
    }

}
