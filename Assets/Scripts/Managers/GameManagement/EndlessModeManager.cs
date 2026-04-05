using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class EndlessModeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float modeDurationInMinutes;
    private float modeDurationInSeconds;
    public float bonusTimeInSeconds;
    
    public float elapsedTime;
    public Transform clockPointer;
    public float degres;

    private bool timeOut, timeUp, endSoundPlaying;
    public GhostCycleManagerEndless ghostCycleManagerEndless;
    void Start()
    {
        modeDurationInMinutes = PlayerPrefs.GetInt("EndlessDuration", 3);
        elapsedTime = 0f;
        modeDurationInSeconds = modeDurationInMinutes * 60;
        degres = 360/modeDurationInSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if(!timeOut && timeUp)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > modeDurationInSeconds)
            {
                Debug.Log("Time out!");
                TimeOut();
            } 
            else if (elapsedTime > modeDurationInSeconds -6.5f && !endSoundPlaying)
            {
                EndSound();
            }
        }
    }

    public void AddBonusTime()
    {
        elapsedTime -= bonusTimeInSeconds;
        
        if (elapsedTime < 0f) 
        {
            elapsedTime = 0f;
        }
        clockPointer.Rotate(0f, 0f, -(bonusTimeInSeconds * degres));
        StartCoroutine(DelayTickingSound());
        if (modeDurationInSeconds - elapsedTime > 6.5f)
        {
            endSoundPlaying = false;
        }
    }

    private IEnumerator DelayTickingSound()
    {
        yield return new WaitForSeconds(0.75f);
        AudioManager.audioInstance.PlayTheGoodSound(12);
    }

    public void StartCountdown()
    {
        timeUp = true;
        StartCoroutine(SecondPointerMovement());
    }

    private void EndSound()
    {  
        AudioManager.audioInstance.PlayTheGoodSound(13);
        endSoundPlaying = true;
    }
    private IEnumerator SecondPointerMovement()
    {
        while(!timeOut && timeUp)
         {
            yield return new WaitForSeconds(1.0f);
            clockPointer.Rotate(0f, 0f, degres);
         }
    }

    private void TimeOut()
    {      
        timeOut = true;
        if(ghostCycleManagerEndless != null)
            ghostCycleManagerEndless.endlessModeTimeOut = timeOut;
        StartCoroutine(ghostCycleManagerEndless.EndGame());
        FindFirstObjectByType<ScoreManagerEndless>().TriggerEndGame();
    }
}
