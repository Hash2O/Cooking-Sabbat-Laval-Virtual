using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class FillPotionWithCookingPot : MonoBehaviour
{
    private bool isReadyToBeFilled = false;

    [SerializeField] private GameObject content;
    [SerializeField] private ParticleSystem bubbles;

    [SerializeField] private List<Material> colorMaterials;

    private OnTilt onTilt;
    private OnVelocity onVelocity;

    private void Start()
    {
        onTilt = gameObject.GetComponent<OnTilt>();
        onVelocity = gameObject.GetComponent<OnVelocity>();

        bubbles.gameObject.SetActive(false);
        content.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("CookingPot") && !isReadyToBeFilled)
        {
            Debug.Log("Inside cooking pot !");
            isReadyToBeFilled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CookingPot") && isReadyToBeFilled)
        {
            Debug.Log("Outside cooking pot !");
            bubbles.gameObject.SetActive(true);

            MeshRenderer waterCookingPotRenderer = other.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            Debug.Log("Color Water name : " + waterCookingPotRenderer.material.name);

            int index = other.gameObject.transform.GetComponent<BasicRecipeManager>().colorIndex;
            Debug.Log("Index : " + index);

            content.GetComponent<MeshRenderer>().sharedMaterial = colorMaterials[index];
            content.SetActive(true);
        }
    }
}
