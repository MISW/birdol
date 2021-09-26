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
    public GameObject selectionDialog;
    public GameObject firstSelection;
    float showspeed = 0.05f;
    string curserifu;
    bool showingseifu = false;
    IEnumerator coroutine;
    Queue<GameObject> selectionqueue = new Queue<GameObject>();
    void Start()
    {
        if (Common.characters == null) Common.initCharacters();//Test Only
        datas = Resources.Load<TextAsset>("story/"+Common.storyid).ToString().Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        size = datas.Length;
        UpdateDialog();
    }

    public void EndStory()
    {
        if (Common.storyid == "opening")
        {
            Common.storyid = "0";
            SceneManager.UnloadSceneAsync((int)gamestate.Story);
            Manager.manager.StateQueue((int)gamestate.Story);
        }else if (Common.storyid == "0")
        {
            Common.storyid = "1a";
            SceneManager.UnloadSceneAsync((int)gamestate.Story);
            Manager.manager.StateQueue((int)gamestate.Story);
        }
        else if (Common.storyid == "8c")
        {
            Common.storyid = "ending";
            SceneManager.UnloadSceneAsync((int)gamestate.Story);
            Manager.manager.StateQueue((int)gamestate.Story);
        }
        else if (Common.storyid == "ending")
        {
            Manager.manager.StateQueue((int)gamestate.Ending);
        }
        else if (Common.storyid.EndsWith("a"))
        {
            //To Lesson
            Common.storyid = Common.storyid.Replace("a","b");
            Manager.manager.StateQueue((int)gamestate.Lesson);
        }
        else if (Common.storyid.EndsWith("b"))
        {
            //To Live
            Common.storyid = Common.storyid.Replace("b", "c");
            Manager.manager.StateQueue((int)gamestate.Lesson);
        }
        else if (Common.storyid.EndsWith("c"))
        {
            //To Live
            Common.storyid = (Int32.Parse(Common.storyid.Replace("c", ""))+1)+"a";
            Manager.manager.StateQueue((int)gamestate.Lesson);
        }
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
            if (data.StartsWith("#"))
            {
                String name = data.Substring(1);
                String filename="";
                if (data.Contains("("))
                {
                    name = name.Substring(0, name.IndexOf("("));
                    filename = data.Substring(data.IndexOf("(")+1).Replace(")","");
                }
                characterImage.sprite = Resources.Load<Sprite>("Images/standimage/" + filename);
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
                }
                UpdateDialog();
            }
            else if (data.Length > 0)
            {
                curserifu = data;
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
