using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("Durée avant destruction (en secondes)")]
    public float lifetime = 3f;

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}

