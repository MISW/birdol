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
    public GameObject selectionDialog;
    public GameObject firstSelection;
    float showspeed = 0.05f;
    string curserifu;
    bool showingseifu = false;
    IEnumerator coroutine;
    Queue<GameObject> selectionqueue = new Queue<GameObject>();

    private Color normal = new Color(1f,1f,1f);
    private Color dark = new Color(0.5f, 0.5f, 0.5f);
    void Start()
    {
        if (Common.characters == null) Common.initCharacters();//Test Only
        datas = Resources.Load<TextAsset>("story/"+Common.mainstoryid).ToString().Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        size = datas.Length;
        UpdateDialog();
    }

    public void EndStory()
    {
        int sceneid = -1;
        Common.loadingCanvas.SetActive(true);
        Common.loadingGif.GetComponent<GifPlayer>().index = 0;
        Common.loadingGif.GetComponent<GifPlayer>().StartGif();
        Common.bgmplayer.Stop();
        Common.bgmplayer.time = 0;
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
            sceneid = (int)gamestate.Ending;
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
        webClient.SetData(Common.mainstoryid,Common.lessonCount);
        webClient.sceneid = sceneid;
        StartCoroutine(webClient.Send());
    }


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
            if (data.StartsWith("##"))
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
            else if (data.StartsWith("#"))
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
            else if (data.StartsWith("/‘I‘ðend"))
            {
                selectionDialog.SetActive(true);
            }
            else if (data.StartsWith("/")){
                if (data.StartsWith("/‘I‘ðstart"))
                {
                    int count = 0;
                    while (selectionqueue.Count != 0)
                    {
                        Destroy(selectionqueue.Dequeue());
                    }
                }
                else if (data.StartsWith("/sel"))
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
                    int ctlength = data.IndexOf(")") - data.IndexOf("(")-1;
                    string choiceText = data.Substring(data.IndexOf("(")+1,ctlength);
                    selectButton.GetComponentInChildren<Text>().text = choiceText;
                    selectButton.onClick.RemoveAllListeners();
                    selectButton.onClick.AddListener(delegate {
                        string command = data.Substring(data.IndexOf(">>"));
                        if (command != "all")
                        {

                        }
                        selectionDialog.SetActive(false);
                        UpdateDialog();
                    });
                    selcount++;
                }else if (data.StartsWith("/2‘Ìon"))
                {
                    characterImage.enabled = false;
                    leftImage.enabled = true;
                    rightImage.enabled = true;
                }
                else if (data.StartsWith("/2‘Ìoff"))
                {
                    characterImage.enabled = true;
                    leftImage.enabled = false;
                    rightImage.enabled = false;
                }
                UpdateDialog();
            }
            else if (data.Length > 0)
            {
                curserifu = data.Replace("[mom]",Common.mom).Replace("[player]",Common.PlayerName);
                coroutine = ShowSerifu();
                StartCoroutine(coroutine);
            }
            else
            {
                UpdateDialog();
            }
            
            /*
            else if (curevent.choiceName != null)
            {
                selectionName.text = curevent.choiceName;
                int count = 0;
                
                while (selectionqueue.Count != 0)
                {
                    Destroy(selectionqueue.Dequeue());
                }
                foreach (Choice choice in curevent.choices)
                {
                    Button selectButton;
                    RectTransform rt = selectionDialog.GetComponent<RectTransform>();
                    if (count==0)
                    {
                        selectButton = firstSelection.GetComponent<Button>();
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);
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
                        selectButton.GetComponent<RectTransform>().anchoredPosition = firstSelection.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, -1 * (buttonHeight + 15) * count);
                        selectButton.GetComponent<RectTransform>().sizeDelta = firstSelection.GetComponent<RectTransform>().sizeDelta;
                        newSelection.name = "otherselection";
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,rt.rect.height+ (buttonHeight+10));
                        selectionqueue.Enqueue(newSelection);
                    }
                    selectButton.GetComponentInChildren<Text>().text = choice.choiceName;
                    selectButton.onClick.RemoveAllListeners();
                    selectButton.onClick.AddListener(delegate {
                        currentEventId = 0;
                        currentStoryId = choice.targetStoryId;
                        Debug.Log("TargentSelection:" + choice.choiceName);
                        Debug.Log("TargentStoryId:"+choice.targetStoryId);
                        selectionDialog.SetActive(false);
                        UpdateDialog();
                    });
                    count++;
                }
                selectionDialog.SetActive(true);
            }*/
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
