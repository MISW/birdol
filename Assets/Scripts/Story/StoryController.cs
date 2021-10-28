using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{

    String[] datas;
    int currentline = 0;
    int selcount = 0;
    int size;
    public bool issubstory;
    public string storyid;
    public Text characterName;
    public Text serifu;
    public Text selectionName;
    public Image characterImage;
    public Image leftImage;
    public Image rightImage;
    public Image background;
    public GameObject selectionDialog;
    public GameObject firstSelection;
    float showspeed = 0.05f;
    string curserifu;
    bool showingseifu = false;
    IEnumerator coroutine;
    Queue<GameObject> selectionqueue = new Queue<GameObject>();

    private Color normal = new Color(1f,1f,1f);
    private Color dark = new Color(0.5f, 0.5f, 0.5f);

    public static bool isSubStory = false;
    void Start()
    {
        if (Common.characters == null) Common.initCharacters();//Test Only
        if (isSubStory)
        {
            int id = RandomArray.GetRandom(Common.remainingSubstory);
            Common.TriggeredSubStory += (id + ",");
            Common.remainingSubstory.Remove(id);
            datas = Resources.Load<TextAsset>("story/sub/" + id).ToString().Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
            );
        }
        else
        {
            datas = Resources.Load<TextAsset>("story/" + Common.mainstoryid).ToString().Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
            );
        }
        size = datas.Length;
        UpdateDialog();
    }

    private IEnumerator UpdateSub()
    {
        UpdateCharacterWebClient characterWebClient = new UpdateCharacterWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/gamedata/character");
        characterWebClient.isReturnLesson = true;
        characterWebClient.SetData();
        yield return characterWebClient.Send();
        if(Common.lessonCount > 0)
        {
            Manager.manager.StateQueue((int)gamestate.Lesson);
        }
        else
        {
            UpdateMainStoryWebClient storyWebClient = new UpdateMainStoryWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/gamedata/story");
            Common.mainstoryid = Common.mainstoryid.Replace("a", "b");
            storyWebClient.SetData(Common.mainstoryid, 5);
            StoryController.isSubStory = false;
            storyWebClient.sceneid = (int)gamestate.Story;
            yield return storyWebClient.Send();
        }
    }
   

    public void EndStory()
    {
        int sceneid = -1;
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
        if (isSubStory)
        {
            isSubStory = false;
            StartCoroutine(UpdateSub());
        }
        else
        {
            isSubStory = false;
            if (Common.mainstoryid == "opening")
            {
                Common.mainstoryid = "0";
                sceneid = (int)gamestate.Story;
            }
            else if (Common.mainstoryid == "0")
            {
                Common.mainstoryid = "1a";
                sceneid = (int)gamestate.Story;
            }
            else if (Common.mainstoryid == "8c")
            {
                Common.mainstoryid = "ending";
                sceneid = (int)gamestate.Story;
            }
            else if (Common.mainstoryid == "ending")
            {
                FinishProgressWebClient finishiClient = new FinishProgressWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/gamedata/complete");
                finishiClient.sceneid = (int)gamestate.Ending;
                finishiClient.SetData();
                StartCoroutine(finishiClient.Send());
                return;
            }
            else if (Common.mainstoryid.EndsWith("a"))
            {
                //To Lesson
                sceneid = (int)gamestate.Lesson;
            }
            else if (Common.mainstoryid.EndsWith("b"))
            {
                //To Live
                sceneid = (int)gamestate.Live;
            }
            else if (Common.mainstoryid.EndsWith("c"))
            {
                //To Live
                Common.mainstoryid = (Int32.Parse(Common.mainstoryid.Replace("c", "")) + 1) + "a";
                sceneid = (int)gamestate.Story;
            }
            Debug.Log(Common.progressId);
            UpdateMainStoryWebClient webClient = new UpdateMainStoryWebClient(WebClient.HttpRequestMethod.Put, $"/api/{Common.api_version}/gamedata/story");
            Common.lessonCount = 5;
            webClient.SetData(Common.mainstoryid, Common.lessonCount);
            webClient.sceneid = sceneid;
            StartCoroutine(webClient.Send());
        }
        
    }

    bool allowShowing = true;
    string condId;
    public void UpdateDialog()
    {
        if (!showingseifu&&!selectionDialog.active)
        {
            if (currentline >= size)
            {
                EndStory();
                return;
            }
            Debug.Log("Current:" + currentline);
            String data = datas[currentline];
            Debug.Log("Data:" + data);
            currentline++;
           if (data.StartsWith("##") && allowShowing)
            {
                String name = data.Substring(2);
                String filename = "";
                String dir = "l";
                if (data.Contains("("))
                {
                    name = name.Substring(0, name.IndexOf("("));
                    String[] arr = data.Substring(data.IndexOf("(") + 1).Replace(")", "").Split(',');
                    filename = arr[0];
                    dir = arr[1];
                }
                if(dir=="l") leftImage.sprite = Resources.Load<Sprite>("Images/standimage/" + filename);
                else rightImage.sprite = Resources.Load<Sprite>("Images/standimage/" + filename);
                characterName.text = name;
                UpdateDialog();
            }
            else if (data.StartsWith("#") && allowShowing)
            {
                String name = data.Substring(1);
                String filename = "";
                if (data.Contains("("))
                {
                    name = name.Substring(0, name.IndexOf("("));
                    filename = data.Substring(data.IndexOf("(") + 1).Replace(")", "");
                    if (characterImage.enabled)
                    {
                        characterImage.sprite = Resources.Load<Sprite>("Images/standimage/" + filename);
                    }
                    else
                    {
                        if (filename == "l")
                        {
                            leftImage.color = normal;
                            rightImage.color = dark;
                        }
                        else
                        {
                            leftImage.color = dark;
                            rightImage.color = normal;
                        }
                    }
                    
                    
                }
                else
                {
                    characterImage.sprite = null;
                }
                characterName.text = name;
                UpdateDialog();
            }
            else if (data.StartsWith("/�I��end"))
            {
                selectionDialog.SetActive(true);
            }
            else if (data.StartsWith("/")){
                if (data.StartsWith("///"))
                {
                    if (data.EndsWith("start"))
                    {
                        string choiceText = data.Replace("///", "").Replace("start", "");
                        Debug.Log("CurrentChoice:" + choiceText);
                        allowShowing = (choiceText == condId);
                    }
                    else
                    {
                        allowShowing = true;
                    }
                }
                else if (data.StartsWith("/�I��start") && allowShowing)
                {
                    int count = 0;
                    while (selectionqueue.Count != 0)
                    {
                        Destroy(selectionqueue.Dequeue());
                    }
                }
                else if (data.StartsWith("/sel") && allowShowing)
                {
                    Button selectButton;
                    RectTransform rt = selectionDialog.GetComponent<RectTransform>();
                    if (selcount == 0)
                    {
                        selectButton = firstSelection.GetComponent<Button>();
                    }
                    else
                    {
                        GameObject newSelection = GameObject.Instantiate(firstSelection);
                        selectButton = newSelection.GetComponent<Button>();
                        selectButton.GetComponent<RectTransform>().SetParent(selectionDialog.transform);
                        selectButton.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        selectButton.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                        selectButton.GetComponent<RectTransform>().localScale = Vector3.one;
                        selectButton.GetComponent<RectTransform>().pivot = firstSelection.GetComponent<RectTransform>().pivot;
                        selectButton.GetComponent<RectTransform>().anchorMin = firstSelection.GetComponent<RectTransform>().anchorMin;
                        selectButton.GetComponent<RectTransform>().anchorMax = firstSelection.GetComponent<RectTransform>().anchorMax;
                        float buttonHeight = firstSelection.GetComponent<RectTransform>().rect.height;
                        selectButton.GetComponent<RectTransform>().anchoredPosition = firstSelection.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, -1 * (buttonHeight + 15) * selcount);
                        selectButton.GetComponent<RectTransform>().sizeDelta = firstSelection.GetComponent<RectTransform>().sizeDelta;
                        newSelection.name = "otherselection";
                        selectionqueue.Enqueue(newSelection);
                    }
                    int ctlength = data.IndexOf(")") - data.IndexOf("(") - 1;
                    string choiceText = data.Substring(data.IndexOf("(") + 1, ctlength);
                    selectButton.GetComponentInChildren<Text>().text = choiceText;
                    selectButton.onClick.RemoveAllListeners();
                    selectButton.onClick.AddListener(delegate
                    {
                        string command = data.Substring(data.IndexOf(">>") + 2);
                        if (command != "all")
                        {
                            condId = command;
                            Debug.Log("CurrentChoice:" + condId);
                        }
                        selectionDialog.SetActive(false);
                        UpdateDialog();
                    });
                    selcount++;
                }
                else if (data.StartsWith("/2��on") && allowShowing)
                {
                    characterImage.enabled = false;
                    leftImage.enabled = true;
                    rightImage.enabled = true;
                }
                else if (data.StartsWith("/2��off") && allowShowing)
                {
                    characterImage.enabled = true;
                    leftImage.enabled = false;
                    rightImage.enabled = false;
                }
                else if (data.StartsWith("/�w�i") && allowShowing)
                {
                    int ctlength = data.IndexOf(")") - data.IndexOf("(") - 1;
                    string filename = data.Substring(data.IndexOf("(") + 1, ctlength);
                    background.sprite = Resources.Load<Sprite>("Images/UI_Background/" + filename);

                }
                else if (data.StartsWith("/BGM�ꎞ��~") && allowShowing)
                {
                    Common.bgmplayer.Pause();
                }
                else if (data.StartsWith("/BGM�ĊJ") && allowShowing)
                {
                    Common.bgmplayer.Play();
                }
                else if (data.StartsWith("/BGM") && allowShowing)
                {
                    int ctlength = data.IndexOf(")") - data.IndexOf("(") - 1;
                    string bgm = data.Substring(data.IndexOf("(") + 1, ctlength);
                    Debug.Log("BGM:" + bgm);
                    Common.bgmplayer.Stop();
                    Common.bgmplayer.time = 0;
                    Common.bgmplayer.clip = (AudioClip)Resources.Load("Music/" + bgm);
                    Common.bgmplayer.Play();
                }
                else if (data.StartsWith("/active") && allowShowing)
                {
                    foreach (ProgressModel progress in Common.progresses)
                    {
                        progress.ActiveSkillLevel++;
                    }
                }
                else if (data.StartsWith("/passive") && allowShowing)
                {
                    foreach (ProgressModel progress in Common.progresses)
                    {
                        progress.PassiveSkillLevel++;
                    }
                }
                UpdateDialog();
            }
            else if (data.Length > 0 && allowShowing)
            {
                curserifu = data.Replace("[mom]",Common.mom).Replace("[player]",Common.PlayerName);
                coroutine = ShowSerifu();
                StartCoroutine(coroutine);
            }
            else
            {
                UpdateDialog();
            }
        }
        else if (showingseifu)
        {
            StopCoroutine(coroutine);
            serifu.text = curserifu;
            showingseifu = false;
        }

    }

    private IEnumerator ShowSerifu()
    {
        int count = 0; 
        serifu.text = "";
        showingseifu = true;
        while (count < curserifu.Length)
        {
            serifu.text += curserifu[count];
            count++;
            yield return new WaitForSeconds(showspeed);
        }
        showingseifu = false;
    }


}
