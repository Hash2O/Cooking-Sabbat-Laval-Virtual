using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    public Image fillImage;

    /// <summary>
    /// Affiche un pourcentage entre 0 et 1 (1 = plein, 0 = vide)
    /// </summary>
    public void SetFill(float value)
    {
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(value);
    }

    /// <summary>
    /// Active/désactive la barre
    /// </summary>
    public void SetVisible(bool state)
    {
        gameObject.SetActive(state);
    }
}

