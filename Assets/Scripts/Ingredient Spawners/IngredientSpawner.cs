using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [Header("Identification")]
    public string ingredientID;

    [Header("Disponibilité")]
    public bool availableAtStart = false;

    public void EnableSpawner()
    {
        gameObject.SetActive(true);

        Debug.Log($"Spawner activé : {ingredientID}");
    }

    public void DisableSpawner()
    {
        gameObject.SetActive(false);
    }
}
