using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleObjectLifeCycle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statusText;

    [SerializeField] int timeBeforeVanish;

    [SerializeField] AudioSource objectAudioSource;

    private bool isCollectible;

    public bool isCollected;

    // Start is called before the first frame update
    void Start()
    {
        objectAudioSource = GetComponent<AudioSource>();

        isCollectible = true;

        statusText.SetText(gameObject.name + "\n Throw a ball to add it to your collection.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isCollectible)
        {
            StartCoroutine(CollectibleObjectLifeCycleCoroutine(timeBeforeVanish));
            isCollectible = false;  //Disable item, not to catch him twice in a row
        }
        
    }

    IEnumerator CollectibleObjectLifeCycleCoroutine(int time)
    {
        statusText.SetText("Well done ! \n" + gameObject.name + " added to your collection !");
        objectAudioSource.Play();
        isCollected = true;
        Debug.Log(gameObject.name + " is collected : " + isCollected);
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
