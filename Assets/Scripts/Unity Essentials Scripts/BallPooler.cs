using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPooler : MonoBehaviour
{
    /// <summary>
    /// OBJECT POOLING DESIGN PATTERN
    /// </summary>

    //Instanciation
    public static BallPooler SharedInstance;

    //Pooled objects' list (obviously)
    public List<GameObject> pooledObjects = new();

    //If only one type of object (i.e. ball) is required
    public GameObject objectToPool;

    //If random types of balls are in order
    //public List<GameObject> cubesToPool;
    public int amountToPool;


    //Object Pooling
    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PoolInstantiation();
    }

    private void PoolInstantiation()
    {
        GameObject tmp;

        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);

            //Optionnal (for multi types of balls)
            //tmp = Instantiate(cubesToPool[Random.Range(0, cubesToPool.Count)]);

            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}

