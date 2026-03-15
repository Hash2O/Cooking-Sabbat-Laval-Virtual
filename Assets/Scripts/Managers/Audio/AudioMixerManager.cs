using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Slider _bgmSlider;

    private void Awake()
    { 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject sliderToFind = GameObject.FindWithTag("AudioSlider");
        //Debug.Log("sliderToFind : " + sliderToFind.name);
        if(sliderToFind != null) _bgmSlider = sliderToFind.GetComponentInChildren<Slider>();
    }

    void Update()
    {
        float bgmFloat = _bgmSlider.value;
        _audioMixer.SetFloat("VolumeMaster", Mathf.Log10(bgmFloat) * 20);
    }
}
