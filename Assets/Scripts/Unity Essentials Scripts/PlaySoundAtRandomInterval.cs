using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtRandomInterval : MonoBehaviour
{
    public float minSecond = 5f; // Minimum interval to wait before playing sound.
    public float maxSecond = 15f; // Maximum interval to wait before playing sound.

    private AudioSource audioSourceX;

    private void Awake()
    {
        audioSourceX = GetComponent<AudioSource>();
        StartCoroutine(PlaySounds());
    }

    private IEnumerator PlaySounds()
    {
        while (true)
        {
            float waitTime = Random.Range(minSecond, maxSecond);
            yield return new WaitForSeconds(waitTime);
            audioSourceX.Play();
        }
    }
}