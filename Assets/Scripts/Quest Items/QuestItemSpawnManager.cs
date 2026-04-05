using UnityEngine;

public class QuestItemSpawnManager : MonoBehaviour
{
    public string itemID;
    public GameObject itemToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemID))
        {
            if (itemToSpawn != null)
            {
                // 1. On cherche les sources audio AVANT de détruire l'original
                // (On utilise GetComponentInChildren au cas où l'audio est sur l'objet ou un de ses enfants)
                AudioSource oldAudio = other.GetComponentInChildren<AudioSource>();
                AudioSource newAudio = itemToSpawn.GetComponentInChildren<AudioSource>();

                // 2. On fait apparaître le nouvel objet
                itemToSpawn.SetActive(true);

                // 3. Le transfert de mémoire !
                if (oldAudio != null && newAudio != null)
                {
                    // On copie la seconde exacte de la piste audio
                    newAudio.time = oldAudio.time; 
                    
                    // On force la lecture à partir de ce point précis
                    newAudio.Play(); 
                }
            }
            
            // 4. On détruit la vraie mandragore
            Destroy(other.gameObject);
        }
    }
}