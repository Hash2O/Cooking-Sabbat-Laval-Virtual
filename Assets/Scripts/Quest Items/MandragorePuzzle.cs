using UnityEngine;


public class MandragorePuzzle : MonoBehaviour
{
    [Header("Références")]
    public GameObject mandragoreToSpawn; // Glisse ici la fausse mandragore (enfant)
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor clocheSocket; // Glisse ici le Quest Item Garden Slot - Cloche
    public string questID = "Mandragore";

    private bool isQuestValidated = false;

    // Cette fonction sera appelée pour vérifier si on a gagné
    public void CheckPuzzleCompletion()
    {
        if (isQuestValidated) return;

        // Est-ce que la plante est apparue ET la cloche est posée ?
        if (mandragoreToSpawn.activeInHierarchy && clocheSocket.hasSelection)
        {
            isQuestValidated = true;

            // 1. Couper le cri de la fausse mandragore
            AudioSource audio = mandragoreToSpawn.GetComponent<AudioSource>();
            if (audio != null) audio.Stop();

            // 2. Valider la quête (comme le faisait ton ancien script)
            ExplorationProgressManager.ExplorationInstance.HasCollectedItemForToday();
            ExplorationProgressManager.ExplorationInstance.CollectQuestItem(questID);

            // 3. Son de succès
            if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(3);
        }
    }
}