using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{

    [SerializeField] private List<AudioClip> creepySound = new List<AudioClip>();
    [SerializeField] private float timeMin, timeMax;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(PlaySound());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlaySound()
    {
        int i = Random.Range(0, creepySound.Count);
        yield return new WaitForSeconds(Random.Range(timeMin, timeMax) + audioSource.clip.length);
        audioSource.clip = creepySound[i];
        audioSource.Play();
        StartCoroutine(PlaySound());
    }
}
