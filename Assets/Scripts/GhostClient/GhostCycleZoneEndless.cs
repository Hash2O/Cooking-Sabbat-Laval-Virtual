using UnityEngine;

public class GhostCycleZoneEndless : MonoBehaviour
{
    [SerializeField] private GhostCycleManagerEndless cycleManager;

    private void Awake()
    {
        if (cycleManager == null)
            cycleManager = FindFirstObjectByType<GhostCycleManagerEndless>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cycleManager.isPlayerInside = true;
            cycleManager.isPaused = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cycleManager.isPlayerInside = false;
            cycleManager.isPaused = true;
        }
    }

}

