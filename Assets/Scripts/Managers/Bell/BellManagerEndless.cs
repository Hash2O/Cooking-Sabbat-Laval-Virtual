using UnityEngine;

public class BellManagerEndless : MonoBehaviour
{
    [SerializeField] private GhostCycleManagerEndless ghostManagerEndless;
    [SerializeField] private ParticleSystem startNotificationVFX;

    public bool isBellActivated = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(AudioManager.audioInstance != null)
            AudioManager.audioInstance.PlayTheGoodSound(7);     // Bell ringing
        Debug.Log("Ring my bell !");

        if(isBellActivated == false)
        {
            Debug.Log("Les clients fant�mes sont invit�s � venir passer commande !");
            isBellActivated = true;
            startNotificationVFX.Play();
            //GameCycleManager.GameCycleInstance.StartNight();
            if(ghostManagerEndless != null) ghostManagerEndless.gameObject.SetActive(true);
        }
    }
}
