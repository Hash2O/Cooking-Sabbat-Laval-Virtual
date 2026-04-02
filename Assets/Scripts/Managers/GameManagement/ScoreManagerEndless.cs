using UnityEngine;
using TMPro; // Pour les TextMeshPro
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening; // Optionnel : pour faire apparaître les textes en douceur

// Cette petite classe permet de créer des "cases" dans l'inspecteur d'Unity
[System.Serializable]
public class GhostScoreUI
{
    [Tooltip("Le nom exact du fantôme envoyé par le script (ex: AngelGhostClient)")]
    public string ghostName; 
    
    [Tooltip("Le composant TextMeshPro placé à côté du dessin de ce fantôme")]
    public TMP_Text scoreText; 
    
    [HideInInspector] 
    public int count = 0; // Le compteur interne (caché dans l'inspecteur)
}

public class ScoreManagerEndless : MonoBehaviour
{
    public static ScoreManagerEndless instance;

    [Header("Score Global")]
    public TMP_Text textTotalPotions;
    private int totalPotionsServed = 0;

    [Header("Scores des Fantômes")]
    [Tooltip("Ajoute ici tes 13 fantômes avec leur nom exact et leur texte respectif")]
    public List<GhostScoreUI> ghostScores = new List<GhostScoreUI>();

    [Header("Livre de Bilan & Bouton 3D")]
    public Animator bookAnimator; 
    public GameObject waxSealButton; // Le sceau 3D à activer à la fin
    
    [Tooltip("Optionnel : Si tes textes sont dans un Canvas, met le CanvasGroup ici pour un fondu")]
    public CanvasGroup textCanvasGroup; 
    public GameObject bookGameObject;
    public GameObject recipeBookGameObject;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (bookGameObject != null) 
            bookGameObject.SetActive(false);
        // 1. On cache le bouton Sceau au début de la partie
        if (waxSealButton != null) waxSealButton.SetActive(false);
        
        // 2. On vide tous les textes pour ne pas afficher "0" pendant la partie
        if (textTotalPotions != null) textTotalPotions.text = "";
        foreach (var ghost in ghostScores)
        {
            if (ghost.scoreText != null) ghost.scoreText.text = "";
        }

        // 3. On cache les textes si on utilise un CanvasGroup
        if (textCanvasGroup != null) textCanvasGroup.alpha = 0f;
    }

    // Fonction appelée quand un fantôme est servi
    public void AddSuccessfulOrder(string ghostName)
    {
        totalPotionsServed++;
        Debug.Log(ghostName);

        // On cherche le fantôme correspondant dans notre liste
        foreach (var ghost in ghostScores)
        {
            if (ghost.ghostName == ghostName)
            {
                ghost.count++;
                break; // On a trouvé, on arrête de chercher
            }
        }
    }

    // Fonction appelée à la fin du timer
    public void TriggerEndGame()
    {
        recipeBookGameObject.SetActive(false);
        if (bookGameObject != null)
        {
            bookGameObject.SetActive(true); // On allume l'objet 3D
            
            // Optionnel : Un petit effet magique avec DOTween pour qu'il n'apparaisse pas d'un coup sec
            bookGameObject.transform.localScale = Vector3.zero;
            bookGameObject.transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.6f).SetEase(Ease.OutBack);
            
            if (AudioManager.audioInstance != null)
                AudioManager.audioInstance.PlayTheGoodSound(Random.Range(17,20)); // Son d'apparition
        }
        // 1. Mettre à jour les textes avec les valeurs finales
        if (textTotalPotions != null) 
            textTotalPotions.text = totalPotionsServed.ToString();

        foreach (var ghost in ghostScores)
        {
            if (ghost.scoreText != null)
            {
                // Affiche le nombre (ou un petit "-" ou "0" si tu préfères)
                ghost.scoreText.text = ghost.count > 0 ? ghost.count.ToString() : "0"; 
            }
        }

        // 2. Ouvrir le livre (si besoin)
        if (bookAnimator != null)
        {
            bookAnimator.speed = 1f;
            bookAnimator.Play("PageTurnRight", -1, 0f); 
            if (AudioManager.audioInstance != null)
                AudioManager.audioInstance.PlayTheGoodSound(Random.Range(17, 19));
        }

        // 3. Apparition des textes en fondu (si CanvasGroup présent)
        if (textCanvasGroup != null)
        {
            textCanvasGroup.DOFade(1f, 1f).SetDelay(0.5f);
        }

        // 4. Activer le bouton Sceau 3D
        if (waxSealButton != null)
        {
            waxSealButton.SetActive(true);
        }
    }

    // Fonction déclenchée par le Sceau 3D
    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}