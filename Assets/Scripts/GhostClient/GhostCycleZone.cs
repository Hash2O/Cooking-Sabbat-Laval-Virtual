using UnityEngine;

public class GhostCycleZone : MonoBehaviour
{
    [SerializeField] private GhostCycleManager cycleManager;

    private void Awake()
    {
        if (cycleManager == null)
            cycleManager = FindFirstObjectByType<GhostCycleManager>();
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

