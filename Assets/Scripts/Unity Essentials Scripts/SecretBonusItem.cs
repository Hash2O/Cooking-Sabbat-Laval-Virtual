using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SecretBonusItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI secretItemText;
    [SerializeField] GameObject secretBonusIcon;

    [SerializeField] AudioSource itemAudioSource;

    private bool isVisible;
    public bool isCollectible;
    public bool isCollected;

    // Start is called before the first frame update
    void Start()
    {
        isCollectible = true;
        itemText.SetText("");
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<MeshRenderer>().enabled == true)
        {
            itemText.SetText(gameObject.name + " Founded !");
            isVisible = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball") && isCollectible && isVisible)
        {
            StartCoroutine(BonusObjectLifeCycleCoroutine(2));
            isCollectible = false;  //Disable item, not to catch him twice in a row
        }
    }

    IEnumerator BonusObjectLifeCycleCoroutine(int time)
    {
        itemAudioSource.Play();
        isCollected = true;
        Debug.Log(gameObject.name + " is collected : " + isCollected);
        secretBonusIcon.SetActive(true);
        secretItemText.SetText("Secret Item added \nto your collection !");
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

}
