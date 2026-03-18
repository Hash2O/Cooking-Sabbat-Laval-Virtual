using UnityEngine;

public class BellManager : MonoBehaviour
{
    [SerializeField] private GhostCycleManager ghostManager;
    [SerializeField] private ParticleSystem startNotificationVFX;
    [SerializeField] private BellInteraction bellInteraction;

    public bool isBellActivated = false;

    private void Awake()
    {
        bellInteraction = GetComponent<BellInteraction>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(7);     // Bell ringing
        Debug.Log("Ring my bell !");

        if(isBellActivated == false)
        {
            //isBellActivated = true;
            startNotificationVFX.Play();
            //GameCycleManager.GameCycleInstance.StartNight();
            bellInteraction.TryActivateBell();
            if (ghostManager != null) ghostManager.gameObject.SetActive(true);
        }
    }
}
