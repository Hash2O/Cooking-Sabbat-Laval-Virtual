using UnityEngine;

public class GhostMovementManager : MonoBehaviour
{
    [SerializeField] private float _floatingSpeed;
    [SerializeField] private float maxYPosition;
    [SerializeField] private float minYPosition;

    private Vector3 upAndDown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        MonterDescendre();
    }

    void MonterDescendre()
    {
        transform.Translate(upAndDown * Time.deltaTime);
        if (transform.localPosition.y <= minYPosition)
        {
            upAndDown = new Vector3(0, _floatingSpeed, 0);
        }
        else if (transform.localPosition.y > maxYPosition)
        {
            upAndDown = new Vector3(0, -_floatingSpeed, 0);
        }
    }
}
