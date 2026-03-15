using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLifeCycle : MonoBehaviour
{

    //This script manage the time before the ball game object deactivation

    //NB : Design Pattern incoming (object pooling)

    private float timeBeforeVanish;

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeVanish = 8f;      //Nexus VI Security
        StartCoroutine(ballLifeCycleCoroutine(4));
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < - 2f)
        {
            gameObject.SetActive(false);
        }

        TimeBeforeVanish();
    }

    //Made to make the ball being disabled 5 sec after being enabled
    IEnumerator ballLifeCycleCoroutine(int time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    //Security type Nexus VI : 10 sec life max, if other means have failed to disable the ball clone
    void TimeBeforeVanish()
    {
        if (timeBeforeVanish > 0)
        {
            timeBeforeVanish -= Time.deltaTime;
            //print(timeBeforeVanish);
        }
        if (timeBeforeVanish <= 0)
        {
            gameObject.SetActive(false);
            timeBeforeVanish = 8f;
        }
    }
}
