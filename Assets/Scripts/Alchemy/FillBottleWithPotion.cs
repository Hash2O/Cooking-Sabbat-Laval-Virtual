using System.Collections.Generic;
using UnityEngine;

public class FillBottleWithPotion : MonoBehaviour
{
    private bool isReadyToBeFilled = false;

    [SerializeField] private GameObject content;
    [SerializeField] private ParticleSystem bubbles;

    private void Start()
    {
        bubbles.gameObject.SetActive(false);
        content.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cauldron") && !isReadyToBeFilled)
        {
            Debug.Log("Inside the cauldron !");
            isReadyToBeFilled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cauldron") && isReadyToBeFilled)
        {
            Debug.Log("Out of the cauldron !");
            bubbles.gameObject.SetActive(true);

            MeshRenderer cauldronLiquidRenderer = other.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            Debug.Log("Color Water name : " + cauldronLiquidRenderer.material.name);

            content.GetComponent<MeshRenderer>().sharedMaterial = cauldronLiquidRenderer.material;
            content.SetActive(true);
        }
    }
}
