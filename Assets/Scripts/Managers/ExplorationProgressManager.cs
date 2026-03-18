//using System.Collections.Generic;
//using UnityEngine;

//public class ExplorationProgressManager : MonoBehaviour
//{
//    public static ExplorationProgressManager Instance { get; private set; }

//    [Header("Ingrédients débloqués")]
//    public List<string> unlockedIngredients = new List<string>();

//    // Visible pour vérification
//    public IngredientSpawner[] allSpawners;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    private void Start()
//    {
//        InitializeSpawners();
//    }

//    private void InitializeSpawners()
//    {
//        allSpawners = FindObjectsOfType<IngredientSpawner>();

//        foreach (var spawner in allSpawners)
//        {
//            if (spawner.availableAtStart)
//            {
//                UnlockIngredient(spawner.ingredientID);
//            }
//            else
//            {
//                spawner.DisableSpawner();
//            }
//        }

//        // Pour que ça fonctionne même quand on recharge une scène (cuisine ↔ exploration)
//        foreach (var ingredient in unlockedIngredients)
//        {
//            ActivateIngredientSpawner(ingredient);
//        }
//    }

//    public void UnlockIngredient(string ingredientID)
//    {
//        if (unlockedIngredients.Contains(ingredientID))
//            return;

//        unlockedIngredients.Add(ingredientID);

//        Debug.Log($"Ingrédient débloqué : {ingredientID}");

//        ActivateIngredientSpawner(ingredientID);
//    }

//    private void ActivateIngredientSpawner(string ingredientID)
//    {
//        foreach (var spawner in allSpawners)
//        {
//            if (spawner.ingredientID == ingredientID)
//            {
//                spawner.EnableSpawner();
//            }
//        }
//    }
//}

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class ExplorationProgressManager : MonoBehaviour
//{
//    public static ExplorationProgressManager ExplorationInstance { get; private set; }

//    [Header("Ingrédients débloqués")]
//    public List<string> unlockedIngredients = new List<string>();

//    // Visible pour debug
//    public IngredientSpawner[] allSpawners;

//    private void Awake()
//    {
//        if (ExplorationInstance != null && ExplorationInstance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        ExplorationInstance = this;
//        DontDestroyOnLoad(gameObject);

//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    private void OnDestroy()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }

//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        InitializeSpawners();
//    }

//    private void InitializeSpawners()
//    {
//        allSpawners = FindObjectsOfType<IngredientSpawner>(true);

//        foreach (var spawner in allSpawners)
//        {
//            // Si ingrédient déjà débloqué
//            if (unlockedIngredients.Contains(spawner.ingredientID))
//            {
//                spawner.EnableSpawner();
//            }
//            else if (spawner.availableAtStart)
//            {
//                UnlockIngredient(spawner.ingredientID);
//            }
//            else
//            {
//                spawner.DisableSpawner();
//            }
//        }
//    }

//    public void UnlockIngredient(string ingredientID)
//    {
//        if (unlockedIngredients.Contains(ingredientID))
//            return;

//        unlockedIngredients.Add(ingredientID);

//        Debug.Log($"Ingrédient débloqué : {ingredientID}");

//        ActivateIngredientSpawner(ingredientID);
//    }

//    private void ActivateIngredientSpawner(string ingredientID)
//    {
//        if (allSpawners == null) return;

//        foreach (var spawner in allSpawners)
//        {
//            if (spawner.ingredientID == ingredientID)
//            {
//                spawner.EnableSpawner();
//            }
//        }
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationProgressManager : MonoBehaviour
{
    public static ExplorationProgressManager ExplorationInstance { get; private set; }

    [Header("Ingrédients débloqués")]
    public List<string> unlockedIngredients = new List<string>();   // Improvement : use HashSet instead of List (quicker, no double)

    [Header("Portes débloquées")]
    public List<string> unlockedDoors = new List<string>();     // Improvement : use HashSet instead of List (quicker, no double)

    [Header("Events déjà joués")]
    public List<string> completedEvents = new List<string>();   // Improvement : use HashSet instead of List (quicker, no double)

    [Header("Objets de rituel")]
    public List<string> collectedQuestItems = new List<string>();
    public int totalQuestItemsRequired = 6;

    // Spawners présents dans la scène actuelle
    private IngredientSpawner[] allSpawners;

    [System.Obsolete]
    private void Awake()
    {
        if (ExplorationInstance != null && ExplorationInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        ExplorationInstance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [System.Obsolete]
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [System.Obsolete]
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeSpawners();
        InitializeDoors();
    }

    // ----------------------------
    // EVENTS
    // ----------------------------

    public bool IsEventCompleted(string eventID)
    {
        return completedEvents.Contains(eventID);
    }

    public void CompleteEvent(string eventID)
    {
        if (!completedEvents.Contains(eventID))
        {
            completedEvents.Add(eventID);
        }
    }

    // ----------------------------
    // OBJECTS FOR THE RITUAL
    // ----------------------------

    public void CollectQuestItem(string itemID)
    {
        if (collectedQuestItems.Contains(itemID))
            return;

        collectedQuestItems.Add(itemID);

        Debug.Log($"Objet de quête récupéré : {itemID}");
    }

    public bool HasCollectedItemForToday()
    {
        return collectedQuestItems.Count > GameCycleManager.GameCycleInstance.currentDay;
    }

    // ----------------------------
    // INGREDIENTS
    // ----------------------------

    [System.Obsolete]
    private void InitializeSpawners()
    {
        allSpawners = FindObjectsOfType<IngredientSpawner>(true);

        foreach (var spawner in allSpawners)
        {
            if (unlockedIngredients.Contains(spawner.ingredientID))
            {
                spawner.EnableSpawner();
            }
            else if (spawner.availableAtStart)
            {
                UnlockIngredient(spawner.ingredientID);
            }
            else
            {
                spawner.DisableSpawner();
            }
        }
    }

    public void UnlockIngredient(string ingredientID)
    {
        if (unlockedIngredients.Contains(ingredientID))
            return;

        unlockedIngredients.Add(ingredientID);

        Debug.Log($"Ingrédient débloqué : {ingredientID}");

        ActivateIngredientSpawner(ingredientID);
    }

    private void ActivateIngredientSpawner(string ingredientID)
    {
        if (allSpawners == null) return;

        foreach (var spawner in allSpawners)
        {
            if (spawner.ingredientID == ingredientID)
            {
                spawner.EnableSpawner();
            }
        }
    }

    // ----------------------------
    // PORTES
    // ----------------------------

    public void UnlockDoor(string doorID)
    {
        if (unlockedDoors.Contains(doorID))
            return;

        unlockedDoors.Add(doorID);

        Debug.Log($"Porte débloquée : {doorID}");
    }

    public bool IsDoorUnlocked(string doorID)
    {
        return unlockedDoors.Contains(doorID);
    }

    [System.Obsolete]
    private void InitializeDoors()
    {
        DoorStatusManagement[] doors = FindObjectsOfType<DoorStatusManagement>(true);   // FindObjectsOfType<DoorStatusManagement>(true);

        foreach (var door in doors)
        {
            if (IsDoorUnlocked(door.doorID))
            {
                door.UnlockDoorVisual();
            }
        }
    }
}