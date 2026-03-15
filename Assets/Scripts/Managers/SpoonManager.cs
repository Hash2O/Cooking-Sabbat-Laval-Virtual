using UnityEngine;

public class SpoonManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;


    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -1.0f)
        {
            transform.SetPositionAndRotation(spawnPoint.position, Quaternion.AngleAxis(0, Vector3.up));
        }
    }
}
