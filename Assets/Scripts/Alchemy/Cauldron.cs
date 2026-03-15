using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Cauldron : MonoBehaviour
{
    public IngredientData ingredientToAdd;
    
    [Header("Paramètres du chaudron")]
    public int maxIngredients = 3;  // Nbre d'ingrédients par défaut (jamais moins de 3, et jusqu'à 5 potentiellement)
    public List<IngredientData> addedIngredients = new();   // Gérer les ingrédients ajoutés au fur et à mesure
    public float resetTime = 30f;   // Temps avant reset du contenu du chaudron (entre 30 et 60 sec ?)

    [Header("Références externes")]
    //public List<RecipeData> allRecipes;     // Toutes les recettes du jeu. Certaines connues au départ, d'autres découvertes par expérimentation ou par récompense
    public StirringManager stirringManager; // Gestion des effets de la louche pour remuer le mélange dans le chaudron

    [Header("UI")]
    public List<Image> ingredientSlots;     // UI Ingrédients présents dans le chaudron avant remuage
    public Image recipePreviewIcon;         // UI Recette potentielle détectée
    public TextMeshProUGUI recipeNameText;    // UI Nom de la recette
    public TextMeshProUGUI newRecipeText;     // UI Info nouvelle recette découverte

    [Header("Effets visuels et audio")]
    public Renderer liquidRenderer; // Surface du liquide (MeshRenderer)
    public AudioSource successSound; // Audio pour notification d'un succès
    public ParticleSystem successParticles; // UI : ParticleSystem pour notifier le joueur
    public float colorChangeSpeed = 2f; // Vitesse de transition entre couleur de base et nouvelle couleur issue de la recette
    public ParticleSystem bubbles; //Les bulles de la potion
    //public ParticleSystem.ColorOverLifetimeModule colorModule; //Module pour accéder à la couleur du particles system

    [Header("Effets visuels")]
    public GameObject resetEffectPrefab;   // BlueSwirl Effect
    public Transform effectSpawnPoint;     // Point de spawn de l'effet ci dessus

    [Header("Audio")]
    public AudioSource resetSound;  // Audio pour notification Reset du contenu du chaudron

    private bool canStir = false;   // Permet de "touiller" ; passe a true si un nombre minimal d'ingrédients est atteint (3, en général)
    private bool recipeCompleted = false;   // Valide la réalisation d'une potion
    private RecipeData currentRecipe = null;    // Infos liées à la recette en cours
    private Color initialLiquidColor;   // Couleur de base du contenu du chaudron
    private List<GameObject> activeIngredientFeedbacks = new(); // Feedbacks visuels actuellement actifs dans le chaudron
    private void Start()
    {
        // Affichage des slots "ingrédients" sur la cheminée, au démarrage (vide)
        foreach (var slot in ingredientSlots)
            if (slot != null)
                slot.sprite = null;

        if (stirringManager != null)
            stirringManager.enabled = false;

        // Récupération de la couleur du contenu du chaudron
        if (liquidRenderer != null)
            initialLiquidColor = liquidRenderer.material.GetColor("_PotionColor");

        UpdateRecipeUI(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EmptyBottle") && recipeCompleted && currentRecipe != null)   
        {
            Debug.Log($"[TRIGGER] -> {other.name} (tag: {other.tag})");
            PotionBottle bottle = other.GetComponent<PotionBottle>();
            if (bottle != null)
            {
                bottle.FillWithRecipe(currentRecipe);
                Debug.Log($"Potion {currentRecipe.recipeName} transférée dans la fiole !");
            }
            else
            {
                Debug.LogWarning("L’objet 'EmptyBottle' n’a pas de PotionBottle !");
            }
        }

        // Autorise toujours la réinitialisation
        if (other.CompareTag("ResetCauldron"))
        {
            Debug.Log("Ingrédient de nettoyage du chaudron détecté !");
            ResetCauldron();
            Destroy(other.gameObject);
            return;
        }

        // Empêche les ingrédients de s’ajouter si recette terminée
        if (recipeCompleted) return;

        if (addedIngredients.Count >= maxIngredients) return;

        if (other.CompareTag("Ingredient"))
        {
            IngredientBehaviour ingredient = other.GetComponent<IngredientBehaviour>();
            if (ingredient != null && ingredient.data != null)
            {
                AddIngredient(ingredient.data);
                AudioManager.audioInstance.PlayTheGoodSound(3); // Liquid Splash
                Destroy(other.gameObject);
            }
        }
    }

    public void AddIngredient(IngredientData data)
    {
        addedIngredients.Add(data);
        Debug.Log($"Ingrédient ajouté : {data.ingredientName}");
        UpdateIngredientUI();


        var possible = FindMatchingRecipes();
        Debug.Log("possible.Count : " + possible.Count);

        if (possible.Count == 1)
        {
            currentRecipe = possible[0];
            maxIngredients = currentRecipe.requiredIngredients.Length;
            Debug.Log($"Recette détectée : {currentRecipe.recipeName}");
            UpdateRecipeUI(currentRecipe);
        }
        else if (possible.Count > 1)
        {
            Debug.Log($"Plusieurs recettes encore possibles ({possible.Count})");
        }
        else if (addedIngredients.Count >= maxIngredients)
        {
            Debug.LogWarning("Mauvais mélange détecté ! Nettoyage du chaudron activé !");
            StartCoroutine(HandleFailedMix());
        }
        else
        {
            Debug.Log("Aucune recette valide avec ces ingrédients !");
            UpdateRecipeUI(null);
        }

        if(addedIngredients.Count == 1)
        {
            StartCoroutine(ChangeLiquidColor(data.colorHint));
        }
        else
        {
            Color mixedColor = Color.Lerp(liquidRenderer.material.GetColor("_PotionColor"), data.colorHint, 0.5f);
            StartCoroutine(ChangeLiquidColor(mixedColor));     
        }

        if (currentRecipe != null && addedIngredients.Count >= currentRecipe.requiredIngredients.Length)
        {
            // Vérifie que les ingrédients correspondent EXACTEMENT à la recette
            bool exactMatch = !currentRecipe.requiredIngredients.Except(addedIngredients).Any() &&
                              !addedIngredients.Except(currentRecipe.requiredIngredients).Any();

            if (exactMatch)
            {
                canStir = true;
                EnableStirring(true);
                Debug.Log("Tous les bons ingrédients sont ajoutés ! Vous pouvez remuer !");
            }
            else
            {
                Debug.LogWarning("La recette contient des ingrédients erronés !");
                StartCoroutine(HandleFailedMix());
            }
        }

        // Feedback visuel de l’ingrédient
        SpawnIngredientFeedback(data);
    }

    // Feedback visuel de l’ingrédient
    private void SpawnIngredientFeedback(IngredientData data)
    {
        if (data.ingredientFeedback == null)
            return;

        // Décommenter pour éviter que tous les ingrédients flottent exactement au même endroit :
        //Vector3 randomOffset = new Vector3(
        //    Random.Range(-0.05f, 0.05f),
        //    0f,
        //    Random.Range(-0.05f, 0.05f)
        //    );

        // Position d’apparition : surface du chaudron
        Vector3 spawnPos = liquidRenderer != null
            ? liquidRenderer.transform.position + Vector3.up * 0.02f // + randomOffset
            : transform.position;

        GameObject feedback = Instantiate(
            data.ingredientFeedback,
            /*spawnPos,
            Quaternion.identity,*/
            transform   // parenté au chaudron
        );

        activeIngredientFeedbacks.Add(feedback);
    }

    private List<RecipeData> FindMatchingRecipes()
    {
        return RecipeManager.RecipeInstance.allRecipes
            .Where(r => addedIngredients.All(i => r.requiredIngredients.Contains(i)))
            .ToList();
    }

    private void UpdateIngredientUI()
    {
        for (int i = 0; i < ingredientSlots.Count; i++)
        {
            if (ingredientSlots[i].sprite == null)
            {
                ingredientSlots[i].sprite = addedIngredients[addedIngredients.Count - 1].icon;
                ingredientSlots[i].color = Color.white;
                return;
            }
        }
    }

    private void UpdateRecipeUI(RecipeData recipe)
    {
        if (recipePreviewIcon != null)
            recipePreviewIcon.sprite = recipe ? recipe.icon : null;

        if (recipeNameText != null)
            recipeNameText.text = recipe ? recipe.recipeName : "Aucune recette détectée";

        if (newRecipeText != null) newRecipeText.text = "";
    }

    private void Update()
    {        
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {       
           // StartCoroutine(ChangeLiquidColor(Color.chocolate));
            AddIngredient(ingredientToAdd);
            Debug.Log("Enter pressed");
        }
        if (!canStir || stirringManager == null || recipeCompleted) return;

        if (stirringManager.isWellStirred)
        {
            FinishRecipe();
        }
    }

    private void EnableStirring(bool enable)
    {
        if (stirringManager != null)
        {
            stirringManager.enabled = enable;
            if (enable)
                stirringManager.ResetStirringValues();
        }
    }

    private void FinishRecipe()
    {
        recipeCompleted = true;
        canStir = false;
        EnableStirring(false);

        Debug.Log($"Recette '{currentRecipe?.recipeName}' terminée !");
        if (currentRecipe != null)
        {
            // Changement progressif de couleur
            if (liquidRenderer != null)
                StartCoroutine(ChangeLiquidColor(currentRecipe.potionColor));

            // Effets sensoriels
            if (successParticles != null)
                successParticles.Play();

            //if (successSound != null)
            //    successSound.Play();

            if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayNotificationSound(3);    // Success Notification

            // NB : si l'option création directe de l'objet, décommenter cette partie (utile pour les tests rapides)
            // Création du résultat
            //if (currentRecipe.resultPrefab != null)
            //{
            //    Instantiate(currentRecipe.resultPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            //}
        }

        // Ajout automatique si nouvelle recette découverte
        //RecipeManager.Instance?.DiscoverRecipe(currentRecipe);

        StartCoroutine(AutoResetAfterDelay(resetTime)); // reset après le temps défini dans l'inspector
    }

    private IEnumerator ChangeLiquidColor(Color targetColor)
    {
        if (liquidRenderer == null || bubbles == null)
            yield break;

        Material mat = liquidRenderer.material;

        // Shader custom → on lit la bonne propriété
        Color startColor = mat.GetColor("_PotionColor");

        // Récupération DU MODULE depuis le ParticleSystem
        var colorOverLifetime = bubbles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * colorChangeSpeed;

            Color currentColor = Color.Lerp(startColor, targetColor, t);

            // 🔮 Liquide
            mat.SetColor("_PotionColor", currentColor);

            // 🫧 Particules
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(currentColor, 0f),
                new GradientColorKey(currentColor, 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
                }
            );

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            yield return null;
        }
    }

    public void ResetCauldron()
    {
        Debug.Log("Réinitialisation du contenu du chaudron en cours.");

        // Effet visuel de purification
        if (resetEffectPrefab != null)
        {
            StartCoroutine(nameof(ResetCauldronVFX), 1f);
        }

        // 1️⃣ Ingrédients et recette
        addedIngredients.Clear();
        currentRecipe = null;
        recipeCompleted = false;
        canStir = false;
        maxIngredients = 3;

        // 2️⃣ UI
        foreach (var slot in ingredientSlots)
        {
            if (slot != null)
            {
                slot.sprite = null;
                slot.color = new Color(1, 1, 1, 0.01f);
            }
        }
        UpdateRecipeUI(null);

        // 3️⃣ Liquide
        if (liquidRenderer != null)
        {
            StopAllCoroutines();
            liquidRenderer.material.SetColor("_PotionColor", initialLiquidColor);
            var colorOverLifetime = bubbles.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(initialLiquidColor, 0f),
                new GradientColorKey(initialLiquidColor, 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
                }
            );

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        // 4️⃣ Particules / sons
        if (successParticles != null) successParticles.Stop();
        if (successSound != null && successSound.isPlaying) successSound.Stop();

        // 5️⃣ StirringManager
        if (stirringManager != null)
        {
            stirringManager.ResetStirringValues();
            stirringManager.enabled = false;
        }

        if (resetSound != null)
            resetSound.Play();

        // 6️⃣ Feedbacks visuels ingrédients
        foreach (var feedback in activeIngredientFeedbacks)
        {
            if (feedback != null)
                Destroy(feedback);
        }

        activeIngredientFeedbacks.Clear();

        Debug.Log("Chaudron prêt pour une nouvelle recette !");
    }

    private IEnumerator AutoResetAfterDelay(float delay)
    {
        Debug.Log("Auto Reset Start.");
        yield return new WaitForSeconds(delay);
        ResetCauldron();
    }

    private IEnumerator ResetCauldronVFX(float time)
    {
        Debug.Log("Ré initialisation du VFX du ResetCauldron.");

        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;

        GameObject vfxObject = Instantiate(resetEffectPrefab, spawnPos, Quaternion.identity);

        ParticleSystem vfx = vfxObject.GetComponentInChildren<ParticleSystem>();

        if (vfx == null)
        {
            Debug.LogWarning("Le prefab ResetEffect n'a pas de ParticleSystem !");
            yield break;
        }

        Debug.Log("VFX : " + vfx.name);
        vfx.Play();
        yield return new WaitForSeconds(time);
    }

    private IEnumerator HandleFailedMix()
    {
        Debug.Log("Mauvaise combinaison d'ingrédients !");

        // Effets visuels/sonores d'échec
        if (liquidRenderer != null)
        {
            Material mat = liquidRenderer.material;
            Color failColor = Color.magenta; // NB : définir une couleur “erreur” spécifique
            mat.color = failColor;
        }

        // (optionnel) Notification : audio d'échec
        if (resetSound != null)
            resetSound.Play();

        // (optionnel) Notifiation : particules de fumée
        if (successParticles != null)
        {
            var main = successParticles.main;
            main.startColor = Color.gray;
            successParticles.Play();
        }

        // délai avant reset
        yield return new WaitForSeconds(2f); 

        ResetCauldron();
    }
}