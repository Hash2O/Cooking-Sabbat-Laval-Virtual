using UnityEngine;

public class NextStageAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasFinished)
        {
            animator.SetTrigger("End");
            if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayTheGoodSound(10);
            hasFinished = true;
        }
    }
}
