using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{

    Story[] stories;
    int currentStoryId = 0;
    int currentEventId = 0;
    public Text characterName;
    public Text serifu;
    public Text selectionName;
    public Image characterImage;
    public GameObject selectionDialog;
    public GameObject firstSelection;
    float showspeed = 0.05f;
    string curserifu;
    bool showingseifu = false;
    Queue<GameObject> selectionqueue = new Queue<GameObject>();
    void Start()
    {
        string json = Resources.Load<TextAsset>("story/1").ToString();
        StoryData result = JsonUtility.FromJson<StoryData>(json);
        if (Common.characters == null) Common.initCharacters();//Test Only
        stories = result.stories;
        UpdateDialog();
    }

    public void UpdateDialog()
    {
        if (!showingseifu&&!selectionDialog.active)
        {
            Event curevent = stories[currentStoryId].events[currentEventId];
            if (curevent.serifu != null)
            {
                curserifu = curevent.serifu;
                showingseifu = true;
                characterName.text = Common.characters[curevent.characterId].name;
                characterImage.sprite = Resources.Load<Sprite>("Images/standimage/"+curevent.characterId);
                StartCoroutine(ShowSerifu());
            }
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
            }
            else
            {
                //その他
            }

            if (currentEventId + 1 < stories[currentStoryId].events.Length)
            {
                currentEventId++;
            }
            else
            {
                //End;
            }
        }

    }

    private IEnumerator ShowSerifu()
    {
        int count = 0; 
        serifu.text = ""; 
        while (count < curserifu.Length)
        {
            serifu.text += curserifu[count];
            count++;
            yield return new WaitForSeconds(showspeed);
        }
        showingseifu = false;
    }


}
