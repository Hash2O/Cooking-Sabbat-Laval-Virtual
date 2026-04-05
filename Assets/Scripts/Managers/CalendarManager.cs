using UnityEngine;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    [SerializeField] private Renderer calendar;

    [SerializeField] private Material sunMaterial;
    [SerializeField] private Material moonMaterial;

    [SerializeField] private TMP_Text numberText;

    public void SunTime()
    {
        if (calendar != null && sunMaterial != null)
        {
            calendar.material = sunMaterial;
        }
    }

    public void MoonTime()
    {
        if (calendar != null && moonMaterial != null)
        {
            calendar.material = moonMaterial;
        }
    }

    public void ChangeDay(int day)
    {
        if (numberText != null)
        {
            numberText.text = day.ToString();
        }
    }
}