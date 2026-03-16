//using UnityEngine;

//[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alchemy/Recipe")]
//public class RecipeData : ScriptableObject
//{
//    [Header("Informations de recette")]
//    public string recipeName;
//    [TextArea] public string description;
//    public Sprite icon;

//    [Header("Ingrédients requis")]
//    public IngredientData[] requiredIngredients; // entre 3 et 5

//    [Header("Résultat final")]
//    // Option à activer si on veut créer directement l'objet contenant la potion 
//    //public GameObject resultPrefab; 

//    // Couleur de l'échec de la concoction (soit la recette n'existe pas, soit les ingrédients ne sont pas les bons)
//    public Color potionColor = Color.magenta;

//    // Bouteille qui contiendra la potion finale 
//    public GameObject bottlePrefab;

//    // Option : Pour gérer les changements visuels du client fantôme
//    // public Material potionMaterial;
//}

using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alchemy/Recipe")]
public class RecipeData : ScriptableObject
{
    //[Header("Informations de recette")]
    //public string recipeName;
    //[TextArea] public string description;
    //public Sprite icon;

    //[Header("Ingrédients requis")]
    //public IngredientData[] requiredIngredients; // entre 3 et 5

    //[Header("Résultat final")]
    //// Option à activer si on veut créer directement l'objet contenant la potion 
    ////public GameObject resultPrefab; 

    //// Couleur de l'échec de la concoction (soit la recette n'existe pas, soit les ingrédients ne sont pas les bons)
    //public Color potionColor = Color.magenta;

    //// Bouteille qui contiendra la potion finale 
    //public GameObject bottlePrefab;

    //// Option : Pour gérer les changements visuels du client fantôme
    //// public Material potionMaterial;

    [Header("Informations de recette")]
    public string recipeName;
    [TextArea] public string description;
    public Sprite icon;

    // Option à activer si on veut créer directement l'objet contenant la potion 
    public Color potionColor = Color.magenta;

    // Bouteille qui contiendra la potion finale
    public GameObject bottlePrefab;

    [Header("Grimoire")]
    public Material pageMaterial;

    [Header("Ingrédients")]
    public IngredientData[] requiredIngredients;    // entre 3 et 5

    [Header("Client fantôme associé")]
    public GameObject ghostPrefab;

    [Header("Découverte")]
    public bool isKnownAtStart = false;
}


