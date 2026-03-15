using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DoorLockWithKey : MonoBehaviour
{
    [Header("R�f�rences")]
    [SerializeField] private XRSocketInteractor keySocket;
    [SerializeField] private HingeJoint doorHinge;              // HingeJoint sur la partie mobile de la porte
    [SerializeField] private XRBaseInteractable doorHandle;      // XR Grab de la poign�e (optionnel)

    [Header("Cl� requise")]
    [SerializeField] private string requiredKeyTag = "";  // Tag de la bonne cl�

    [Header("Options")]
    [SerializeField] private bool lockHandleWhenLocked = false;  // D�sactiver la poign�e quand c'est verrouill�

    private bool isUnlocked = false;
    private XRGrabInteractable currentKeyGrab;
    private Rigidbody currentKeyRb;

    // Pour verrouiller via les limites
    private JointLimits lockedLimits;
    private JointLimits unlockedLimits;
    private bool hingeConfigInitialized = false;

    // Pour restaurer le spring d'origine
    private bool hingeInitialUseSpring;
    private JointSpring hingeInitialSpring;

    private void Awake()
    {
        if (!keySocket)
            keySocket = GetComponent<XRSocketInteractor>();

        if (doorHinge != null)
        {
            // Sauvegarde configuration initiale du HingeJoint
            unlockedLimits = doorHinge.limits;
            lockedLimits = doorHinge.limits;
            lockedLimits.min = -0.1f;
            lockedLimits.max = 0.1f;
            hingeConfigInitialized = true;

            hingeInitialUseSpring = doorHinge.useSpring;
            hingeInitialSpring = doorHinge.spring;
        }
    }

    private void OnEnable()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnKeyInserted);
            keySocket.selectExited.AddListener(OnKeyRemoved);
        }

        LockDoor();
    }

    private void OnDisable()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.RemoveListener(OnKeyInserted);
            keySocket.selectExited.RemoveListener(OnKeyRemoved);
        }
    }

    private void LockDoor()
    {
        isUnlocked = false;

        if (doorHinge != null && hingeConfigInitialized)
        {
            // Porte verrouill�e : limites ultra serr�es autour de 0�
            doorHinge.limits = lockedLimits;
            doorHinge.useLimits = true;

            // On peut �ventuellement d�sactiver le spring pour qu'elle reste bien fixe
            doorHinge.useSpring = false;
        }

        if (lockHandleWhenLocked && doorHandle != null)
        {
            doorHandle.enabled = false;
        }
    }

    private void UnlockDoor()
    {
        isUnlocked = true;

        if (doorHinge != null && hingeConfigInitialized)
        {
            // Restaure les limites normales
            doorHinge.limits = unlockedLimits;
            doorHinge.useLimits = true;

            // Restaure le spring d'origine si tu l'utilises pour la fermeture auto
            doorHinge.useSpring = hingeInitialUseSpring;
            if (hingeInitialUseSpring)
                doorHinge.spring = hingeInitialSpring;
        }

        if (doorHandle != null)
        {
            doorHandle.enabled = true;
        }
    }

    private void OnKeyInserted(SelectEnterEventArgs args)
    {
        if (isUnlocked)
            return; // d�j� d�verrouill�e

        var keyObject = args.interactableObject.transform.gameObject;

        // V�rifie que c'est bien la bonne cl� via le tag
        if (!keyObject.CompareTag(requiredKeyTag))
        {
            // Mauvaise cl� : pas de d�verrouillage
            return;
        }

        // Bonne cl� ins�r�e : on la fige et on d�verrouille
        currentKeyGrab = keyObject.GetComponent<XRGrabInteractable>();
        currentKeyRb = keyObject.GetComponent<Rigidbody>();

        if (currentKeyGrab != null)
        {
            // Emp�che toute nouvelle prise
            currentKeyGrab.enabled = false;
        }

        if (currentKeyRb != null)
        {
            currentKeyRb.isKinematic = true;
            currentKeyRb.linearVelocity = Vector3.zero;
            currentKeyRb.angularVelocity = Vector3.zero;
        }

        UnlockDoor();
    }

    private void OnKeyRemoved(SelectExitEventArgs args)
    {
        // Si tu ne veux plus permettre de retirer la cl� (cas le plus courant ici),
        // cet event ne devrait pas se produire, car on a d�sactiv� le Grab et mis la cl� en kinematic.
        // Tu peux laisser cette m�thode vide ou y mettre une logique sp�ciale si besoin.
    }
}
