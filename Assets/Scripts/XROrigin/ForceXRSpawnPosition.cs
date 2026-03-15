using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class ForceXRSpawnPosition : MonoBehaviour
{
    [Header("XR Rig � repositionner")]
    public GameObject xrRig;

    [Header("Position forc�e")]
    public Transform spawnPoint;

    private bool hasRecentered = false;

    private void Start()
    {
        //if (xrRig.transform.position.y < 0) xrRig.transform.position = spawnPoint.position;
        StartCoroutine(RecenterXR());
    }

    private void Update()
    {
        
    }

    private IEnumerator RecenterXR()
    {
        // On attend quelques frames que le XR s'initialise
        yield return null;
        yield return null;

        // Forcer la recentering plateforme (Quest)
        XRInputSubsystem subsystem = null;

        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        if (subsystems.Count > 0)
            subsystem = subsystems[0];

        subsystem?.TryRecenter();

        // Et maintenant on force la position du XR Rig
        if (xrRig != null && spawnPoint != null)
        {
            xrRig.transform.position = spawnPoint.position;
            xrRig.transform.rotation = spawnPoint.rotation;
        }

        hasRecentered = true;
    }
}

