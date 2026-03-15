using System.Collections;
using UnityEngine;

public class PotionBottle : MonoBehaviour
{
    [Header("Données de la potion")]
    public RecipeData containedRecipe;
    public int minPotionEffectiveTime = 10;
    public int maxPotionEffectiveTime = 20;

    [Header("Aspect visuel")]
    public Renderer liquidRenderer;

    private RecipeData originalRecipe;
    private int potionEffectiveTime;

    private void Start()
    {
        originalRecipe = containedRecipe;
    }


    public void FillWithRecipe(RecipeData recipe)
    {
        containedRecipe = recipe;

        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = recipe.potionColor;
        }

        Debug.Log($" La bouteille contient maintenant la potion : {recipe.recipeName}");

        potionEffectiveTime = Random.Range(minPotionEffectiveTime, maxPotionEffectiveTime + 1);

        StartCoroutine(nameof(EffectiveTimeOfTheNewPotion), potionEffectiveTime);

        Debug.Log(" Elle restera efficace pendant " + potionEffectiveTime + " secondes seulement !" );
    }

    public RecipeData GetContainedRecipe()
    {
        return containedRecipe;
    }

    // 
    IEnumerator EffectiveTimeOfTheNewPotion(int time)
    {
        yield return new WaitForSeconds(time);
        containedRecipe = originalRecipe;
        liquidRenderer.material.color = originalRecipe.potionColor;
    }
}

