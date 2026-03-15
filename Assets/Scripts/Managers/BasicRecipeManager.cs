using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BasicRecipeManager : MonoBehaviour
{
    public int colorIndex;

    [SerializeField] private TextMeshProUGUI numberOfIngredientsText;
    [SerializeField] private TextMeshProUGUI colorIndexText;
    [SerializeField] private TextMeshProUGUI requiredNumberText;

    [SerializeField] private int numberOfIngredients;
    [SerializeField] private List<Material> waterMaterials = new();
    [SerializeField] private Material waterMaterial;

    [SerializeField] private MeshRenderer cookingPotWaterRenderer;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip potionReadyAudioclip;

    private StirringCauldron StirringCauldron;
    private bool hasRequiredIngredientNumber = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StirringCauldron = GetComponent<StirringCauldron>();

        numberOfIngredientsText.text = "Number of ingredients : 0";
        colorIndexText.text = "Color Index : 0";
        requiredNumberText.text = "Required Number : False" ;
        numberOfIngredients = 0;
        cookingPotWaterRenderer.material = waterMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasRequiredIngredientNumber && StirringCauldron.isWellStirred)
        {
            // Reset to allow new potion to be made
            StirringCauldron.ResetStirringValues();
            MakePotion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ingredient"))
        {
            // After the release of a potion, putting a new ingredient re init the actual brew from the cooking pot
            if(numberOfIngredients == 0 && cookingPotWaterRenderer.material != waterMaterial)
            {
                cookingPotWaterRenderer.material = waterMaterial;
            }

            numberOfIngredients++;
            numberOfIngredientsText.text = "Number of ingredients : " + numberOfIngredients;
            
            if(numberOfIngredients == 3)
            {
                hasRequiredIngredientNumber = true;
                requiredNumberText.text = "Required Number : True";
            }
        }
    }

    public void ResetRequiredIngredientNumberValues()
    {
        hasRequiredIngredientNumber = false;
        requiredNumberText.text = "Required Number : False";
        numberOfIngredients = 0;
        numberOfIngredientsText.text = "Number of ingredients : " + numberOfIngredients;
    }

    void MakePotion()
    {
        colorIndex = Random.Range(0, waterMaterials.Count);
        colorIndexText.text = "Color Index : " + colorIndex.ToString();
        cookingPotWaterRenderer.material = waterMaterials[colorIndex];
        audioSource.PlayOneShot(potionReadyAudioclip);
    }
}
