using UnityEngine;

public class IngredientUnlock : MonoBehaviour
{
    public string ingredientID;

    // A utiliser si besoin de faire apparaitre un GO pour améliorer l'UX, sinon, à commenter
    public GameObject ingredientToSpawn;

    private void Start()
    {
        if (ingredientToSpawn != null) ingredientToSpawn.SetActive(false);
    }

    public void UnlockIngredient()
    {
        ExplorationProgressManager.ExplorationInstance.UnlockIngredient(ingredientID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Herb"))
        {
            UnlockIngredient();
            Destroy(other.gameObject);
            if (ingredientToSpawn != null) ingredientToSpawn.SetActive(true);
        }
    }
}
