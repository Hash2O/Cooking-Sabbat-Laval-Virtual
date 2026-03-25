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
        if(ExplorationProgressManager.ExplorationInstance != null) ExplorationProgressManager.ExplorationInstance.UnlockIngredient(ingredientID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ingredientID))
        {
            Debug.Log("IngredientUnlock : Entrée");
            UnlockIngredient();
            if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(3);
            Destroy(other.gameObject);
            if (ingredientToSpawn != null) ingredientToSpawn.SetActive(true);
            Debug.Log("IngredientUnlock : Sortie");
        }
    }
}
