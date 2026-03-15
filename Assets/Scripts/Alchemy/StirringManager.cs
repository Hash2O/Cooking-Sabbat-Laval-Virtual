using TMPro;
using UnityEngine;

public class StirringManager : MonoBehaviour
{
    [Header("Références")]
    public Transform cauldronCenter;     // Centre du bol (un Empty placé au milieu)
    public Transform cookingSpoon;          // La cuillère tenue par le joueur
    public ParticleSystem stirEffect; // Optionnel : effet visuel quand ça mélange

    [Header("Paramètres")]
    public float stirProgress;   // Progression du mélange [0..1]
    public float requiredProgress; // Seuil pour finir le mélange
    public float stirMultiplier; // Vitesse de progression (ajustable)

    [HideInInspector]
    public bool isWellStirred;

    public bool isInBowl = false;
    private Vector3 lastSpoonPos;

    public Cauldron linkedCauldron;

    private void Start()
    {
        ResetStirringValues();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == cookingSpoon)
        {
            isInBowl = true;
            lastSpoonPos = cookingSpoon.position;
            Debug.Log("Cuillère dans le bol !");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == cookingSpoon)
        {
            isInBowl = false;
            Debug.Log("Cuillère sortie du bol !");
        }
    }

    void Update()
    {
        if (isInBowl && stirProgress < requiredProgress)
        {
            StirringProcess();
        }
    }

    void StirringProcess()
    {

        Vector3 spoonOffsetNow = cookingSpoon.position - cauldronCenter.position;
        Vector3 spoonOffsetLast = lastSpoonPos - cauldronCenter.position;

        // Produit vectoriel pour estimer la "rotation" autour du centre du bol
        float circularity = Vector3.Cross(spoonOffsetLast, spoonOffsetNow).magnitude;

        // Mise à jour de la progression
        stirProgress += circularity * stirMultiplier * Time.deltaTime;

        // Feedback visuel : lancer des particules si présentes
        if (stirEffect != null && !stirEffect.isPlaying && isInBowl)
        {
            stirEffect.Play();
            AudioManager.audioInstance.PlayTheGoodSound(4);
        }
            

        // Vérification si terminé
        if (stirProgress >= requiredProgress)
        {
            stirProgress = requiredProgress;
            isWellStirred = true;
            if (stirEffect != null) stirEffect.Stop();
            Debug.Log("Mélange terminé !");

            if (linkedCauldron != null)
                linkedCauldron.SendMessage("FinishRecipe", SendMessageOptions.DontRequireReceiver);
        }

        lastSpoonPos = cookingSpoon.position;
    }

    public void ResetStirringValues()
    {
        stirProgress = 0f;
        requiredProgress = 2f;
        stirMultiplier = 150f;
        isWellStirred = false;
    }
}
