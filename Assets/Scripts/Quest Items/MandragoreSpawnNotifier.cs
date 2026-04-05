using UnityEngine;

public class MandragoreSpawnNotifier : MonoBehaviour
{
    public MandragorePuzzle puzzleManager;

    void OnEnable()
    {
        // Dès que cette fausse mandragore apparait, on prévient le Juge
        if (puzzleManager != null)
        {
            puzzleManager.CheckPuzzleCompletion();
        }
    }
}