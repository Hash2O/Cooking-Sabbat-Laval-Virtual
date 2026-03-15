//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactors;

//public class TeleportationActivator : MonoBehaviour
//{
//    public XRRayInteractor teleportInteractor;
//    public XRRayInteractor rayInteractor;
//    public InputActionProperty teleportActivatorAction;


//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        teleportInteractor.gameObject.SetActive(false);
//        teleportActivatorAction.action.performed += Action_performed;
//        rayInteractor.uiHoverEntered.AddListener(x => DisableTeleportRay());
//    }

//    // Press button to activate the teleport ray
//    private void Action_performed(InputAction.CallbackContext obj)
//    {
//        // Désactiver la téléportation quand le Ray Interactor interagir avec un objet (UI ou autre ayant le layer adequat)
//        if (rayInteractor && rayInteractor.IsOverUIGameObject()) return;

//        teleportInteractor.gameObject.SetActive(true);
//    }

//    public void DisableTeleportRay()
//    {
//        teleportInteractor.gameObject.SetActive(false);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // Stop pressing button : de activate teleport ray
//        if(teleportActivatorAction.action.WasReleasedThisFrame())
//        {
//            teleportInteractor.gameObject.SetActive(false);
//        }
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class TeleportationActivator : MonoBehaviour
{
    public XRRayInteractor teleportInteractor;
    public XRRayInteractor rayInteractor;
    public InputActionProperty teleportActivatorAction;

    void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        teleportActivatorAction.action.performed += Action_performed;
        rayInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
    }

    void OnDisable()
    {
        teleportActivatorAction.action.performed -= Action_performed;
        rayInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (rayInteractor && rayInteractor.IsOverUIGameObject()) return;

        teleportInteractor.gameObject.SetActive(true);
    }

    private void OnUIHoverEntered(UIHoverEventArgs args)
    {
        DisableTeleportRay();
    }

    public void DisableTeleportRay()
    {
        teleportInteractor.gameObject.SetActive(false);
    }

    void Update()
    {
        if (teleportActivatorAction.action.WasReleasedThisFrame())
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }
}