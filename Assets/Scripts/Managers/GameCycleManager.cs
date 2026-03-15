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

using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject directionalLight;
    public BellInteraction bell;
    public BellManager bellManager;

    [Header("Objectifs")]
    public int baseGhostsFirstNight = 5;

    private int ghostsRequiredThisNight;

    private bool hasInitialized = false;

    void Awake()
    {
        // Singleton robuste
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
            StartDay();
            hasInitialized = true;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reconnecter les références de la scène
        ghostCycleManager = FindFirstObjectByType<GhostCycleManager>();
        directionalLight = GameObject.FindWithTag("Sun");             // FindFirstObjectByType<Light>();
        bell = FindFirstObjectByType<BellInteraction>();
        bellManager = FindFirstObjectByType<BellManager>();

        // Réappliquer l'état du cycle
        if (currentTimeOfDay == TimeOfDay.Day)
        {
            ApplyDayState();
        }
        else
        {
            ApplyNightState();
        }
    }

    public void StartDay()
    {
        currentTimeOfDay = TimeOfDay.Day;

        ApplyDayState();

        Debug.Log($"☀️ Jour {currentDay + 1}");
    }

    public void StartNight()
    {
        if (currentTimeOfDay == TimeOfDay.Night)
            return;

        currentTimeOfDay = TimeOfDay.Night;

        ghostsRequiredThisNight = baseGhostsFirstNight + currentDay;

        ApplyNightState();

        PumpkinCounter.PumpkinInstance.SetNightObjective(ghostsRequiredThisNight);

        Debug.Log($"🌙 Nuit {currentDay + 1}");

        if (ghostCycleManager == null)
        {
            ghostCycleManager = FindFirstObjectByType<GhostCycleManager>();
        }

        if (ghostCycleManager != null)
        {
            ghostCycleManager.StartCycle();
            Debug.Log("StartCycle() : " + ghostCycleManager.cycleActive);
        }
        else
        {
            Debug.LogWarning("GhostCycleManager introuvable !");
        }

        //Debug.Log("StartCycle() : " + ghostCycleManager.cycleActive);
    }

    public void EndNight()
    {
        if (ghostCycleManager != null)
            ghostCycleManager.StopCycle();

        bellManager.isBellActivated = false;

        SpawnKeyForCurrentDay();

        currentDay++;

        StartDay();
    }

    private void SpawnKeyForCurrentDay()
    {
        if (KeyManager.KeyInstance != null)
        {
            KeyManager.KeyInstance.SpawnKey(currentDay);
        }
    }

    private void ApplyDayState()
    {
        if (ghostCycleManager != null)
            ghostCycleManager.enabled = false;

        if (directionalLight != null)
            directionalLight.GetComponent<Light>().intensity = 1.0f;

        if (bell != null)
            bell.SetEnabled(true);

        if (bellManager != null)
            bellManager.isBellActivated = false;
    }

    private void ApplyNightState()
    {
        if (ghostCycleManager != null)
            ghostCycleManager.enabled = true;

        if (directionalLight != null)
            directionalLight.GetComponent<Light>().intensity = 0.3f;

        if (bell != null)
            bell.SetEnabled(false);
    }
}
