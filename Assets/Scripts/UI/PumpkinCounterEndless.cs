using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PumpkinCounterEndless : MonoBehaviour
{
    public static PumpkinCounterEndless PumpkinInstance { get; private set; }

    [Header("Liste des citrouilles (feedback visuel uniquement)")]
    public List<GameObject> pumpkins = new List<GameObject>();

    [Header("Progression Nuit en cours")]
    [Tooltip("Nombre de clients satisfaits durant la nuit actuelle")]
    public int satisfiedClients = 0;



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

    #region Client Registration

    public void RegisterSatisfiedClient()
    {
        satisfiedClients++;

        UpdatePumpkinVisual();

        // if (AudioManager.audioInstance != null)
        //     AudioManager.audioInstance.PlayTheGoodSound(5);

        Debug.Log($"Client satisfait ({satisfiedClients})");
    }

    public void RegisterUnsatisfiedClient()
    {
        if (satisfiedClients <= 0)
            return;

        satisfiedClients--;

        UpdatePumpkinVisual();

        if (AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(8);

        Debug.Log($"Client perdu ({satisfiedClients})");
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