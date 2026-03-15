using UnityEngine;


public class SecretItemRandomPosition : MonoBehaviour
{
    [SerializeField] private Transform[] randomTransforms;
    [SerializeField] private GameObject secretItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomizeSecretItemPosition();
    }

    private void RandomizeSecretItemPosition()
    {
        int randomIndex = Random.Range(0, randomTransforms.Length);
        Debug.Log("Random Index : " +  randomIndex);
        secretItem.transform.position = randomTransforms[randomIndex].position;
    }
}
