//using UnityEngine;

////Responsabilités:
////    gérer le jour courant(int currentDay 0–6)
////    gérer l’état Day/Night
////    déterminer le nombre de fantômes requis
////    activer/désactiver le GhostCycleManager
////    notifier la fin de nuit
////    déclencher l’apparition des clés

//public enum TimeOfDay
//{
//    Day,
//    Night
//}

//public class GameCycleManager : MonoBehaviour
//{
//    public static GameCycleManager Instance;

//    [Header("Semaine")]
//    public int currentDay = 0; // 0 = Lundi
//    public int maxDays = 7;

//    [Header("Etat actuel")]
//    public TimeOfDay currentTimeOfDay = TimeOfDay.Day;

//    [Header("Référence")]
//    public GhostCycleManager ghostCycleManager;
//    public Light directionalLight;

//    [Header("Objectifs")]
//    public int baseGhostsFirstNight = 5;

//    private int ghostsRequiredThisNight;
//    public BellInteraction bell;
//    public BellManager bellManager;

//    private void Awake()
//    {
//        Instance = this;
//    }

//    private void Start()
//    {
//        StartDay();
//    }

//    public void StartDay()
//    {
//        currentTimeOfDay = TimeOfDay.Day;

//        ghostCycleManager.enabled = false;

//        directionalLight.intensity = 1.0f;

//        bell.SetEnabled(true); // Clochette activée

//        bellManager.isBellActivated = false;

//        Debug.Log($"☀️ Jour {currentDay + 1}");
//    }

//    public void StartNight()
//    {
//        if (currentTimeOfDay == TimeOfDay.Night)
//            return;

//        currentTimeOfDay = TimeOfDay.Night;

//        bell.SetEnabled(false); // Clochette désactivée

//        ghostsRequiredThisNight = baseGhostsFirstNight + currentDay;

//        ghostCycleManager.enabled = true;

//        directionalLight.intensity = 0.3f;

//        PumpkinCounter.Instance.SetNightObjective(ghostsRequiredThisNight);

//        Debug.Log($"🌙 Nuit {currentDay + 1}");
//    }

//    public void EndNight()
//    {
//        ghostCycleManager.StopCycle();

//        SpawnKeyForCurrentDay();

//        currentDay++;

//        StartDay(); // La clochette sera réactivée ici
//    }

//    private void SpawnKeyForCurrentDay()
//    {
//        KeyManager.Instance.SpawnKey(currentDay);
//    }
//}

//using UnityEngine;
//using UnityEngine.SceneManagement;

//public enum TimeOfDay
//{
//    Day,
//    Night
//}

//public class GameCycleManager : MonoBehaviour
//{
//    public static GameCycleManager GameCycleInstance { get; private set; }

//    [Header("Semaine")]
//    public int currentDay = 0;
//    public int maxDays = 7;

//    [Header("Etat actuel")]
//    public TimeOfDay currentTimeOfDay = TimeOfDay.Day;

//    [Header("Références")]
//    public GhostCycleManager ghostCycleManager;
//    public GameObject directionalLight;
//    public BellInteraction bell;
//    public BellManager bellManager;

//    [Header("Objectifs")]
//    public int baseGhostsFirstNight = 5;

//    [Header("Intensité Jour et Nuit")]
//    public float dayIntensity = 1.0f;
//    public float nightIntensity = 0.3f;

//    private int ghostsRequiredThisNight;

//    private bool hasInitialized = false;

//    void Awake()
//    {
//        // Singleton robuste
//        if (GameCycleInstance != null && GameCycleInstance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        GameCycleInstance = this;
//        DontDestroyOnLoad(gameObject);

//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    void OnDestroy()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }

//    void Start()
//    {
//        if (!hasInitialized)
//        {
//            StartDay();
//            hasInitialized = true;
//        }
//    }

//    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        // Reconnecter les références de la scène
//        ghostCycleManager = FindFirstObjectByType<GhostCycleManager>();
//        directionalLight = GameObject.FindWithTag("Sun");             
//        bell = FindFirstObjectByType<BellInteraction>();
//        bellManager = FindFirstObjectByType<BellManager>();

//        // Réappliquer l'état du cycle
//        if (currentTimeOfDay == TimeOfDay.Day)
//        {
//            ApplyDayState();
//        }
//        else
//        {
//            ApplyNightState();
//        }
//    }

