using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    void Start()
    {
        float bgmFloat = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        
        bgmFloat = Mathf.Max(0.0001f, bgmFloat);
        
        _audioMixer.SetFloat("VolumeMaster", Mathf.Log10(bgmFloat) * 20);
    }
}
