using System.Collections.Generic;
using UnityEngine;

public class CookingPotManager : MonoBehaviour
{

    [SerializeField] private List<ParticleSystem> particleEffects;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> cookingClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ingredient"))
        {
            Debug.Log("Contact !");
            Instantiate(particleEffects[Random.Range(0, particleEffects.Count)], other.gameObject.transform.position, Quaternion.identity);
            audioSource.PlayOneShot(cookingClips[Random.Range(0, cookingClips.Count)]);
            if (other.gameObject != null) Destroy(other.gameObject, 0.1f);
        }
    }
}
