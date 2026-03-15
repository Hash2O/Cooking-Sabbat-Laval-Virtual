using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TargetManager : MonoBehaviour
{
    //This script manages the modifications of the GO through the game
    //(Color, shape, and so on...)


    [SerializeField] Color newColor;

    [SerializeField] GameObject newItem;

    private Vector3 scaleChange;

    private float scalingSpeed = 1.0f;
    private float _maxWidth = 1f;

    private bool isWhite;
    private bool mustScale;

    public int targetScore;

    // Start is called before the first frame update
    void Start()
    {
        isWhite = true;
        mustScale = true;
        scaleChange = new Vector3(0.25f, 0.25f, 0.25f);
        targetScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Verif
        //print("Score " + gameObject.name + " : " + targetScore);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isWhite && mustScale)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = newColor;
            isWhite = false;
            targetScore++;
        }
        else if (!isWhite && mustScale)
        {
            StartCoroutine(scaleTime());
            mustScale = false;
            targetScore++;
        }
        else
        {
            targetScore += 5;
            Debug.Log("Each hit will give you five more points while the circle is active !");
        }
    }

    IEnumerator scaleTime()
    {
        while (transform.localScale.x < _maxWidth)
        {
            transform.localScale += new Vector3(scalingSpeed * Time.deltaTime, scalingSpeed * Time.deltaTime, 0);
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
        newItem.SetActive(true);
        //Instantiate(newItem, transform.position, transform.rotation);
    }

}
