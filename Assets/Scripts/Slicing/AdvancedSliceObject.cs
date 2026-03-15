//using UnityEngine;
//using EzySlice;
//using Valve.VR.InteractionSystem;

//public class AdvancedSliceObject : MonoBehaviour
//{
//    public Transform startSlicePoint;
//    public Transform endSlicePoint;
//    public VelocityEstimator velocityEstimator;
//    public Material crossSectionMaterials;
//    public float moreCutForce = 200;
//    public LayerMask sliceableLayer;

//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
//        if (hasHit)
//        {
//            GameObject target = hit.transform.gameObject;
//            // Trying to get the material from the object being sliced
//            //crossSectionMaterials = target.GetComponent<Material>();
//            SliceAndDice(target);
//        }
//    }

//    public void SliceAndDice(GameObject target)
//    {
//        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
//        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
//        planeNormal.Normalize();

//        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

//        if (hull != null)
//        {
//            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterials);
//            SetupSlicedComponents(upperHull);
//            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterials);
//            SetupSlicedComponents(lowerHull);
//            Destroy(target);
//        }
//    }

//    public void SetupSlicedComponents(GameObject slicedObject)
//    {
//        slicedObject.layer = 7;
//        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
//        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
//        IngredientBehaviour ingredient = slicedObject.AddComponent<IngredientBehaviour>();
//        //if (slicedObject.GetComponent<IngredientBehaviour>() != null) ingredient.data = slicedObject.GetComponent<IngredientBehaviour>().data;
//        collider.convex = true;
//        //rb.AddExplosionForce(moreCutForce, slicedObject.transform.position, 1);
//    }
//}

using UnityEngine;
using EzySlice;
using Valve.VR.InteractionSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using NUnit.Framework;
using System.Collections.Generic;

public class AdvancedSliceObject : MonoBehaviour
{
    [Header("Slicing References")]
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public Material crossSectionMaterials;
    public LayerMask sliceableLayer;
    public ParticleSystem slicingVFX;

    [Header("Physics Settings")]
    public float sliceForce = 200f;

    [Header("List of Possible Ingredients Parts")]
    public List<IngredientData> ingredientPartsData;    // Liste des nouveaux ingredients, constituée des moitiés d'ingredients plus grands

    private bool isCutting = false;

    void FixedUpdate()
    {
        if (isCutting) return;

        if (Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer))
        {
            GameObject target = hit.transform.gameObject;
            IngredientBehaviour ingredient = target.GetComponent<IngredientBehaviour>();

            if (ingredient != null) // on ne découpe que les objets reconnus comme ingrédients
            {
                isCutting = true;
                SliceAndDice(target, ingredient);
            }
        }
    }

    private void SliceAndDice(GameObject target, IngredientBehaviour originalIngredient)
    {
        Debug.Log("Target : " + target.name + " Ingredient : " + originalIngredient.name);

        Vector3 velocity = velocityEstimator != null ? velocityEstimator.GetVelocityEstimate() : Vector3.forward;
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity).normalized;

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject partOne = hull.CreateUpperHull(target, crossSectionMaterials);
            SetupSlicedComponents(partOne, originalIngredient);

            GameObject partTwo = hull.CreateLowerHull(target, crossSectionMaterials);
            SetupSlicedComponents(partTwo, originalIngredient);

            // On détruit l'objet original
            Destroy(target);
        }

        if (slicingVFX != null) slicingVFX.Play();

        // petit délai avant de pouvoir recouper autre chose
        Invoke(nameof(ResetCutting), 0.2f);
    }

    private void SetupSlicedComponents(GameObject slicedPart, IngredientBehaviour parentIngredient)
    {
        // Donner un nom au nouvel élément
        slicedPart.name = "Morceau de " + parentIngredient.name;
        // Modifier le tag pour qu'il soit reconnu comme ingredient par le chaudron afin de valider la recette
        slicedPart.tag = "Ingredient";
        // Changer de layer pour éviter que les nouveaux morceaux puissent etre coupés à nouveau
        slicedPart.layer = 0;   // Default layer = 0, Slicable layer = 6

        // Le rendre interagissable
        Rigidbody rb = slicedPart.AddComponent<Rigidbody>();
        MeshCollider collider = slicedPart.AddComponent<MeshCollider>();
        collider.convex = true;

        XRGrabInteractable grabInteractable = slicedPart.AddComponent<XRGrabInteractable>();
        grabInteractable.useDynamicAttach = true;

        // Hériter du comportement d’ingrédient
        IngredientBehaviour newIngredient = slicedPart.AddComponent<IngredientBehaviour>();

        //Hériter des datas du parent
        //if (parentIngredient != null)
        //{
        //    newIngredient.data = parentIngredient.data; // Héritage de la RecipeData
        //}

        if (parentIngredient != null)
        {
            Debug.Log("Changement Ingredient Data");
            switch (parentIngredient.name)
            {
                case "RatSteak":
                    newIngredient.data = ingredientPartsData[0];    // Ingredient Half Steak
                    break;
                case "RatSteak(Clone)":
                    newIngredient.data = ingredientPartsData[0];    // Ingredient Half Steak
                    break;
                default:
                    newIngredient.data = parentIngredient.data;
                    break;
            }
        }

        // Optionnel : légère poussée pour séparer visuellement les morceaux
        //rb.AddExplosionForce(sliceForce, slicedPart.transform.position, 1f);
    }

    private void ResetCutting()
    {
        isCutting = false;
    }
}
