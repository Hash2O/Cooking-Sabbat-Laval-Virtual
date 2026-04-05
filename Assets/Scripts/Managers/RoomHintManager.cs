using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Burst.CompilerServices;

public class RoomHintManager : MonoBehaviour
{
    [Header("Paramètres d'aide")]
    public float timeBeforeHint = 20f;
    public List<GameObject> hiddenItems;
    public Vector3 shakeRotation;
    public float positionShake;

    private Coroutine hintCoroutine;
    private bool isPlayerInside = false;
    
    private GameObject currentHintedItem = null;
    
    // --- NOUVEAU : La mémoire du Transform ---
    private Transform currentlyShakingTransform = null;
    private Vector3 savedLocalPosition;
    private Quaternion savedLocalRotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            RestartHintTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            StopHintSystem();
        }
    }

    public void RestartHintTimer()
    {
        if (hintCoroutine != null) StopCoroutine(hintCoroutine);
        
        if (hiddenItems.Count > 0)
        {
            hintCoroutine = StartCoroutine(IdleHintTimer());
        }
    }

    private IEnumerator IdleHintTimer()
    {
        yield return new WaitForSeconds(timeBeforeHint);
        hiddenItems.RemoveAll(item => item == null);

        if (hiddenItems.Count > 0)
        {
            ApplyPossessionEffect(hiddenItems[0]);
        }
    }

    private void StopHintSystem()
    {
        if (hintCoroutine != null) StopCoroutine(hintCoroutine);
        
        if (currentHintedItem != null)
        {
            // On restaure le transform avant de nettoyer
            RestoreTransformAndKillTween();
            
            ItemHint hintScript = currentHintedItem.GetComponent<ItemHint>();
            if (hintScript != null) hintScript.StopPossessionAudio();
            
            currentHintedItem = null;
        }
    }

    private void ApplyPossessionEffect(GameObject item)
    {
        currentHintedItem = item;
        ItemHint hintScript = item.GetComponent<ItemHint>();

        Transform targetToShake = item.transform;
        if (hintScript != null)
        {
            targetToShake = hintScript.GetTransformToShake();
            hintScript.StartPossessionAudio();
        }

        // --- NOUVEAU : On sauvegarde l'état initial AVANT de secouer ---
        currentlyShakingTransform = targetToShake;
        savedLocalPosition = targetToShake.localPosition;
        savedLocalRotation = targetToShake.localRotation;

        targetToShake.DOShakeRotation(2f, shakeRotation, 10, 90, false)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        targetToShake.DOShakePosition(2f, positionShake, 10, 90, false, false)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
            if (targetToShake != item.transform) 
        {
            // On le glisse dans l'arrosoir
            item.transform.SetParent(targetToShake); 
            
            // On désactive sa physique pour qu'il ne rebondisse plus
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; 
        }
    }

    public void OnItemFound(GameObject foundItem)
    {
        if (hiddenItems.Contains(foundItem))
        {
            hiddenItems.Remove(foundItem);
            
            if (currentHintedItem == foundItem)
            {
                // On restaure le transform
                RestoreTransformAndKillTween();
                currentHintedItem = null;
            }

            if (isPlayerInside)
            {
                RestartHintTimer();
            }
        }
    }

    // --- NOUVEAU : Fonction de nettoyage et restauration ---
    private void RestoreTransformAndKillTween()
    {
        if (currentlyShakingTransform != null)
        {
            // 1. On tue l'animation
            DOTween.Kill(currentlyShakingTransform);
            
            // 2. On remet exactement à la position et rotation sauvegardées
            currentlyShakingTransform.localPosition = savedLocalPosition;
            currentlyShakingTransform.localRotation = savedLocalRotation;
            
            if (currentHintedItem != null && currentlyShakingTransform != currentHintedItem.transform)
            {
                currentHintedItem.transform.SetParent(null); // Il redevient indépendant
                
                Rigidbody rb = currentHintedItem.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false; // La gravité revient !
            }
            // 3. On vide la mémoire
            currentlyShakingTransform = null;
        }
    }

    public void InterruptHint()
    {
        if (currentHintedItem != null)
        {
            RestoreTransformAndKillTween();
            ItemHint hintScript = currentHintedItem.GetComponent<ItemHint>();
            if (hintScript != null) hintScript.StopPossessionAudio();
            currentHintedItem = null;

            if (isPlayerInside) RestartHintTimer();
        }
    }
}