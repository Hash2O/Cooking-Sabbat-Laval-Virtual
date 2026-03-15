using System.Collections;
using UnityEngine;

public class BottleSpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject bottlePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string tagName = "EmptyBottle";

    private bool isSpawning = false; // empêche les doublons
    private GameObject currentBottle; // référence actuelle

    void Start()
    {
        StartCoroutine(SpawnNewBottle(1));
    }

    private void OnTriggerExit(Collider other)
    {
        // Vérifie qu’il s’agit bien de la bouteille actuelle
        if (!isSpawning && other.gameObject.CompareTag(tagName) && other.gameObject == currentBottle)
        {
            Debug.Log("Bottle grabbed — scheduling respawn...");
            StartCoroutine(SpawnNewBottle(2));
        }
    }

    IEnumerator SpawnNewBottle(int time)
    {
        isSpawning = true;
        yield return new WaitForSeconds(time);

        // Crée une nouvelle bouteille et garde sa référence
        currentBottle = Instantiate(bottlePrefab, spawnPoint.position, spawnPoint.rotation);
        isSpawning = false;
    }
}
