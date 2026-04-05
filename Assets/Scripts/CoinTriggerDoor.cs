using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CoinTriggerDoor : MonoBehaviour
{
    [Header("R�glages")]
    public string coinTag = "Coin";     // Tag des objets � d�tecter
    public int requiredCoins = 3;       // Nombre d'objets n�cessaires

    [Header("R�f�rence porte")]
    public Animator doorAnimator;       // Animator de la porte (optionnel)
    public string openTriggerName = "Open"; // Nom du trigger dans l'Animator
    public AudioSource doorAudioSource;
    public string doorID = "LibraryDoor";

    [Header("R�f�rence compteur tirelire")]
    public TextMeshProUGUI compteurTirelire;

    [Header("Cl� dans la serrure")]
    public XRSocketInteractor keySocket;    // La socket o� la cl� doit �tre plac�e
    public string requiredKeyTag = "LibraryKey";   // Tag de l�objet-cl�

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

        Debug.Log("Pi�ces dans la tirelire : " + currentCoinsInTrigger);
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
            Debug.Log("Cl� ins�r�e dans la serrure !");
            CheckCondition();
        }
    }

    private void OnKeyRemoved(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag(requiredKeyTag))
        {
            keyInserted = false;
            Debug.Log("Cl� retir�e de la serrure.");
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

