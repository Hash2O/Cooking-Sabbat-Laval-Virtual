using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private List<Canvas> inGameUICanvas;
    [SerializeField] private TextMeshProUGUI inGameUICanvasButtonText;

    [SerializeField] private PlayableDirector director;
    [SerializeField] private TextMeshProUGUI directorButtonText;

    private bool isInGameMenuActive;
    private bool isInGameUIActive;

    [HideInInspector]
    public bool isNarrativeStoryActive;


    private void Start()
    {
        isInGameMenuActive = false;
        isInGameUIActive = true;
        isNarrativeStoryActive = true;

        inGameUICanvasButtonText.text = "Show";
        directorButtonText.text = "Stop";

        ToggleInGameUI();
    }
    public void ToggleInGameUI()
    {
        if (isInGameUIActive == true) 
        {
            foreach (var item in inGameUICanvas)
            {
                 item.gameObject.SetActive(false);
            }
            isInGameUIActive = false;
            inGameUICanvasButtonText.text = "Show";
        }
        else
        {
            foreach (var item in inGameUICanvas)
            {
                item.gameObject.SetActive(true);
            }
            isInGameUIActive = true;
            inGameUICanvasButtonText.text = "Hide";
        }
    }

    public void ToggleNarrativeStory()
    {
        if (isNarrativeStoryActive == true)
        {
            director.gameObject.SetActive(false);
            isNarrativeStoryActive = false;
            directorButtonText.text = "Start";
        }
        else if (isNarrativeStoryActive == false)
        {
            director.gameObject.SetActive(true);
            isNarrativeStoryActive = true;
            directorButtonText.text = "Stop";
        }
    }

    public void ToggleInGameMenu()
    {
        if(isInGameMenuActive == false)
        {
            inGameMenu.gameObject.SetActive(true);
            isInGameMenuActive = true;
        }
        else if(isInGameMenuActive == true)
        {
            inGameMenu.gameObject.SetActive(false);
            isInGameMenuActive = false;
        }
    }

    public void LeaveKitchen()
    {
        if(GameManager.GMInstance != null) GameManager.GMInstance.QuitGame();
    }
}
