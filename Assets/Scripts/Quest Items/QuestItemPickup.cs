using UnityEngine;

public class QuestItemPickup : MonoBehaviour
{
    public string itemID;

    public void Collect()
    {
        ExplorationProgressManager.ExplorationInstance.HasCollectedItemForToday();
        ExplorationProgressManager.ExplorationInstance.CollectQuestItem(itemID);
        Destroy(gameObject);
    }
}
