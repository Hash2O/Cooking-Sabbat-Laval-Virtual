using System.Collections;
using UnityEngine;

public class IngredientSpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool isSpawning = false; // empêche les doublons
    private GameObject currentIngredient; // référence actuelle

    void Start()
    {
        StartCoroutine(SpawnNewIngredient(1));
    }

    private void OnTriggerExit(Collider other)
    {
        // Vérifie qu’il s’agit bien de l'ingrédient actuel
        if (!isSpawning && other.gameObject == currentIngredient) // && other.gameObject.name == ingredientName
        {
            Debug.Log("Ingredient " + currentIngredient.name + " grabbed — scheduling respawn...");
            StartCoroutine(SpawnNewIngredient(2));
        }
    }

    IEnumerator SpawnNewIngredient(int time)
    {
        isSpawning = true;
        yield return new WaitForSeconds(time);

        // Crée une nouvelle bouteille et garde sa référence
        currentIngredient = Instantiate(ingredientPrefab, spawnPoint.position, spawnPoint.rotation);
        isSpawning = false;
    }
}
