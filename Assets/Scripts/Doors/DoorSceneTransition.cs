using UnityEngine;

public class DoorSceneTransition : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private HingeJoint doorHinge;

    [Header("Changement de scène")]
    [SerializeField] private int nextSceneIndex = 1;  // Index de la scène suivante dans Build Settings
    [SerializeField] private float openAngleThreshold = 45f;  // Angle pour déclencher (degrés)

    [Header("Options")]
    [SerializeField] private bool oneTimeOnly = true;  // Une seule fois par porte
    [SerializeField] private float debounceTime = 1f;  // Délai anti-retrigger (secondes)

    private bool sceneTriggered = false;
    private float lastTriggerTime = 0f;

    private void Awake()
    {
        if (doorHinge == null)
            doorHinge = GetComponent<HingeJoint>();
    }

    private void FixedUpdate()
    {
        if (sceneTriggered || doorHinge == null)
            return;

        float currentAngle = Mathf.Abs(doorHinge.angle);  // angle en degrés

        if (currentAngle >= openAngleThreshold &&
            Time.time >= lastTriggerTime + debounceTime)
        {
            TriggerSceneChange();
        }
    }

    private void TriggerSceneChange()
    {
        sceneTriggered = true;
        lastTriggerTime = Time.time;

        Debug.Log($"Porte ouverte à {doorHinge.angle:F1}° → Chargement scène {nextSceneIndex}");

        if (GameManager.GMInstance != null)
        {
            GameManager.GMInstance.LoadSceneByIndex(nextSceneIndex);
        }
        else
        {
            Debug.LogError("GameManager.Instance non trouvé !");
        }
    }
}

