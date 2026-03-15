using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        // Calcule le vecteur direction depuis l'objet vers le player
        Vector3 direction = transform.position - player.position; // inverse par rapport à (player.position - transform.position)

        // Oriente l'objet vers cette direction
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
