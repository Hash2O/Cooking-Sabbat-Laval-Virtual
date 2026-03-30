using UnityEngine;

public class ToggleMovementModeManager : MonoBehaviour
{
    [SerializeField] private TeleportationActivator teleportMode;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject continuousMoveMode;
    [SerializeField] private GameObject continuousTurnMode;

    public bool isTeleportActivated = false;

    private void Start()
    {
        teleportMode = rightHand.GetComponentInChildren<TeleportationActivator>();
        teleportMode.enabled = false;
        continuousMoveMode.SetActive(true);
        continuousTurnMode.SetActive(true);
    }

    public void ToggleMovementMode()
    {
        if(isTeleportActivated == false)
        {
            isTeleportActivated = true;
            teleportMode.enabled = true;
            continuousMoveMode.SetActive(false);
            continuousTurnMode.SetActive(false);
        }
        else if(isTeleportActivated == true)
        {
            isTeleportActivated = false;
            teleportMode.enabled = false;
            continuousMoveMode.SetActive(true);
            continuousTurnMode.SetActive(true);
        }
    }
}
