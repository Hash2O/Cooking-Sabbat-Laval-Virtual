//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class RecipeManager : MonoBehaviour
//{
//    public static RecipeManager Instance { get; private set; }

//    // A garder si on veut déplacer la gestion des recettes depuis le chaudron vers le RecipeManager
//    [Header("Toutes les recettes disponibles dans le jeu")]
//    public List<RecipeData> allRecipes = new List<RecipeData>();

//    [Header("Recettes découvertes par le joueur")]
//    public List<RecipeData> discoveredRecipes = new List<RecipeData>();

//    [SerializeField] private TextMeshProUGUI newRecipeText;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    public bool IsRecipeDiscovered(RecipeData recipe)
//    {
//        return discoveredRecipes.Contains(recipe);
//    }

//    public void DiscoverRecipe(RecipeData recipe)
//    {
//        if (!discoveredRecipes.Contains(recipe))
//        {
//            discoveredRecipes.Add(recipe);
//            Debug.Log($"Nouvelle recette découverte : {recipe.recipeName}");
//            // A faire : notifier ici le grimoire / livre de recettes (UI Canvas)
//            newRecipeText.text = "New Recipe Discovered.";
//        }
//    }

//    public List<RecipeData> GetAvailableRecipes()
//    {
//        return discoveredRecipes;
//    }

//    public void ResetDiscoveredRecipes()
//    {
//        discoveredRecipes.Clear();
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager RecipeInstance { get; private set; }

    [Header("Toutes les recettes du jeu")]
    public List<RecipeData> allRecipes = new();

    //[Header("Recettes connues au démarrage")]
    //public List<RecipeData> startingRecipes = new();

    [Header("Recettes connues et/ou découvertes en jeu (DEBUG / SCORE)")]
    [SerializeField] private List<RecipeData> discoveredRecipes = new();

    private HashSet<RecipeData> knownRecipes = new();

    public Action<RecipeData> OnRecipeDiscovered;

    private void Awake()
    {
        if (RecipeInstance != null && RecipeInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        RecipeInstance = this;
        DontDestroyOnLoad(gameObject);

        InitializeKnownRecipes();
    }

    private void InitializeKnownRecipes()
    {
        knownRecipes.Clear();
        discoveredRecipes.Clear();

        foreach (var recipe in allRecipes)
        {
            if (recipe == null) continue;

            if (recipe.isKnownAtStart)
            {
                knownRecipes.Add(recipe);
                discoveredRecipes.Add(recipe);
            }
        }

        Debug.Log($"📚 Recettes connues au départ : {knownRecipes.Count}");
    }

    public void DiscoverRecipe(RecipeData recipe)
    {
        if (recipe == null) return;
        if (knownRecipes.Contains(recipe)) return;

        knownRecipes.Add(recipe);
        discoveredRecipes.Add(recipe);

        Debug.Log($"📖 Nouvelle recette découverte : {recipe.recipeName}");
        OnRecipeDiscovered?.Invoke(recipe);
    }

    public bool IsRecipeKnown(RecipeData recipe)
    {
        return knownRecipes.Contains(recipe);
    }

    public List<RecipeData> GetKnownRecipes()
    {
        return new List<RecipeData>(knownRecipes);
    }

    public List<RecipeData> GetDiscoveredRecipes()
    {
        return discoveredRecipes;
    }

    public List<GameObject> GetKnownGhostPrefabs()
    {
        return knownRecipes
            .Where(r => r.ghostPrefab != null)
            .Select(r => r.ghostPrefab)
            .ToList();
    }
}