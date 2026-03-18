using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CoinTriggerDoor : MonoBehaviour
{
    [Header("Réglages")]
    public string coinTag = "Coin";     // Tag des objets à détecter
    public int requiredCoins = 3;       // Nombre d'objets nécessaires

    [Header("Référence porte")]
    public Animator doorAnimator;       // Animator de la porte (optionnel)
    public string openTriggerName = "Open"; // Nom du trigger dans l'Animator
    public AudioSource doorAudioSource;
    public string doorID = "LibraryDoor";

    [Header("Référence compteur tirelire")]
    public TextMeshProUGUI compteurTirelire;

    [Header("Clé dans la serrure")]
    public XRSocketInteractor keySocket;    // La socket où la clé doit être placée
    public string requiredKeyTag = "LibraryKey";   // Tag de l’objet-clé

    [HideInInspector]
    public bool keyInserted = false;
    [HideInInspector]
    public int currentCoinsInTrigger = 0;
    private bool doorOpened = false;

    private void Start()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnKeyInserted);
            keySocket.selectExited.AddListener(OnKeyRemoved);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(coinTag)) return;

        currentCoinsInTrigger++;
        Destroy(other.gameObject, 0.5f);

        Debug.Log("Pièces dans la tirelire : " + currentCoinsInTrigger);
        compteurTirelire.text = currentCoinsInTrigger.ToString();

        CheckCondition();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(coinTag)) return;

        CoinRemoving();
    }

    public void CoinRemoving()
    {
        currentCoinsInTrigger--;
        if (currentCoinsInTrigger < 0) currentCoinsInTrigger = 0;
        compteurTirelire.text = currentCoinsInTrigger.ToString();
    }

    private void OnKeyInserted(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag(requiredKeyTag))
        {
            keyInserted = true;
            Debug.Log("Clé insérée dans la serrure !");
            CheckCondition();
        }
    }

    private void OnKeyRemoved(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag(requiredKeyTag))
        {
            keyInserted = false;
            Debug.Log("Clé retirée de la serrure.");
        }
    }

    private void CheckCondition()
    {
        if (doorOpened) return;

        if (currentCoinsInTrigger >= requiredCoins && keyInserted)
        {
            doorOpened = true;

            ExplorationProgressManager.ExplorationInstance.UnlockDoor(doorID);

            Debug.Log("Conditions remplies : ouverture de la porte !");

            if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
            {
                doorAnimator.SetTrigger(openTriggerName);

                if (AudioManager.audioInstance != null)
                    AudioManager.audioInstance.PlayTheGoodSound(6);
            }
        }
    }

}

