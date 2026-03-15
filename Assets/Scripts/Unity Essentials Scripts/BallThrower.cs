using UnityEngine;

public class BallThrower : MonoBehaviour
{
    [SerializeField] private float force;    //Pour gérer la puissance de la balle

    //Variables we'll need to reference other objects in our game
    public GameObject _ballPrefab;  //This will store the Ball Prefab we created earlier, so we can spawn a new Ball whenever we want
    public Camera _mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ThrowBall();
        }
    }

    private void ThrowBall()
    {
        //Let's spawn a new ball to bounce around our space
        //Object Pooling
        GameObject newBall = BallPooler.SharedInstance.GetPooledObject();

        if (newBall != null)
        {
            newBall.SetActive(true);
        }

        newBall.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));   //Set the rotation of our new Ball
        newBall.transform.position = _mainCamera.transform.position + _mainCamera.transform.forward;    //Set the position of our new Ball to just in front of our Main Camera

        //Add velocity to our Ball, here we're telling the game to put Force behind the Ball in the direction Forward from our Camera (so, straight ahead)
        Rigidbody rigbod = newBall.GetComponent<Rigidbody>();
        rigbod.linearVelocity = new Vector3(0f, 0f, 0f);
        //NB : Think to give the ball a force (Unity UI)
        rigbod.AddForce(_mainCamera.transform.forward * force);
    }
}
