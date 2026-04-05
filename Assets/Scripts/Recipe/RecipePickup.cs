using UnityEngine;
using DG.Tweening; // Ne pas oublier d'importer DOTween !
 // --- NOUVEAU 1 : Indispensable pour la VR ---

public class RecipePickup : MonoBehaviour
{
    public RecipeData recipe;

    public Renderer parchmentRenderer;
    public float dissolveDuration = 1.5f;

    private bool isCollected = false; 
    
    // --- NOUVEAU 2 : La variable pour savoir si la main tient l'objet ---
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; 

    private void Start() 
    {
        parchmentRenderer = this.gameObject.GetComponent<MeshRenderer>();
        
        // On récupère le composant VR qui est sur le même objet
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(); 
    }
    
    public void Collect()
    {
        if (recipe == null) return;

        RecipeManager.RecipeInstance.DiscoverRecipe(recipe);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // --- NOUVEAU 3 : On ajoute la condition "grabInteractable.isSelected" ---
        // Le script vérifie : 1. C'est le joueur ? 2. C'est pas déjà collecté ? 3. JE LE TIENS DANS MA MAIN ?
        if (other.CompareTag("Player") && !isCollected && grabInteractable != null && grabInteractable.isSelected)
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