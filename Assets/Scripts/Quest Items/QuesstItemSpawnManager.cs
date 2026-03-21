using UnityEngine;

public class QuesstItemSpawnManager : MonoBehaviour
{
    public string itemID;
    public GameObject itemToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemID))
        {
            if (itemToSpawn != null) itemToSpawn.SetActive(true);
        }
    }
}
