using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Audio;

public class GameMenuManager : MonoBehaviour
{
    [Header("Panneaux (CanvasGroups)")]
    public CanvasGroup mainPanel;
    public CanvasGroup playPanel;
    public CanvasGroup optionsPanel;
    public CanvasGroup creditsPanel;

    [Header("UI Options")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle subtitlesToggle;

    [Header("Boutons de Mode de Jeu (Sprites)")]
    public Button btnModeStory;
    public Button btnModeTimeAttack;
    public Sprite spriteNormal;
    public Sprite spriteHover;
    public Sprite spriteSelected;

    [Header("Boutons de Temps (Endless)")]
    public Button btnTime3;
    public Button btnTime5;
    public Button btnTime10;
    private int selectedTime = 3;
    [Tooltip("Sprites spécifiques pour les boutons de temps")]
    public Sprite spriteTimeNormal;
    public Sprite spriteTimeHover;
    public Sprite spriteTimeSelected;

    [Header("Audio Settings")]
    public AudioMixer mainAudioMixer;
    public string musicParameterName = "VolumeMaster";
    public string sfxParameterName = "VolumeSFX";

    [Header("Livre Magique & Animations")]
    public Animator bookAnimator;
    public AudioManager audioManager; // Pour jouer le son de page
    public string animTurnRight = "PageTurnRight";
    public string animTurnLeft = "PageTurnLeft";

    [Header("Paramètres de Lancement")]
    public string storySceneName = "Scene_Histoire";
    public string timeAttackSceneName = "Scene_Chrono";
    
    // État interne
    private string selectedMode = "Story";
    private CanvasGroup currentPanel;

    private void Start()
    {
        LoadSettings();

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        subtitlesToggle.onValueChanged.AddListener(SetSubtitles);

        SetupButtonJuice();

        HideAllPanelsInstantly();
        UpdateModeVisuals();
        UpdateTimeVisuals();
        
        // On affiche le menu principal au démarrage SANS animation de page
        TransitionToPanel(mainPanel, false, true);
    }
    void Update() 
    {
        if (bookAnimator != null) Debug.Log("Vitesse actuelle : " + bookAnimator.speed);
    }
  
    private void LoadSettings()
    {
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float savedSfxVol = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        
        musicSlider.value = savedMusicVol;
        sfxSlider.value = savedSfxVol;
        
        subtitlesToggle.isOn = PlayerPrefs.GetInt("SubtitlesEnabled", 1) == 1; 
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        if (mainAudioMixer != null)
        {
            float clampedValue = Mathf.Max(0.0001f, value);
            mainAudioMixer.SetFloat(musicParameterName, Mathf.Log10(clampedValue) * 20f);
        }
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        if (mainAudioMixer != null)
        {
            float clampedValue = Mathf.Max(0.0001f, value);
            mainAudioMixer.SetFloat(sfxParameterName, Mathf.Log10(clampedValue) * 20f);
        }
    }

    public void SetSubtitles(bool isEnabled)
    {
        PlayerPrefs.SetInt("SubtitlesEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

   
    public void SelectModeStory() 
    { 
        selectedMode = "Story"; 
        UpdateModeVisuals();
    }
    public void SelectModeTimeAttack() 
    { 
        selectedMode = "TimeAttack"; 
        UpdateModeVisuals();
    }
    public void SelectTime3()  { SetEndlessTime(3); }
    public void SelectTime5()  { SetEndlessTime(5); }
    public void SelectTime10() { SetEndlessTime(10); }

    private void SetEndlessTime(int minutes)
    {
        selectedTime = minutes;
        PlayerPrefs.SetInt("EndlessDuration", selectedTime);
        PlayerPrefs.Save();
        UpdateTimeVisuals();
    }

    private void UpdateTimeVisuals()
    {
        if (btnTime3 != null)  
            btnTime3.image.sprite =  (selectedTime == 3)  ? spriteTimeSelected : spriteTimeNormal;
            
        if (btnTime5 != null)  
            btnTime5.image.sprite =  (selectedTime == 5)  ? spriteTimeSelected : spriteTimeNormal;
            
        if (btnTime10 != null) 
            btnTime10.image.sprite = (selectedTime == 10) ? spriteTimeSelected : spriteTimeNormal;
    }
    
    public void LaunchGame()
    {
        string sceneToLoad = (selectedMode == "Story") ? storySceneName : timeAttackSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

 
    
    // Si on retourne au menu principal, c'est un retour en arrière (isGoingBack = true)
    public void OpenMainPanel() { TransitionToPanel(mainPanel, true); }
    
    // Pour tous les autres menus, on avance (isGoingBack = false)
    public void OpenPlayPanel() { TransitionToPanel(playPanel, false); }
    public void OpenOptionsPanel() { TransitionToPanel(optionsPanel, false); }
    public void OpenCreditsPanel() { TransitionToPanel(creditsPanel, false); }

    private void TransitionToPanel(CanvasGroup newPanel, bool isGoingBack, bool skipAnimation = false)
    {
        if (currentPanel == newPanel) return;

        // 1. Déclenchement de l'animation du livre et du son
        if (!skipAnimation && bookAnimator != null)
        {
            bookAnimator.speed = 1f;
            // On joue l'animation correspondante depuis le début (temps 0f)
            string animToPlay = isGoingBack ? animTurnLeft : animTurnRight;
            bookAnimator.Play(animToPlay, -1, 0f); 
            Debug.Log("Lancement anim : " + animToPlay);

            if (AudioManager.audioInstance != null) 
            {
                AudioManager.audioInstance.PlayTheGoodSound(Random.Range(17,20));
            }
        }

        // 2. Disparition rapide du panneau actuel
        if (currentPanel != null)
        {
            CanvasGroup panelToHide = currentPanel;
            panelToHide.interactable = false;
            panelToHide.blocksRaycasts = false;
            
            // On le cache vite (0.2s) pour ne pas gêner la page qui tourne
            panelToHide.DOFade(0f, 0.2f).OnComplete(() => {
                panelToHide.gameObject.SetActive(false);
            });
        }

        // 3. Apparition du nouveau panneau avec un léger délai
        newPanel.gameObject.SetActive(true);
        newPanel.alpha = 0f;
        newPanel.transform.localScale = Vector3.one * 0.95f;
        
        // On ajoute un SetDelay(0.2f) : le texte attend que la page ait commencé à tourner
        // pour apparaître, ce qui donne l'illusion qu'il était imprimé sur la nouvelle page !
        newPanel.transform.DOScale(Vector3.one, 0.3f).SetDelay(0.2f).SetEase(Ease.OutBack);
        newPanel.DOFade(1f, 0.3f).SetDelay(0.4f).OnComplete(() => {
            newPanel.interactable = true;
            newPanel.blocksRaycasts = true;
        });

        currentPanel = newPanel;
    }

    private void HideAllPanelsInstantly()
    {
        CanvasGroup[] allPanels = { mainPanel, playPanel, optionsPanel, creditsPanel };
        foreach (var p in allPanels)
        {
            if (p != null)
            {
                p.alpha = 0f;
                p.interactable = false;
                p.blocksRaycasts = false;
                p.gameObject.SetActive(false);
            }
        }
    }

    private void SetupButtonJuice()
    {
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();
            
            // --- QUAND LE LASER ENTRE (HOVER) ---
            EventTrigger.Entry enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((data) => { 
                btn.transform.DOScale(1.08f, 0.2f).SetEase(Ease.OutBack); 

                // Si c'est un bouton de mode et qu'il N'EST PAS sélectionné, on met le sprite Hover
                if (btn == btnModeStory && selectedMode != "Story") 
                    btn.image.sprite = spriteHover;
                else if (btn == btnModeTimeAttack && selectedMode != "TimeAttack") 
                    btn.image.sprite = spriteHover;
                else if (btn == btnTime3 && selectedTime != 3) 
                    btn.image.sprite = spriteTimeHover;
                else if (btn == btnTime5 && selectedTime != 5) 
                    btn.image.sprite = spriteTimeHover;
                else if (btn == btnTime10 && selectedTime != 10) 
                    btn.image.sprite = spriteTimeHover;
            });
            trigger.triggers.Add(enter);

            // --- QUAND LE LASER SORT ---
            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { 
                btn.transform.DOScale(1f, 0.2f).SetEase(Ease.OutCubic); 

                // On remet le visuel à jour (ça remettra le sprite Normal ou Selected)
                if (btn == btnModeStory || btn == btnModeTimeAttack)
                    UpdateModeVisuals();
                if (btn == btnTime3 || btn == btnTime5 || btn == btnTime10) UpdateTimeVisuals();
            });
            trigger.triggers.Add(exit);
        }
    }

    private void UpdateModeVisuals()
    {
        if (btnModeStory != null) 
            btnModeStory.image.sprite = (selectedMode == "Story") ? spriteSelected : spriteNormal;
            
        if (btnModeTimeAttack != null) 
            btnModeTimeAttack.image.sprite = (selectedMode == "TimeAttack") ? spriteSelected : spriteNormal;
    }
}