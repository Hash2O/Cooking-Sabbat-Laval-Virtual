using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollection : MonoBehaviour
{
    [SerializeField] AudioSource audioEnvironment;

    [SerializeField] GameObject[] itemCollection;

    CollectibleObjectLifeCycle firstItem;
    CollectibleObjectLifeCycle secondItem;
    CollectibleObjectLifeCycle thirdItem;

    [SerializeField] TextMeshProUGUI firstItemText;
    [SerializeField] TextMeshProUGUI secondItemText;
    [SerializeField] TextMeshProUGUI thirdItemText;

    [SerializeField] GameObject firstIcon;
    [SerializeField] GameObject secondIcon;
    [SerializeField] GameObject thirdIcon;

    [SerializeField] TextMeshProUGUI victoryText;

    bool firstCondition;
    bool secondCondition;
    bool thirdCondition;

    public bool isLevelFinished;

    // Start is called before the first frame update
    void Start()
    {
        firstItem = itemCollection[0].GetComponent<CollectibleObjectLifeCycle>();
        secondItem = itemCollection[1].GetComponent<CollectibleObjectLifeCycle>();
        thirdItem = itemCollection[2].GetComponent<CollectibleObjectLifeCycle>();

        firstItemText.SetText("");
        secondItemText.SetText("");
        thirdItemText.SetText("");

        isLevelFinished = false;

    }

    // Update is called once per frame
    void Update()
    {
        //First Item Collected
        if(firstItem != null && firstItem.isCollected == true)
        {
            Debug.Log("First Item Collected Successfully !");
            firstIcon.SetActive(true);
            firstItemText.SetText("Success");
            firstCondition = true;
        }

        //Second Item Collected
        if (secondItem != null && secondItem.isCollected == true)
        {
            Debug.Log("Second Item Collected Successfully !");
            secondIcon.SetActive(true);
            secondItemText.SetText("Success");
            secondCondition = true;
        }

        //Third Item Collected
        if (thirdItem != null && thirdItem.isCollected == true)
        {
            Debug.Log("Third Item Collected Successfully !");
            thirdIcon.SetActive(true);
            thirdItemText.SetText("Success");
            thirdCondition = true;
        }

        //Then, if all items have been collected, collection is complete
        if(firstCondition && secondCondition && thirdCondition)
        {
            isLevelFinished = true;
            audioEnvironment.enabled = false;
            victoryText.gameObject.SetActive(true);
            victoryText.SetText("Well Done ! \nYou have found all lost items !");
        }
    }
}
