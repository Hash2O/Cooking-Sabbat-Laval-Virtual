using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class VerminBusterManager : MonoBehaviour
{
    [SerializeField] private GameObject smashPrefab;
    //[SerializeField] private GameObject splashPrefab;
    [SerializeField] private ParticleSystem smashParticle;
    [SerializeField] private float impactVelocity = 0.1f;

    private Rigidbody rb;
    private AudioSource smashAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        smashAudio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Vermin") && rb.linearVelocity.sqrMagnitude >= impactVelocity)
        {
            Debug.Log("Velocity : " + rb.linearVelocity.sqrMagnitude);
            GameObject prefab = Instantiate(smashPrefab, collision.transform.position, collision.transform.rotation);
            //Instantiate(splashPrefab, prefab.transform.position, Quaternion.identity);
            smashParticle.Play();
            smashAudio.Play();

            VerminTrigger verminScript = collision.gameObject.GetComponent<VerminTrigger>();
            if (verminScript != null)
            {
                verminScript.HitByWeapon();
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }

}
