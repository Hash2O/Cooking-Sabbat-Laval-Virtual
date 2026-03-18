using UnityEngine;
using UnityEngine.Playables;

public class TutorialNarrative : MonoBehaviour
{
    public string eventID = "intro_tutorial";
    public PlayableDirector director;

    void Start()
    {
        if (ExplorationProgressManager.ExplorationInstance.IsEventCompleted(eventID))
        {
            return;
        }
        //Debug.Log("Start Narrative Tuto");
        director.Play();

        ExplorationProgressManager.ExplorationInstance.CompleteEvent(eventID);
        //Debug.Log("Narrative Tuto Completed");
    }
}
