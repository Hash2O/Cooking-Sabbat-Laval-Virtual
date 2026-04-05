using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRDrawerRail : MonoBehaviour
{
    [Header("Paramètres du Tiroir")]
    public float maxOpenDistance = 0.3f;
    public Vector3 openAxis = new Vector3(-1, 0, 0);

    private Transform parentRef;
    private Vector3 startLocalPos;
    private Quaternion startLocalRot;
    private Rigidbody rb;

    // Variables pour la connexion à la main VR
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Transform interactorTransform;
    private float grabOffsetDistance;

    void Start()
    {
        parentRef = transform.parent;
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
        rb = GetComponent<Rigidbody>();

        // On récupère le système XR
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // On dit au script d'écouter quand la main attrape et quand elle lâche
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // La main vient d'attraper la poignée
        interactorTransform = args.interactorObject.transform;

        // Petite sécurité mathématique : on calcule la distance entre ta main et le centre du tiroir.
        // Ça évite que le tiroir ne se téléporte brutalement dans ta paume à la milliseconde où tu cliques !
        Vector3 handLocal = parentRef.InverseTransformPoint(interactorTransform.position);
        Vector3 drawerLocal = parentRef.InverseTransformPoint(transform.position);
        grabOffsetDistance = Vector3.Dot(handLocal - drawerLocal, openAxis.normalized);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Le joueur a lâché la poignée
        interactorTransform = null;
    }

    void FixedUpdate() 
    {
        if (parentRef == null) return;

        Vector3 targetPosition = transform.position;

        // Si la main tient le tiroir, on calcule le mouvement
        if (interactorTransform != null)
        {
            // Où est la main par rapport au point zéro du meuble ?
            Vector3 handLocalPos = parentRef.InverseTransformPoint(interactorTransform.position);
            Vector3 handOffsetFromStart = handLocalPos - startLocalPos;

            // On projette l'effort du joueur uniquement sur l'axe d'ouverture (-1, 0, 0)
            float distance = Vector3.Dot(handOffsetFromStart, openAxis.normalized) - grabOffsetDistance;

            // On bloque le tiroir entre 0 (fermé) et sa limite
            distance = Mathf.Clamp(distance, 0f, maxOpenDistance);

            // On détermine les vraies coordonnées exactes
            targetPosition = parentRef.TransformPoint(startLocalPos + (openAxis.normalized * distance));
        }

        Quaternion targetRotation = parentRef.rotation * startLocalRot;

        // On déplace le tiroir ET son contenu physique
        if (rb != null && rb.isKinematic)
        {
            rb.MovePosition(targetPosition);
            rb.MoveRotation(targetRotation);
        }
        else
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }
    }
}