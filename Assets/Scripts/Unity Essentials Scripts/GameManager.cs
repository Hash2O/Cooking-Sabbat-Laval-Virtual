//using UnityEngine;
//using UnityEngine.SceneManagement;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//public class GameManager : MonoBehaviour
//{
//    //public static GameManager Instance { get; private set; }

//    //private void Awake()
//    //{
//    //    if (Instance != null)
//    //    {
//    //        Destroy(gameObject);
//    //        return;
//    //    }

//    //    // this : the current instance of MainManager.
//    //    Instance = this;
//    //    DontDestroyOnLoad(gameObject);
//    //}

//    public void ReloadScene()
//    {
//        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//    }

//    public void LoadNextScene()
//    {
//        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

//        // Calcule l’index de la scène suivante
//        int nextSceneIndex = currentSceneIndex + 1;

//        // Si on dépasse, on reboucle à 0
//        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
//        {
//            nextSceneIndex = 0;
//        }

//        // Charge la scène correspondante
//        SceneManager.LoadScene(nextSceneIndex);
//    }

//    public void Exit()
//    {
//#if UNITY_EDITOR
//        EditorApplication.ExitPlaymode();
//#else
//        Application.Quit();     //Original code to quit Unity player
//#endif
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GMInstance { get; private set; }

    private FadeCanvas fadeCanvas;

    [Header("Transition Settings")]
    [Tooltip("Durée du fade avant le chargement d'une nouvelle scène")]
    public float fadeOutDuration = 3.0f;

    [Tooltip("Durée du fade après le chargement d'une nouvelle scène")]
    public float fadeInDuration = 3.0f;

    [Header("References")]
    //public Button loadGameButton;
    //public Button clearSavedDataButton;
    
    // 
    public bool loadSavedData;

    private void Awake()
    {
        if (GMInstance != null && GMInstance != this)
        {
            // Si on revient à la scène d'intro, on garde le GameManager de la scène
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Destroy(GMInstance.gameObject); // détruire l'ancien
                GMInstance = this; // et le remplacer
            }
            else
            {
                Destroy(gameObject); // sinon détruire le nouveau
                return;
            }
        }

        GMInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeCanvas = FindFirstObjectByType<FadeCanvas>();
        if (fadeCanvas != null)
            fadeCanvas.StartFadeOut(); // Assure que l'écran commence visible puis s'efface

        // Init buttons if save file exist
        bool saveFileExist = System.IO.File.Exists(Application.persistentDataPath + "/SavedData.json");
        //loadGameButton.interactable = saveFileExist;
        //clearSavedDataButton.interactable = saveFileExist;
    }

    /// <summary>
    /// Charge la scène suivante dans la build settings (à associer à un bouton UI)
    /// </summary>
    public void LoadNextScene()
    {
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        StartCoroutine(LoadSceneWithFade(nextSceneIndex));
    }

    /// <summary>
    /// Recharge la scène actuelle (utile pour un bouton "Recommencer")
    /// </summary>
    public void ReloadCurrentScene()
    {
        StartCoroutine(LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    /// Charge une scène spécifique via son index (peut être appelée par un bouton)
    /// </summary>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Indice de scène invalide !");
            return;
        }

        StartCoroutine(LoadSceneWithFade(sceneIndex));
    }

    /// <summary>
    /// Coroutine générique pour gérer le fade et le chargement
    /// </summary>
    private IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        if (fadeCanvas == null)
            fadeCanvas = FindFirstObjectByType<FadeCanvas>();

        if (fadeCanvas != null)
        {
            fadeCanvas.StartFadeIn(); // Fade vers noir
            yield return new WaitForSeconds(fadeOutDuration);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
            yield return null;

        // Attendre un frame pour être sûr que la scène est bien chargée
        yield return null;

        fadeCanvas = FindFirstObjectByType<FadeCanvas>();
        if (fadeCanvas != null)
        {
            fadeCanvas.StartFadeOut(); // Réapparition douce
            yield return new WaitForSeconds(fadeInDuration);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
