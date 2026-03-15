using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorLockDummyKey : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private HingeJoint doorHinge;
    [SerializeField] private XRBaseInteractable doorHandle;
    [SerializeField] private GameObject dummyKey;

    [Header("Clé requise")]
    [SerializeField] private string requiredKeyTag = "DoorKey";

    [Header("Changement de scène")]
    [SerializeField] private int nextSceneIndex = 1;  // Index de la scène suivante dans Build Settings
    [SerializeField] private float openAngleThreshold = 45f;  // Angle pour déclencher le changement (degrés)

    [Header("Options")]
    [SerializeField] private bool lockHandleWhenLocked = false;
    [SerializeField] private bool oneTimeOnly = true;  // Une seule fois par porte
    [SerializeField] private float debounceTime = 1f;  // Délai anti-retrigger (secondes)

    private bool isUnlocked = false;
    private bool sceneTriggered = false;
    private float lastTriggerTime = 0f;

    // HingeJoint config
    private JointLimits lockedLimits;
    private JointLimits unlockedLimits;
    private bool hingeConfigInitialized = false;
    private bool hingeInitialUseSpring;
    private JointSpring hingeInitialSpring;

    private void Awake()
    {
        if (doorHinge != null)
        {
            unlockedLimits = doorHinge.limits;
            lockedLimits = doorHinge.limits;
            lockedLimits.min = -0.1f;
            lockedLimits.max = 0.1f;
            hingeConfigInitialized = true;

            hingeInitialUseSpring = doorHinge.useSpring;
            hingeInitialSpring = doorHinge.spring;
        }

        if (dummyKey != null)
            dummyKey.SetActive(false);
    }

    private void OnEnable()
    {
        LockDoor();
    }

    private void FixedUpdate()
    {
        // Vérifie si la porte est assez ouverte pour changer de scène
        if (isUnlocked && !sceneTriggered && doorHinge != null)
        {
            float currentAngle = Mathf.Abs(doorHinge.angle);  // angle en degrés

            if (currentAngle >= openAngleThreshold &&
                Time.time >= lastTriggerTime + debounceTime)
            {
                TriggerSceneChange();
            }
        }
    }

    private void LockDoor()
    {
        isUnlocked = false;

        if (doorHinge != null && hingeConfigInitialized)
        {
            doorHinge.limits = lockedLimits;
            doorHinge.useLimits = true;
            doorHinge.useSpring = false;
        }

        if (lockHandleWhenLocked && doorHandle != null)
            doorHandle.enabled = false;
    }

    private void UnlockDoor()
    {
        isUnlocked = true;

        if (doorHinge != null && hingeConfigInitialized)
        {
            doorHinge.limits = unlockedLimits;
            doorHinge.useLimits = true;
            doorHinge.useSpring = hingeInitialUseSpring;
            if (hingeInitialUseSpring)
                doorHinge.spring = hingeInitialSpring;
        }

        if (doorHandle != null)
            doorHandle.enabled = true;
    }

    private void TriggerSceneChange()
    {
        sceneTriggered = true;
        lastTriggerTime = Time.time;

        Debug.Log($"Porte ouverte à {doorHinge.angle:F1}° → Chargement scène {nextSceneIndex}");

        // Appel via ton GameManager singleton
        if (GameManager.GMInstance != null)
        {
            GameManager.GMInstance.LoadSceneByIndex(nextSceneIndex);
        }
        else
        {
            Debug.LogError("GameManager.Instance non trouvé !");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUnlocked)
            return;

        if (!other.CompareTag(requiredKeyTag))
            return;

        var keyGO = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        var grab = keyGO.GetComponent<XRGrabInteractable>();
        var rb = keyGO.GetComponent<Rigidbody>();

        if (grab != null)
            grab.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        keyGO.SetActive(false);

        if (dummyKey != null)
            dummyKey.SetActive(true);

        UnlockDoor();
    }
}
