using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    [SerializeField]
    private float _rayLength; //Pour gérer la portée du ray

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShootRaycast();
    }

    private void ShootRaycast()
    {
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out RaycastHit hit, _rayLength))
        {
            Debug.Log("Le rayon a croisé " + hit.transform.name);

            if (hit.transform.CompareTag("Interactible"))
            {
                Debug.Log("New Item Founded : " + hit.transform.name);
                MeshRenderer itemMR = hit.transform.GetComponent<MeshRenderer>();
                if (itemMR != null)
                {
                    Debug.Log("Make the item visible");
                    itemMR.enabled = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward * _rayLength);
    }
}
