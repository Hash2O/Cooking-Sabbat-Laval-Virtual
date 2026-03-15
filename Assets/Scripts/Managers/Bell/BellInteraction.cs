using UnityEngine;

public class BellInteraction : MonoBehaviour
{
    private bool isEnabled = true;

    public void SetEnabled(bool value)
    {
        isEnabled = value;
    }

    public void RingBell()
    {
        if (!isEnabled)
        {
            Debug.Log("Clochette désactivée");
            return;
        }

        GameCycleManager.GameCycleInstance.StartNight();
    }
}
