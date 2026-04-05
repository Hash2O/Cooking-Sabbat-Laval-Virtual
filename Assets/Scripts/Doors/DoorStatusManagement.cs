using UnityEngine;

public class DoorStatusManagement : MonoBehaviour
{
    public string doorID;

    private void Start()
    {
        if (ExplorationProgressManager.ExplorationInstance.IsDoorUnlocked(doorID))
        {
            UnlockDoorVisual();
        }
    }

    public void TryUnlock()
    {
        ExplorationProgressManager.ExplorationInstance.UnlockDoor(doorID);
        UnlockDoorVisual();
    }

    public void UnlockDoorVisual()
    {
        // animation / collider / poign�e
        Debug.Log("Porte ouverte !");
    }
}