//    public void StartDay()
//    {
//        currentTimeOfDay = TimeOfDay.Day;

//        ApplyDayState();

//        Debug.Log($"☀️ Jour {currentDay + 1}");
//    }

//    public void StartNight()
//    {
//        if (currentTimeOfDay == TimeOfDay.Night)
//            return;

//        currentTimeOfDay = TimeOfDay.Night;

//        ghostsRequiredThisNight = baseGhostsFirstNight + currentDay;

//        ApplyNightState();

//        PumpkinCounter.PumpkinInstance.SetNightObjective(ghostsRequiredThisNight);

//        Debug.Log($"🌙 Nuit {currentDay + 1}");

//        if (ghostCycleManager == null)
//        {
//            ghostCycleManager = FindFirstObjectByType<GhostCycleManager>();
//        }

//        if (ghostCycleManager != null)
//        {
//            ghostCycleManager.StartCycle();
//            Debug.Log("StartCycle() : " + ghostCycleManager.cycleActive);
//        }
//        else
//        {
//            Debug.LogWarning("GhostCycleManager introuvable !");
//        }

//        //Debug.Log("StartCycle() : " + ghostCycleManager.cycleActive);
//    }

//    public void EndNight()
//    {
//        if (ghostCycleManager != null)
//            ghostCycleManager.StopCycle();

//        bellManager.isBellActivated = false;

//        SpawnKeyForCurrentDay();

//        currentDay++;

//        StartDay();
//    }

//    private void SpawnKeyForCurrentDay()
//    {
//        if (KeyManager.KeyInstance != null)
//        {
//            KeyManager.KeyInstance.SpawnKey(currentDay);
//        }
//    }

//    private void ApplyDayState()
//    {
//        if (ghostCycleManager != null)
//            ghostCycleManager.enabled = false;

//        if (directionalLight != null)
//            directionalLight.GetComponent<Light>().intensity = dayIntensity;

//        if (bell != null)
//            bell.SetEnabled(true);

//        if (bellManager != null)
//            bellManager.isBellActivated = false;
//    }

//    private void ApplyNightState()
//    {
//        if (ghostCycleManager != null)
//            ghostCycleManager.enabled = true;

//        if (directionalLight != null)
//            directionalLight.GetComponent<Light>().intensity = nightIntensity;

//        if (bell != null)
//            bell.SetEnabled(false);
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering; // Indispensable pour utiliser "Volume"
using System.Collections;

public enum TimeOfDay
{
    Day,
    Night
}

public class GameCycleManager : MonoBehaviour
{
    public static GameCycleManager GameCycleInstance { get; private set; }

    [Header("Semaine")]
    public int currentDay = 0;
    public int maxDays = 7;

    [Header("Etat actuel")]
    public TimeOfDay currentTimeOfDay = TimeOfDay.Day;

    [Header("Références")]
    public GhostCycleManager ghostCycleManager;
    public BellInteraction bell;
    public BellManager bellManager;
    public CalendarManager calendarManager;

    [Header("Post-Processing Volumes")]
    public Volume dayVolume;
    public Volume nightVolume;

    [Header("Objectifs & Transitions")]
    public int baseGhostsFirstNight = 5;
    public float transitionDuration = 3f;

    private int ghostsRequiredThisNight;
    private bool hasInitialized = false;
    private Coroutine transitionCoroutine;

