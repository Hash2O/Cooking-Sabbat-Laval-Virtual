using System.Collections;
using UnityEngine;

public class StartCupManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem splashVFX;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ingredient"))
        {
            StartCoroutine(nameof(SplashTransition), 3);
            Destroy(other.gameObject);
        }
    }

    IEnumerator SplashTransition(int time)
    {
        AudioManager.audioInstance.PlayTheGoodSound(3); //Splash sound
        splashVFX.Play();
        yield return new WaitForSeconds(time);
        GameManager.GMInstance.LoadNextScene();   // Load Main Scene
    }
}
