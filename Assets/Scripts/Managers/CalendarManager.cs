using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Image sunImage;
    [SerializeField] private Image moonImage;
    [SerializeField] private TextMeshProUGUI numberText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SunTime();
        //numberText.text = "";
    }

    public void SunTime()
    {
        sunImage.gameObject.SetActive(true);
        moonImage.gameObject.SetActive(false);
    }

    public void MoonTime()
    {
        sunImage.gameObject.SetActive(false);
        moonImage.gameObject.SetActive(true);
    }

    public void ChangeDay(int day)
    {
        numberText.text = day.ToString();
    }
}
