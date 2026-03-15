using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils; // Nécessaire pour XROrigin

public class ForceRoomscale : MonoBehaviour
{
    public XROrigin xrOrigin;

    void Start()
    {
        if (xrOrigin == null) xrOrigin = GetComponent<XROrigin>();

        // Force le mode "Floor" pour le Roomscale
        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
        // Optionnel : Réaligne la caméra si nécessaire
        // xrOrigin.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);
    }
}

