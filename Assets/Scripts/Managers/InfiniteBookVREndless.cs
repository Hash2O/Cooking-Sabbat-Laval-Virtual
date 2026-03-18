using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InfiniteBookEndless : MonoBehaviour
{
    [Header("Réglages")]
    public Animator bookAnimator;
    
    [Header("Animations")]
    public string animRightToLeft = "PageTurnRight";
    public string animLeftToRight = "PageTurnLeft"; 

    [Header("Matériaux & Rendu")]
    public Renderer staticBookRenderer; 
    public Renderer movingPageRenderer; 
    public List<Material> pageMaterials;
    
    [Tooltip("Matériau vide (blanc ou transparent) pour quand il n'y a plus de pages")]
    public Material emptyPageMaterial; 

    [Tooltip("La distance horizontale de la main nécessaire pour tourner la page complètement")]
    public float dragDistance = 0.5f;

    [Header("Debug")]
    [Range(0, 1)]
    [SerializeField] private float currentProgress = 0f;
    public TMP_Text debug;

    private Transform currentHand;
    private Vector3 startHandPos;
    private float startProgress;
    private bool isGrabbing = false;
    private string activeStateName; 
    private Vector3 localHandPos;

    private bool isTurningNext; 

    [Header("Réglages de Direction")]
    public Vector3 pageDirection = new Vector3(-1, 0, 0); 

    [Header("Livre Infini")]
    public int totalPages = 4; 
    public int currentPageIndex = 0; 

    private void Start()
    {
        activeStateName = animRightToLeft;
        UpdateBookContent(currentPageIndex);
    }

    // On passe l'index en paramètre pour gérer la "Prévisualisation"
    void UpdateBookContent(int indexToRender)
    {
        if (pageMaterials == null || pageMaterials.Count == 0) return;

        int baseIndex = indexToRender * 2;

        Material matStaticLeft  = GetMaterialSafe(baseIndex);     
        Material matMovingFront = GetMaterialSafe(baseIndex + 1); 
        Material matMovingBack  = GetMaterialSafe(baseIndex + 2); 
        Material matStaticRight = GetMaterialSafe(baseIndex + 3); 

        if (staticBookRenderer != null)
        {
            Material[] mats = staticBookRenderer.materials;
            if (mats.Length >= 2)
            {
                mats[2] = matStaticLeft;  
                mats[1] = matStaticRight; 
                staticBookRenderer.materials = mats; 
            }
        }

        if (movingPageRenderer != null)
        {
            Material[] mats = movingPageRenderer.materials;
            if (mats.Length >= 2)
            {
                mats[0] = matMovingBack; 
                mats[1] = matMovingFront; 
                movingPageRenderer.materials = mats; 
            }
        }
    }

    Material GetMaterialSafe(int index)
    {
        if (index < 0) return pageMaterials[0]; 
        if (index >= pageMaterials.Count) 
        {
            return emptyPageMaterial != null ? emptyPageMaterial : pageMaterials[pageMaterials.Count - 1];
        }
        return pageMaterials[index];
    }

    public void StartGrab(SelectEnterEventArgs args)
    {
        GameObject hand = args.interactorObject.transform.gameObject;
        localHandPos = transform.InverseTransformPoint(hand.transform.position);

        // --- GESTION DU GRAB ---
        if (localHandPos.x < 0) // Main à Droite -> SUIVANT
        {
            if (currentPageIndex >= totalPages) return;

            isTurningNext = true;   
            activeStateName = animRightToLeft;
            currentProgress = 0f; 

            // On affiche les pages actuelles
            UpdateBookContent(currentPageIndex);
        }
        else // Main à Gauche -> PRECEDENT
        {
            if (currentPageIndex <= 0) return;

            isTurningNext = false;    
            activeStateName = animLeftToRight;
            currentProgress = 1f; 

            // ASTUCE : On charge les matériaux "d'avant" pour que la page qu'on tient
            // ressemble bien à la page précédente qu'on est en train de ramener.
            UpdateBookContent(currentPageIndex - 1);
        }

        currentHand = hand.transform;
        isGrabbing = true;
        startHandPos = localHandPos; 
        startProgress = currentProgress; 

        bookAnimator.speed = 0;
        ApplyAnimation(currentProgress);
    }

    public void EndGrab(SelectExitEventArgs args)
    {
        if (!isGrabbing) return; 

        isGrabbing = false;
        currentHand = null;

        float target = currentProgress > 0.5f ? 1f : 0f;
        StartCoroutine(SmoothFinish(target));
    }

    void Update()
    {
        if (!isGrabbing && bookAnimator.speed == 0)
            ApplyAnimation(currentProgress);

        if (isGrabbing && currentHand != null)
        {
            UpdatePagePosition();
        }
        //debug.text = "Progress: " + currentProgress + "\nMain: " + localHandPos.x + "\nIsGrabbing: " + isGrabbing;
    }

    void UpdatePagePosition()
    {
        Vector3 localHandPos = transform.InverseTransformPoint(currentHand.position);
        Vector3 movementVector = localHandPos - startHandPos; 

        float moveProjected = Vector3.Dot(movementVector, pageDirection);
        float newProgress = startProgress - (moveProjected / dragDistance);
        
        currentProgress = Mathf.Clamp01(newProgress);
        ApplyAnimation(currentProgress);
    }

    void ApplyAnimation(float progress)
    {
        if (activeStateName == animRightToLeft)
            bookAnimator.Play(activeStateName, 0, progress);
        else 
            bookAnimator.Play(activeStateName, 0, 1f - progress);
    }

    System.Collections.IEnumerator SmoothFinish(float targetValue)
    {
        float duration = 0.2f;
        float startValue = currentProgress;
        float time = 0;

        while (time < duration)
        {
            if (isGrabbing) yield break;
            time += Time.deltaTime;
            currentProgress = Mathf.Lerp(startValue, targetValue, time / duration);
            ApplyAnimation(currentProgress);
            yield return null;
        }

        // On termine l'animation visuelle
        currentProgress = targetValue;
        ApplyAnimation(currentProgress);

        // --- VALIDATION DU CHANGEMENT ---
        bool pageChanged = false;

        // Si on visait Gauche (1) et qu'on y est arrivé -> Suivant Validé
        if (isTurningNext && targetValue >= 0.99f)
        {
            currentPageIndex++;
            pageChanged = true;
        }
        // Si on visait Droite (0) et qu'on y est arrivé -> Précédent Validé
        else if (!isTurningNext && targetValue <= 0.01f)
        {
            currentPageIndex--;
            pageChanged = true;
        }

        // --- LE RESET FINAL ---
        
        // Qu'on ait changé de page ou qu'on ait annulé le mouvement,
        // on doit s'assurer que les matériaux correspondent à l'index final
        UpdateBookContent(currentPageIndex);

        // CRUCIAL : Une fois l'opération finie et les matériaux mis à jour,
        // la page physique doit TOUJOURS revenir à sa position de repos à DROITE (0).
        // C'est ce qui crée l'illusion d'infini.
        currentProgress = 0f;
        ApplyAnimation(currentProgress);
    }
}