using UnityEngine;
using TMPro;

public class StirringCauldron : MonoBehaviour
{
    [Header("Références")]
    public Transform cauldronCenter;     // Centre du bol (un Empty placé au milieu)
    public Transform cookingSpoon;          // La cuillère tenue par le joueur
    public ParticleSystem stirEffect; // Optionnel : effet visuel quand ça mélange

    [Header("Paramètres")]
    public float stirProgress;   // Progression du mélange [0..1]
    public float requiredProgress; // Seuil pour finir le mélange
    public float stirMultiplier; // Vitesse de progression (ajustable)

    public TextMeshProUGUI spoonText;
    public TextMeshProUGUI stirProgressText;
    public TextMeshProUGUI isWellStirredText;
    public TextMeshProUGUI resultText;

    [HideInInspector]
    public bool isWellStirred;

    private BasicRecipeManager BasicRecipeManager;
    private bool isInBowl = false;
    private Vector3 lastSpoonPos;

    private void Start()
    {
        BasicRecipeManager = GetComponent<BasicRecipeManager>();
        ResetStirringValues();
        spoonText.text = "Spoon : false";
        stirProgressText.text = "Stir Progress : 0";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == cookingSpoon)
        {
            isInBowl = true;
            lastSpoonPos = cookingSpoon.position;
            spoonText.text = "Spoon : true";
            Debug.Log("Cuillère dans le bol !");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == cookingSpoon)
        {
            isInBowl = false;
            spoonText.text = "Spoon : false";
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
           stirProgressText.text = "Stir Progress : " + stirProgress.ToString();

           // Feedback visuel : lancer des particules si présentes
           if (stirEffect != null && !stirEffect.isPlaying && isInBowl)
                stirEffect.Play();

           // Vérification si terminé
           if (stirProgress >= requiredProgress)
           {
                stirProgress = requiredProgress;
                resultText.text = "Potion is ready !";
                Debug.Log("Mélange terminé !");
                if (stirEffect != null) stirEffect.Stop();
                // Ici tu pourrais déclencher : changement d'état des ingrédients, son, animation, etc.
                isWellStirred = true;
                isWellStirredText.text = "Stir Status : Stirring is done !";
           }

           lastSpoonPos = cookingSpoon.position;
    }

    public void ResetStirringValues()
    {
        BasicRecipeManager.ResetRequiredIngredientNumberValues();
        stirProgress = 0f;
        stirProgressText.text = "Stir Progress : 0";
        requiredProgress = 5f;
        stirMultiplier = 100f;
        isWellStirred = false;
        isWellStirredText.text = "Stir Status : Stir is needed";
    }
}
