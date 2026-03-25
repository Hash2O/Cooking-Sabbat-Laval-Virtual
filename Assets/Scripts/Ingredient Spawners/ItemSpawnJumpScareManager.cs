using UnityEngine;

public class ItemSpawnJumpScareManager : MonoBehaviour
{

    public string itemID;
    public GameObject itemToSpawn;
    public ParticleSystem vfxToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemID))
        {
            Debug.Log("Contact ! ");
            if (itemToSpawn != null) itemToSpawn.SetActive(true);
            if (vfxToSpawn != null) vfxToSpawn.Play();
            if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayGhostSound(1);   // Angry Ghost Voice
            Destroy(other.gameObject);
        }
    }
}
