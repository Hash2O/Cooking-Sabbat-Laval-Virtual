using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioInstance { get; private set; }

    [SerializeField] private List<AudioClip> _audioClips = new();
    [SerializeField] private List<AudioClip> _ghostAudioClips;
    [SerializeField] private List<AudioClip> _notificationAudioClips;
    [SerializeField] private List<AudioClip> _itemSoundAudioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        if (audioInstance != null && audioInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayTheGoodSound(int index)
    {
        if(index <= _audioClips.Count - 1)
            audioSource.PlayOneShot(_audioClips[index]);
        //Debug.Log("Sound n�" + index + " has been played successfully !");
    }

    public void PlayGhostSound(int index)
    {
        if(index <= _ghostAudioClips.Count - 1)
            audioSource.PlayOneShot(_ghostAudioClips[index]);
    }

    public void PlayNotificationSound(int index)
    {
        if(index <= _notificationAudioClips.Count - 1)
            audioSource.PlayOneShot(_notificationAudioClips[index]);
    }

    public void PlayItemSound(int index)
    {
        if(index <= _itemSoundAudioClips.Count - 1)
            audioSource.PlayOneShot(_itemSoundAudioClips[index]);
    }
}
