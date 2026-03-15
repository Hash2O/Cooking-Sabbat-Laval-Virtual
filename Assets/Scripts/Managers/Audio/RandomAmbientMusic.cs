using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class RandomAmbientMusic : MonoBehaviour
//{
//    [SerializeField] private List<AudioClip> ambientMusic = new List<AudioClip>();
//    [SerializeField] private float timeMin, timeMax;
//    private AudioSource audioSource;

//    void Start()
//    {
//        audioSource = this.GetComponent<AudioSource>();
//        StartCoroutine(PlayRandomAmbientMusic());
//    }

//    IEnumerator PlayRandomAmbientMusic()
//    {
//        int i = Random.Range(0, ambientMusic.Count);
//        yield return new WaitForSeconds(Random.Range(timeMin, timeMax) +  audioSource.clip.length);
//        audioSource.clip = ambientMusic[i];
//        audioSource.Play();
//        StartCoroutine(PlayRandomAmbientMusic());
//    }
//}

[RequireComponent(typeof(AudioSource))]
public class RandomAmbientMusic : MonoBehaviour
{
    [SerializeField] private List<AudioClip> ambientMusic = new();
    [SerializeField] private float timeMin = 1f;
    [SerializeField] private float timeMax = 3f;

    private AudioSource audioSource;
    private int lastIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        StartCoroutine(AmbientMusicLoop());
    }

    private IEnumerator AmbientMusicLoop()
    {
        while (true)
        {
            if (ambientMusic.Count == 0)
                yield break;

            int index;
            do
            {
                index = Random.Range(0, ambientMusic.Count);
            }
            while (index == lastIndex && ambientMusic.Count > 1);

            lastIndex = index;

            AudioClip clip = ambientMusic[index];

            // Assignation AVANT l'attente
            audioSource.clip = clip;

            // Petite pause "respiration" avant le play (sécurité Quest)
            yield return null;

            audioSource.Play();

            // Attente de la fin du clip + pause aléatoire
            float waitTime = clip.length + Random.Range(timeMin, timeMax);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
