//using UnityEngine;
//using System.Collections.Generic;
//using DG.Tweening;

//public class PumpkinCounter : MonoBehaviour
//{
//    [Header("Liste des citrouilles (dans l'ordre)")]
//    public List<GameObject> pumpkins = new List<GameObject>();

//    [Header("Debug")]
//    public int satisfiedClients = 0;

//    // Appel�e par GhostClient lorsqu'un fant�me est satisfait
//    public void RegisterSatisfiedClient()
//    {
//        // Trouver la premi�re citrouille inactive
//        foreach (GameObject pumpkin in pumpkins)
//        {
//            if (!pumpkin.activeSelf)
//            {
//                pumpkin.SetActive(true);
//                satisfiedClients++;

//                // Audio 
//                if(AudioManager.audioInstance != null)
//                    AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification

//                Debug.Log($"Citrouille activ�e ! Total : {satisfiedClients}");

//                CheckForVictory();
//                return;
//            }
//        }

//        Debug.Log("Toutes les citrouilles sont d�j� activ�es (clients suppl�mentaires ?)");
//    }

//    public void ActivatePumpkins()
//    {
//        // Trouver la premi�re citrouille inactive
//        foreach (GameObject pumpkin in pumpkins)
//        {
//            if (!pumpkin.activeSelf)
//            {
//                Vector3 pumpkinSize = pumpkin.transform.localScale;
//                pumpkin.transform.DOScale(0.0f,.0f);

//                pumpkin.SetActive(true);
//                pumpkin.transform.DOScale(pumpkinSize, 2.0f);

//                // Audio 
//                if (AudioManager.audioInstance != null)
//                    AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification

//                Debug.Log($"Citrouille activ�e ! Total : {satisfiedClients}");

//                //CheckForVictory();
//                return;
//            }
//        }

//        Debug.Log("Toutes les citrouilles sont d�j� activ�es (clients suppl�mentaires ?)");
//    }

//    // Appel�e lorsqu'un fant�me repart frustr�
//    public void RegisterUnsatisfiedClient()
//    {
//        // On ne descend pas en-dessous de z�ro
//        if (satisfiedClients <= 0)
//        {
//            Debug.Log("Aucune citrouille � retirer.");
//            return;
//        }

//        // Trouver la derni�re citrouille active
//        for (int i = pumpkins.Count - 1; i >= 0; i--)
//        {
//            if (pumpkins[i].activeSelf)
//            {
//                pumpkins[i].SetActive(false);
//                satisfiedClients--;

//                // Audio �chec
//                if (AudioManager.audioInstance != null)
//                    AudioManager.audioInstance.PlayTheGoodSound(8); // Horror lose

//                Debug.Log($"Citrouille d�sactiv�e... Total : {satisfiedClients}");
//                return;
//            }
//        }
//    }

//    private void CheckForVictory()
//    {
//        if (satisfiedClients >= pumpkins.Count)
//        {
//            Debug.Log("VICTOIRE ! Tous les clients sont satisfaits !");
//            // Ajouter ici : animation, son, popup, fin du niveau, etc...
//        }
//    }
//}

using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PumpkinCounter : MonoBehaviour
{
    public static PumpkinCounter PumpkinInstance { get; private set; }

    [Header("Liste des citrouilles (feedback visuel uniquement)")]
    public List<GameObject> pumpkins = new List<GameObject>();

    [Header("Progression Nuit en cours")]
    [Tooltip("Nombre de clients satisfaits durant la nuit actuelle")]
    public int satisfiedClients = 0;

    [Tooltip("Objectif à atteindre cette nuit")]
    [SerializeField] private int clientsToSatisfyThisNight = 0;

    [Header("Progression Globale")]
    [Tooltip("Nombre total de clients satisfaits depuis le début du jeu")]
    public int totalSatisfiedClients = 0;

    private void Awake()
    {
        if (PumpkinInstance != null && PumpkinInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        PumpkinInstance = this;
        //DontDestroyOnLoad(gameObject);
    }

    #region Night Setup

    public void SetNightObjective(int amount)
    {
        clientsToSatisfyThisNight = amount;
        satisfiedClients = 0;

        ResetPumpkinsVisual();

        Debug.Log($"🎯 Nouvel objectif : {clientsToSatisfyThisNight} fantômes");
    }

    #endregion

    #region Client Registration

    public void RegisterSatisfiedClient()
    {
        satisfiedClients++;
        totalSatisfiedClients++;

        UpdatePumpkinVisual();

        // if (AudioManager.audioInstance != null)
        //     AudioManager.audioInstance.PlayTheGoodSound(5);

        Debug.Log($"Client satisfait ({satisfiedClients}/{clientsToSatisfyThisNight})");

        CheckForNightCompletion();
    }

    public void RegisterUnsatisfiedClient()
    {
        if (satisfiedClients <= 0)
            return;

        satisfiedClients--;

        UpdatePumpkinVisual();

        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(8);

        Debug.Log($"Client perdu ({satisfiedClients}/{clientsToSatisfyThisNight})");
    }

    #endregion

    #region Completion Check

    private void CheckForNightCompletion()
    {
        if (satisfiedClients < clientsToSatisfyThisNight)
            return;

        Debug.Log("🎃 Objectif nocturne atteint !");
        GameCycleManager.GameCycleInstance.EndNight();
    }

    #endregion

    #region Visual Management

    private void UpdatePumpkinVisual()
    {
        for (int i = 0; i < pumpkins.Count; i++)
        {
            if (pumpkins[i] == null) continue;

            if(!pumpkins[i].activeInHierarchy)
            {            
                Vector3 scale = pumpkins[i].transform.localScale;
                pumpkins[i].transform.localScale = Vector3.zero;
                pumpkins[i].SetActive(i < satisfiedClients);
                pumpkins[i].transform.DOScale(scale, 1.5f); 
                AudioManager.audioInstance.PlayTheGoodSound(15); 
            }
        }
    }

    private void ResetPumpkinsVisual()
    {
        foreach (GameObject pumpkin in pumpkins)
        {
            if (pumpkin != null)
                pumpkin.SetActive(false);
        }
    }

    #endregion

    // <summary>
    /// Permet de réactiver les citrouilles en cas de chargement des données de la précédente partie
    /// </summary>
    public void ActivatePumpkins()
    {
        // Trouver la première citrouille inactive
        foreach (GameObject pumpkin in pumpkins)
        {
            if (!pumpkin.activeSelf)
            {
                pumpkin.SetActive(true);

                // Audio 
                if (AudioManager.audioInstance != null)
                    //AudioManager.audioInstance.PlayTheGoodSound(5); // Success Notification

                Debug.Log($"Citrouille activée ! Total : {satisfiedClients}");

                //CheckForVictory();
                return;
            }
        }

        Debug.Log("Toutes les citrouilles sont déjà activées (clients supplémentaires ?)");
    }
}