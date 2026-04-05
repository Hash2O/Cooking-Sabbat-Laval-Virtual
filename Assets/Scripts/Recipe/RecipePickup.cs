using UnityEngine;
using DG.Tweening; // Ne pas oublier d'importer DOTween !

public class RecipePickup : MonoBehaviour
{
    public RecipeData recipe;

    public Renderer parchmentRenderer;
    public float dissolveDuration = 1.5f;

    private bool isCollected = false; 

    private void Start() {
        parchmentRenderer = this.gameObject.GetComponent<MeshRenderer>();
    }
    public void Collect()
    {
        if (recipe == null) return;

        RecipeManager.RecipeInstance.DiscoverRecipe(recipe);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true; 
            Debug.Log("Nouvelle recette trouvée !");
            
            if (AudioManager.audioInstance != null) 
                AudioManager.audioInstance.PlayNotificationSound(3);

            if (parchmentRenderer != null)
            {
                parchmentRenderer.material.DOFloat(2f, "_Dissolve", dissolveDuration)
                    .OnComplete(() => 
                    {
                        Collect(); 
                    });
            }
            else
            {
                Collect();
            }
        }
    }
}