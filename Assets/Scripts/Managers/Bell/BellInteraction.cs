using UnityEngine;

public class BellInteraction : MonoBehaviour
{
    [SerializeField] private BellManager bellManager;
    private bool isEnabled = true;

    private void Awake()
    {
        bellManager = GetComponent<BellManager>();
    }

    public void SetEnabled(bool value)
    {
        isEnabled = value;
    }

    public void TryActivateBell()
    {
        if (!ExplorationProgressManager.ExplorationInstance.HasCollectedItemForToday())
        {
            Debug.Log("Objet de quête manquant !");
            return;
        }

        RingBell();
    }

    public void RingBell()
    {
        if (!isEnabled)
        {
            Debug.Log("Clochette désactivée");
            return;
        }
        bellManager.isBellActivated = true;
        GameCycleManager.GameCycleInstance.StartNight();
        Debug.Log("Les clients fantômes sont invités à venir passer commande !");
    }
}
