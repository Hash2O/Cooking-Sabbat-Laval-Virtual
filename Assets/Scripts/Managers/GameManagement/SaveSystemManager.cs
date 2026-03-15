using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Playables;

public class SaveSystemManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CoinTriggerDoor coin;
    [SerializeField] private PumpkinCounter pumpkinCounter;
    [SerializeField] private InGameUIManager gameUIManager;

    // Needs to be associated in Hierarchy
    [Header("Game Mechanics")]
    [SerializeField] private GhostCycleManager ghostCycleManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assign all possible references at start
        if (playerTransform == null) playerTransform = FindFirstObjectByType<XROrigin>().transform;
        if (coin == null) coin = FindFirstObjectByType<CoinTriggerDoor>();
        if (pumpkinCounter == null) pumpkinCounter = FindFirstObjectByType<PumpkinCounter>();
        if (gameUIManager == null) gameUIManager = FindFirstObjectByType<InGameUIManager>();

        // A décommenter si on veut lancer le jeu en récupérant les données sauvegardées directement
        //if (GameManager.Instance.loadSavedData == false)
        //{
        //    Debug.Log("No saved data found.");
        //    return;
        //}
        //else
        //{
        //    LoadData();
        //    gameUIManager.ToggleNarrativeStory();
        //    Debug.Log("Datas have been loaded.");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData()
    {
        SavedData savedData = new SavedData
        {
            playerPosition = playerTransform.position,
            coinInPocket = coin.currentCoinsInTrigger,
            satisfiedClients = pumpkinCounter.satisfiedClients,
        };

        // Formatting game data to json format
        string jsonData = JsonUtility.ToJson(savedData);

        // Initializing path to file where saved data are stored
        string filePath = Application.persistentDataPath + "/SavedData.json";
        Debug.Log("Persistent Data Path : " + filePath);

        // Generating file and writing saved data inside
        System.IO.File.WriteAllText(filePath, jsonData);

        Debug.Log("Datas have been saved.");
    }

    public void LoadData()
    {

        // File path to retrieve data
        string filePath = Application.persistentDataPath + "/SavedData.json";
        // Read data 
        string jsonData = System.IO.File.ReadAllText(filePath);
        // Formmating data from json to specified format type (SavedData)
        SavedData savedData = JsonUtility.FromJson<SavedData>(jsonData);

        // Load data
        // Position 
        playerTransform.position = savedData.playerPosition;
        // Coins
        coin.currentCoinsInTrigger = savedData.coinInPocket;
        // Update coin counter text
        coin.compteurTirelire.text = savedData.coinInPocket.ToString();
        // Satisfied Clients
        pumpkinCounter.satisfiedClients = savedData.satisfiedClients;
        // Affichage citrouille par client satisfait
        for (int i = 0; i < savedData.satisfiedClients; i++) 
        {
            pumpkinCounter.ActivatePumpkins();
        }

        // Stop Audio Tutorial
        if(gameUIManager.isNarrativeStoryActive == true) gameUIManager.ToggleNarrativeStory();

        // Active GhostCycle if disabled (starting game)
        ActivateGhostCycleManager();

        Debug.Log("Datas have been loaded.");
    }

    private void ActivateGhostCycleManager()
    {
        if (ghostCycleManager.isActiveAndEnabled)
        {
            Debug.Log("Ghost Cycle Manager already active.");
            return;
        }
        else
        {
            Debug.Log("Ghost Cycle Manager found disabled and then enabled.");
            ghostCycleManager.gameObject.SetActive(true);
        }
    }
}

public class SavedData
{
    public Vector3 playerPosition;
    public int coinInPocket;
    public int satisfiedClients;
}
