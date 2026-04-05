using UnityEngine;

public class MandragoreAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayedOnce = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // On s'assure que le son boucle
    }

    // Appelé quand on attrape la plante
    public void StartScreaming()
    {
        // Si c'est la toute première fois qu'on l'attrape
        if (!hasPlayedOnce)
        {
            audioSource.Play();
            hasPlayedOnce = true;
        }
        else
        {
            // Les fois suivantes, on enlève juste la pause !
            audioSource.UnPause();
        }
    }

    // Appelé quand on la pose (sous la cloche, ou par terre)
    public void StopScreaming()
    {
        // On "gèle" le son au lieu de le rembobiner
        audioSource.Pause();
    }
}