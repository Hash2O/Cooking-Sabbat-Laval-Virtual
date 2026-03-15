using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallGardenManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> plantIngredients = new List<GameObject>();

    private GameObject currentPlant;

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Ingredient"))
        {
            Debug.Log("Plant Gathered");
            currentPlant = other.gameObject;
            StartCoroutine(nameof(ReInitPlant), 2);
        }
    }

    // Manage respawn of each plant after being gathered by player.
    public IEnumerator ReInitPlant(int time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < plantIngredients.Count; ++i)
        {
            if (plantIngredients[i] == null)
            {
                SpawnPlantIngredient(currentPlant, plantIngredients[i].transform);
            }
        }
    }

    private void SpawnPlantIngredient(GameObject plantIngredient, Transform transform)
    {
        Instantiate(plantIngredient, transform.position, transform.rotation);
    }

}
