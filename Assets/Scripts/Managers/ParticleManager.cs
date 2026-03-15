using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{

    void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.CompareTag("Cauldron"))
        {
            Debug.Log(other.name);
        }
    }
}
