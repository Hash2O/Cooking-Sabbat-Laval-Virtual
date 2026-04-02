using UnityEngine;

public class BreakObjectOnCollision : MonoBehaviour
{
    [Tooltip("Vitesse d'impact minimum en mètres par seconde")]
    public float minSpeed = 1.5f;
    public GameObject brokenGlassFX;

    public PotionBottle bottle;
    private Rigidbody rb;

    private void Start() 
    {   
        bottle = GetComponent<PotionBottle>();
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ghost") || (rb != null && rb.linearVelocity.magnitude > minSpeed))
        {
            HandleImpact(col.gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ghost") || col.relativeVelocity.magnitude > minSpeed)
        {
            HandleImpact(col.gameObject);
        }
    }

    private void HandleImpact(GameObject targetObject)
    {
        bool hasHitValidTarget = false;

        if(targetObject.CompareTag("Furnitures"))
        {
            hasHitValidTarget = true;
        }
        else if(targetObject.CompareTag("Ghost"))
        {
            hasHitValidTarget = true;
            Debug.Log("Hit a ghost");
            targetObject.GetComponent<GhostClient>().TriggerReceivePotion(1f, bottle);              
        }

        if (hasHitValidTarget)
        {
            Vector3 position = this.transform.position;
            Instantiate(brokenGlassFX, position, Quaternion.identity);
            AudioManager.audioInstance.PlayTheGoodSound(16);               
            Destroy(bottle.gameObject);
        }
    }
}