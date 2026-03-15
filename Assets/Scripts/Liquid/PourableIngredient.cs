using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class PourableIngredient : MonoBehaviour
{
    [Header("Données de l'ingrédient")]
    [Tooltip("Référence à l'IngredientData associé à ce liquide")]
    public IngredientData ingredientData;

    [Header("Réglages du versement")]
    [Tooltip("Nombre de particules à détecter pour valider l'ingrédient")]
    public int particlesNeededForValidation = 5;

    [Tooltip("Temps minimum entre deux validations de versement")]
    public float cooldownBetweenPours = 3.0f;

    [Tooltip("Effet sonore optionnel lors du versement validé")]
    public AudioSource pourSuccessSound;

    private int particleHitCount = 0;
    private bool canPour = true;
    private Cauldron currentCauldron;

    private void OnParticleCollision(GameObject other)
    {
        if (!enabled || ingredientData == null) return;
        if (!canPour) return; // encore en cooldown

        if (other.CompareTag("Cauldron"))
        {
            // Trouver le chaudron associé
            if (currentCauldron == null)
                currentCauldron = other.GetComponent<Cauldron>();

            if (currentCauldron == null)
                return;

            // Incrémente le compteur à chaque particule touchée
            particleHitCount++;

            // Debug facultatif
             Debug.Log($"💧 Particule touchée ! Compteur : {particleHitCount}/{particlesNeededForValidation}");

            // Si on atteint le nombre requis, on valide le versement
            if (particleHitCount >= particlesNeededForValidation)
            {
                ValidatePour();
            }
        }
    }

    private void ValidatePour()
    {
        if (currentCauldron == null) return;

        Debug.Log($"✅ Versement validé : {ingredientData.ingredientName}");

        // Ajout unique de l’ingrédient au chaudron
        currentCauldron.AddIngredient(ingredientData);

        //if (pourSuccessSound != null)
        //    pourSuccessSound.Play();

        if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(3);    // Success Notification

        // Lancer le cooldown avant qu’un nouveau versement soit possible
        StartCoroutine(PourCooldown());
    }

    private IEnumerator PourCooldown()
    {
        Debug.Log("Starting reinit before next pour.");
        canPour = false;
        particleHitCount = 0;
        yield return new WaitForSeconds(cooldownBetweenPours);
        canPour = true;
        currentCauldron = null;
        Debug.Log("End : pour available again.");
    }
}
