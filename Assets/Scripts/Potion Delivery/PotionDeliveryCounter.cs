using UnityEngine;

public class PotionDeliveryCounter : MonoBehaviour
{
    private PotionBottle currentBottle;

    public PotionBottle GetCurrentBottle()
    {
        return currentBottle;
    }

    private void OnTriggerEnter(Collider other)
    {
        PotionBottle bottle = other.GetComponent<PotionBottle>();

        if (bottle != null)
        {
            currentBottle = bottle;
            Debug.Log("Une potion a été posée sur le comptoir.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PotionBottle bottle = other.GetComponent<PotionBottle>();

        if (bottle != null && bottle == currentBottle)
        {
            currentBottle = null;
            Debug.Log("La potion a été retirée du comptoir.");
        }
    }

    // Optionnel : méthode pour consommer la potion
    public PotionBottle TakeBottle()
    {
        PotionBottle bottle = currentBottle;
        currentBottle = null;
        return bottle;
    }
}

