using System.Collections;
using UnityEngine;

public class ItemHint : MonoBehaviour
{
    [Header("Références")]
    public RoomHintManager roomManager;
    public GameObject destinationFX;
    public Transform overrideShakeObject;

    [Header("Sons de Possession (3D)")]
    [Tooltip("L'AudioSource pour le bruit de cahotement/tremblement")]
    public AudioSource rattleSource;
    [Tooltip("L'AudioSource pour le chuchotement fantomatique")]
    public AudioSource whisperSource;

    [Header("Temps")]
    public float timeBeforeDestinationHint = 15f;

    private Coroutine holdCoroutine;
    private Rigidbody rb;
    private bool isPlaced = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public Transform GetTransformToShake()
    {
        // Si overrideShakeObject n'est pas vide, on le renvoie. Sinon, on renvoie l'objet lui-même.
        return overrideShakeObject != null ? overrideShakeObject : transform;
    }
    public void StartPossessionAudio()
    {
        if (rattleSource != null) rattleSource.Play();
        if (whisperSource != null) whisperSource.Play();
    }

    public void StopPossessionAudio()
    {
        if (rattleSource != null) rattleSource.Stop();
        if (whisperSource != null) whisperSource.Stop();
    }

    // --- GESTION DU GRAB ---

    public void StartGrabHint()
    {
        if (isPlaced) return;
        if (rb != null) rb.isKinematic = false;
        transform.SetParent(null);
        
        StopPossessionAudio();

        if (roomManager != null) 
        {
            roomManager.OnItemFound(gameObject);
        }

        if (destinationFX != null)
        {
            if (holdCoroutine != null) StopCoroutine(holdCoroutine);
            holdCoroutine = StartCoroutine(HoldTimer());
        }
    }

    public void StopGrabHint()
    {
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }

        if (destinationFX != null) 
        {
            destinationFX.SetActive(false);
        }
    }

    private IEnumerator HoldTimer()
    {
        yield return new WaitForSeconds(timeBeforeDestinationHint);
        if (destinationFX != null)
        {
            destinationFX.SetActive(true);
        }
    }

    public void OnSuccessfullyPlaced()
    {
        isPlaced = true;
        StopGrabHint();
        if (destinationFX != null) 
        {
            destinationFX.SetActive(false);
        }
    }
}