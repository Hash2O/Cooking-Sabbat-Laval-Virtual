using UnityEngine;

public class RecipePickup : MonoBehaviour
{
    public RecipeData recipe;

    public void Collect()
    {
        if (recipe == null) return;

        RecipeManager.RecipeInstance.DiscoverRecipe(recipe);
        // Insérer  VFX ici (dissolve ?)
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Nouvelle recette trouvée !");
            Collect();
            if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(3);
        }
    }
}
