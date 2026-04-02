using UnityEngine;
using UnityEngine.Playables;

public class TutorialNarrative : MonoBehaviour
{
    public string eventID = "intro_tutorial";
    public PlayableDirector director;

    void Start()
    {
        bool subtitlesEnabled = PlayerPrefs.GetInt("SubtitlesEnabled", 1) == 1;
    
        if (!subtitlesEnabled)
        {
            // Désactiver ta piste de sous-titres ou le GameObject associé ici
        }
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
