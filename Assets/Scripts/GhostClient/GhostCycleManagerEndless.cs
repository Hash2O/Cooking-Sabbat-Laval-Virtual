using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostCycleManagerEndless : MonoBehaviour
{
    [Header("Points de déplacement")]
    public Transform ghostSpawnPoint;
    public Transform ghostWaitPoint;
    public Transform ghostExitPoint;

    [Header("Gestion des fantômes")]
    //public List<GameObject> ghostPrefabs;
    public List<GameObject> availableGhosts = new();
    private GhostClient activeGhost;
    private bool firstGhost = true;

    [Header("Timers")]
    public float spawnDelay = 2f;
    public float exitDelay = 3f;
    public float timeBeforeVanish = 5f;

    [Tooltip("Temps maximum qu’un fantôme attend sa potion avant de partir (en secondes).")]
    public float maxWaitTime = 180f;

    [Header("Référence au compteur de citrouilles")]
    [SerializeField] private PumpkinCounterEndless pumpkinCounter;

    [Header("Référence à la tirelire")]
    [SerializeField] private CoinTriggerDoor coinTrigger;

    [Header("Gestion Présence/Absence XR Rig dans la cuisine")]
    public bool isPlayerInside = false; // gère l'absence ou la présence du XR Rig dans la "cuisine"
    public bool isPaused = false;   // gel du timer patience si le joueur sort

    [Header("Patience penalties")]
    [Tooltip("Temps retiré à la patience du fantôme en cas de mauvaise potion")]
    public float wrongPotionPenalty = 30f;
    [Header("Références Endless Mode")]
    public  EndlessModeManager endlessModeManager;
    public PostProcessManager postProcessManager;
    public bool endlessModeTimeOut = false;

    private bool isSpawning = false;
    private float remainingWaitTime;    // Stockage du temps restant quand le fantôme est satisfait (gestion récompense bonus)

    private void Awake()
    {
        pumpkinCounter = FindFirstObjectByType<PumpkinCounterEndless>();
        coinTrigger = FindFirstObjectByType<CoinTriggerDoor>();
    }

    private void Start()
    {
        firstGhost = true;
        StartCoroutine(GhostCycleLoop());
        if(endlessModeManager != null) endlessModeManager.StartCountdown();
    }

    public void StopCycle()
    {
        StopAllCoroutines();
        // Faire partir le fantôme actif proprement
    }

    private IEnumerator GhostCycleLoop()
    {
        while (true)
        {
            // Tant que le joueur n’est PAS dans la zone → pause
            while (!isPlayerInside)
                yield return null;

            // Tant qu’on est en pause, SpawnGhost n’est pas lancé.
            if (!isSpawning && activeGhost == null && !endlessModeTimeOut)
            {
                yield return StartCoroutine(SpawnGhost());
            }

            yield return null;
        }
    }

    private IEnumerator SpawnGhost()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);

        if (availableGhosts.Count == 0)
        {
            Debug.LogWarning("Aucun fantôme disponible !");
            isSpawning = false;
            yield break;
        }

        GameObject prefab;
        if(firstGhost)
        {
            prefab = availableGhosts[6];
            firstGhost = false;
        }
        else
        {
            prefab = availableGhosts[UnityEngine.Random.Range(0, availableGhosts.Count)];
        }
        GameObject ghostObj = Instantiate(prefab, ghostSpawnPoint.position, Quaternion.identity);
        activeGhost = ghostObj.GetComponent<GhostClient>();

        NavMeshAgent agent = ghostObj.GetComponent<NavMeshAgent>();

        // Aller au comptoir
        if (agent != null)
            agent.SetDestination(ghostWaitPoint.position);

        // Son d’alerte
        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(1);

        // Attente que le fantôme arrive au point de patience
        yield return StartCoroutine(WaitForGhostToReach(agent, ghostWaitPoint.position));

        // Lancer le timer de patience
        bool potionDelivered = false;
        yield return StartCoroutine(GhostWaitTimer(() => potionDelivered = activeGhost.isSatisfied));

        // Si la potion a été donnée à temps
        if (activeGhost.isSatisfied)
        {
            GiveReward();
            if (pumpkinCounter != null)
            {
                pumpkinCounter.RegisterSatisfiedClient();
            }
        }
        else
        {
            Debug.Log("Le fantôme est parti frustré (pas de potion à temps).");
            if (pumpkinCounter != null)
            {
                pumpkinCounter.RegisterUnsatisfiedClient();
            }
        }

        // Départ du fantôme
        if (agent != null)
            agent.SetDestination(ghostExitPoint.position);

        yield return new WaitForSeconds(exitDelay);
        yield return new WaitForSeconds(timeBeforeVanish);

        Destroy(ghostObj);
        activeGhost = null;
        isSpawning = false;
    }

    /// <summary>
    /// Attend que le fantôme soit proche de son point d’attente.
    /// </summary>
    private IEnumerator WaitForGhostToReach(NavMeshAgent agent, Vector3 target)
    {
        if (agent == null) yield break;

        // On laisse une fraction de seconde à l'agent pour calculer son chemin
        yield return new WaitForSeconds(0.1f);

        // On utilise la distance sur le chemin (remainingDistance) qui est insensible aux différences de hauteur (Y)
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            yield return null;
        }
    }

    private IEnumerator GhostWaitTimer(System.Func<bool> isSatisfiedCheck)
    {
        remainingWaitTime = maxWaitTime;
        if (activeGhost.ghostRenderer != null)
        {
            foreach(Material mat in activeGhost.ghostRenderer.materials)
                mat.SetFloat("_Alpha", 1.0f);
        }
        while (remainingWaitTime > 0f)
        {
            if (isSatisfiedCheck())
            {
                if (activeGhost.patienceBar != null)
                    activeGhost.patienceBar.SetVisible(false);
                if (activeGhost.ghostRenderer != null)
                {
                    foreach(Material mat in activeGhost.ghostRenderer.materials)
                    {
                        mat.SetFloat("_Alpha", Mathf.Lerp(activeGhost.ghostRenderer.material.GetFloat("_Alpha"), 1.0f, 0.75f));
                        if ( mat.name == "Ghost (Instance)" ) 
                        {
                            mat.SetColor("_MainColor",Color.Lerp(activeGhost.ghostRenderer.material.GetColor("_MainColor"), activeGhost.requestedRecipe.potionColor, 1.5f));
                        }
                    }
                        
                }

                yield break;
            }
            while (isPaused)
                yield return null;

            remainingWaitTime -= Time.deltaTime;

            if (activeGhost.patienceBar != null)
            {
                float remainingPercent = remainingWaitTime / maxWaitTime;
                activeGhost.patienceBar.SetFill(remainingPercent);
            }
            if (activeGhost.ghostRenderer != null)
            {
                float remainingPercent = remainingWaitTime / maxWaitTime;
                foreach(Material mat in activeGhost.ghostRenderer.materials)
                {
                    mat.SetFloat("_Alpha", remainingPercent);
                }
            }

            yield return null;
        }

        if (activeGhost.patienceBar != null)
            activeGhost.patienceBar.SetVisible(false);

        // Temps écoulé
        Debug.Log("⏰ Temps écoulé ! Le fantôme part sans potion, vidant en partie la tirelire en partant.");

        if (activeGhost.patienceBar != null) activeGhost.patienceBar.SetVisible(false); // Désactivation de la barre de patience
        if (coinTrigger != null) coinTrigger.CoinRemoving();    // Si pièce dans la tirelire, le fantôme en enlève une 
        if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(1);    // Fail Notification Horror
        

    }

    public void ApplyWrongPotionPenalty()
    {
        remainingWaitTime -= wrongPotionPenalty;
        remainingWaitTime = Mathf.Max(remainingWaitTime, 0f);

        if(postProcessManager != null)
            postProcessManager.DarkenScreen();

        Debug.Log($"❌ Mauvaise potion ! -{wrongPotionPenalty} secondes de patience.");
    }

    private void GiveReward()
    {
        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(11);    // Fairy Cartoon Success Voice


        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(0); // Cashing Sound

        if (endlessModeManager != null)
            endlessModeManager.AddBonusTime();

        FindFirstObjectByType<ScoreManagerEndless>().AddSuccessfulOrder(activeGhost.name);
    }

    public IEnumerator EndGame()
    {
        StopAllCoroutines();
        foreach(Material mat in activeGhost.ghostRenderer.materials)
                mat.SetFloat("_Alpha", Mathf.Lerp(activeGhost.ghostRenderer.material.GetFloat("_Alpha"), 0.0f, 0.35f));
        
        yield return new WaitForSeconds(0.5f);
        Destroy(activeGhost.gameObject);
        activeGhost = null;
    }
}