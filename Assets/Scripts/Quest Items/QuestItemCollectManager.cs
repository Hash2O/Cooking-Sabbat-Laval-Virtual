using UnityEngine;

public class QuestItemCollectManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("QuestItem"))
        {
            QuestItemPickup questItem = other.GetComponent<QuestItemPickup>();
            if(questItem != null) questItem.Collect();
        }
    }
}