    void Awake()
    {
        if (GameCycleInstance != null && GameCycleInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        GameCycleInstance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (!hasInitialized)
        {
            // Lancement instantané pour le tout premier jour
            StartDay(true); 
            hasInitialized = true;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reconnexion des managers de la scène
        ghostCycleManager = FindFirstObjectByType<GhostCycleManager>();
        bell = FindFirstObjectByType<BellInteraction>();
        bellManager = FindFirstObjectByType<BellManager>();
        calendarManager = FindFirstObjectByType<CalendarManager>();

        // // Recherche dynamique des Volumes de Post-Processing via les Tags
        // GameObject dayVolObj = GameObject.FindWithTag("DayVolume");
        // if (dayVolObj != null) dayVolume = dayVolObj.GetComponent<Volume>();

        // GameObject nightVolObj = GameObject.FindWithTag("NightVolume");
        // if (nightVolObj != null) nightVolume = nightVolObj.GetComponent<Volume>();

        // Application de l'état (en mode instantané au chargement de la scène)
        if (currentTimeOfDay == TimeOfDay.Day)
            ApplyDayState(true);
        else
            ApplyNightState(true);

        RefreshCalendarUI();
    }

    private void RefreshCalendarUI()
    {
        if (calendarManager == null) return;
        calendarManager.ChangeDay(currentDay + 1);
        if (currentTimeOfDay == TimeOfDay.Day) calendarManager.SunTime();
        else calendarManager.MoonTime();
    }

    public void StartDay(bool instant = false)
    {
        currentTimeOfDay = TimeOfDay.Day;
        ApplyDayState(instant);
        Debug.Log($"☀️ Jour {currentDay + 1}");
        
        if (calendarManager != null)
        {
            calendarManager.SunTime();
            calendarManager.ChangeDay(currentDay + 1);
        }
    }

    public void StartNight(bool instant = false)
    {
        if (currentTimeOfDay == TimeOfDay.Night && !instant) return;
        
        currentTimeOfDay = TimeOfDay.Night;
        ghostsRequiredThisNight = baseGhostsFirstNight + currentDay;

        ApplyNightState(instant);
        
        if (PumpkinCounter.PumpkinInstance != null)
            PumpkinCounter.PumpkinInstance.SetNightObjective(ghostsRequiredThisNight);

        Debug.Log($"🌙 Nuit {currentDay + 1}");

        if (calendarManager != null) calendarManager.MoonTime();
        if (ghostCycleManager == null) ghostCycleManager = FindFirstObjectByType<GhostCycleManager>();
        if (ghostCycleManager != null) ghostCycleManager.StartCycle();
    }

    public void EndNight()
    {
        if (ghostCycleManager != null) ghostCycleManager.StopCycle();
        if (bellManager != null) bellManager.isBellActivated = false;

        SpawnKeyForCurrentDay();
        currentDay++;
        
        // StartDay classique (avec la transition douce de 3 secondes)
        StartDay(); 
    }

    private void SpawnKeyForCurrentDay()
    {
        if (KeyManager.KeyInstance != null) KeyManager.KeyInstance.SpawnKey(currentDay);
    }

    private void ApplyDayState(bool instant = false)
    {
        if (ghostCycleManager != null) ghostCycleManager.enabled = false;
        if (bell != null) bell.SetEnabled(true);
        if (bellManager != null) bellManager.isBellActivated = false;

        // Le jour : Le Volume Jour passe à 1 (100%), le Volume Nuit passe à 0
        StartVolumeTransition(1f, 0f, instant);
    }

    private void ApplyNightState(bool instant = false)
    {
        if (ghostCycleManager != null) ghostCycleManager.enabled = true;
        if (bell != null) bell.SetEnabled(false);

        // La nuit : Le Volume Jour passe à 0, le Volume Nuit passe à 1 (100%)
        StartVolumeTransition(0f, 1f, instant);
    }

    private void StartVolumeTransition(float targetDayVol, float targetNightVol, bool instant = false)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);

        if (instant)
        {
            if (dayVolume != null) dayVolume.weight = targetDayVol;
            if (nightVolume != null) nightVolume.weight = targetNightVol;
        }
        else
        {
            transitionCoroutine = StartCoroutine(FadeVolumes(targetDayVol, targetNightVol));
        }
    }

    private IEnumerator FadeVolumes(float targetDayVol, float targetNightVol)
    {
        // On récupère les poids actuels pour commencer la transition
        float startDayVol = (dayVolume != null) ? dayVolume.weight : 0f;
        float startNightVol = (nightVolume != null) ? nightVolume.weight : 0f;
        
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            
            // On fait le fondu croisé des deux Volumes
            if (dayVolume != null) dayVolume.weight = Mathf.Lerp(startDayVol, targetDayVol, t);
            if (nightVolume != null) nightVolume.weight = Mathf.Lerp(startNightVol, targetNightVol, t);

            yield return null;
        }

        // On sécurise l'arrivée exacte sur la valeur cible
        if (dayVolume != null) dayVolume.weight = targetDayVol;
        if (nightVolume != null) nightVolume.weight = targetNightVol;
        
        transitionCoroutine = null;
    }
}