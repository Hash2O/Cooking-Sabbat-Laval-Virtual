using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;   // Pour la gestion de l'haptique sur StirringProcess

public class StirringManager : MonoBehaviour
{
    [Header("Références")]
    public Transform cauldronCenter;     // Centre du bol (un Empty placé au milieu)
    public Transform cookingSpoon;          // La cuillère tenue par le joueur
    public ParticleSystem stirEffect; // Optionnel : effet visuel quand ça mélange
    [SerializeField] private AudioSource stirringLoopAudio; // Gérer le son du remuage

    [Header("Audio Fade")]
    public float fadeOutDuration = 0.5f;

    [Header("Paramètres")]
    public float stirProgress;   // Progression du mélange [0..1]
    public float requiredProgress; // Seuil pour finir le mélange
    public float stirMultiplier; // Vitesse de progression (ajustable)

    [Header("Gestion de l'haptique")]
    [SerializeField] private XRGrabInteractable spoonInteractable;
    public float hapticInterval = 0.05f;
    private float hapticTimer = 0f;
    private XRDirectInteractor currentInteractor;

    [HideInInspector]
    public bool isWellStirred;

    public Cauldron linkedCauldron;
    public bool isInBowl = false;

    private Vector3 lastSpoonPos;
    private Coroutine fadeCoroutine;
    private float initialVolume;

    private void Awake()
    {
        spoonInteractable = GameObject.FindWithTag("Spoon").GetComponent<XRGrabInteractable>();
        spoonInteractable.selectEntered.AddListener(OnGrab);
        spoonInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as XRDirectInteractor;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void Start()
    {
        ResetStirringValues();
        initialVolume = stirringLoopAudio.volume;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == cookingSpoon)
        {
            isInBowl = true;
            lastSpoonPos = cookingSpoon.position;
            Debug.Log("Cuillère dans le bol !");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == cookingSpoon)
        {
            isInBowl = false;
            Debug.Log("Cuillère sortie du bol !");
        }
    }

    void Update()
    {
        if (isInBowl && stirProgress < requiredProgress)
        {
            StirringProcess();
        }
    }

    void StirringProcess()
    {

        Vector3 spoonOffsetNow = cookingSpoon.position - cauldronCenter.position;
        Vector3 spoonOffsetLast = lastSpoonPos - cauldronCenter.position;

        // Produit vectoriel pour estimer la "rotation" autour du centre du bol
        float circularity = Vector3.Cross(spoonOffsetLast, spoonOffsetNow).magnitude;

        // Base du process de gestion du retour haptique
        float intensity = Mathf.Clamp01(circularity * 50f);

        // Mise à jour de la progression
        stirProgress += circularity * stirMultiplier * Time.deltaTime;

        // Feedback visuel : lancer des particules si présentes
        if (stirEffect != null && !stirEffect.isPlaying && isInBowl)
        {
            stirEffect.Play();

            if (!stirringLoopAudio.isPlaying)
            {
                stirringLoopAudio.volume = initialVolume;
                stirringLoopAudio.Play();
            }
        }

        // Haptique : gérer la vibration
        hapticTimer -= Time.deltaTime;

        if (isInBowl && intensity > 0.01f && hapticTimer <= 0f)
        {
            if (currentInteractor != null)
            {
                currentInteractor.SendHapticImpulse(intensity, hapticInterval);
            }
            hapticTimer = hapticInterval;
        }

        // Vérification si terminé
        if (stirProgress >= requiredProgress)
        {
            stirProgress = requiredProgress;
            isWellStirred = true;
            if (stirEffect != null) stirEffect.Stop();
            //if (stirringLoopAudio.isPlaying) stirringLoopAudio.Stop();
            if (stirringLoopAudio.isPlaying)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);

                fadeCoroutine = StartCoroutine(FadeOutAudio(stirringLoopAudio, fadeOutDuration));
            }
            Debug.Log("Mélange terminé !");

            if (linkedCauldron != null)
                linkedCauldron.SendMessage("FinishRecipe", SendMessageOptions.DontRequireReceiver);
        }

        lastSpoonPos = cookingSpoon.position;
    }

    IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        // Reset pour la prochaine utilisation
        audioSource.volume = initialVolume;
    }

    public void ResetStirringValues()
    {
        stirProgress = 0f;
        requiredProgress = 2f;
        stirMultiplier = 150f;
        isWellStirred = false;
    }
}
