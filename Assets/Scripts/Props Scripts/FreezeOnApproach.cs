using UnityEngine;

public class FreezeOnApproach : MonoBehaviour
{
    public RoomHintManager hintManager;

    private void OnTriggerEnter(Collider other)
    {
        // Si la main (ou le corps) du joueur s'approche, on coupe l'animation !
        if (other.CompareTag("Player"))
        {
            if (hintManager != null)
            {
                hintManager.InterruptHint();
            }
        }
    }
}