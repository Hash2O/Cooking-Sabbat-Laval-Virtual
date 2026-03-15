using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class HandGestureManager : MonoBehaviour
{

    private string spell = "";
    private int counter = 0;
    private AudioSource audioSource;

    public TextMeshProUGUI spellText;
    public GameObject fireEffects;

    private bool isSpellReady = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        if(counter == 3 && spell == "Zeno Acta Gamat")
        {
            spell = "";
            counter = 0;
            audioSource.Play();
            isSpellReady = true;
        }
        else if (counter >= 3)
        {
            spell = "";
            counter = 0;
            isSpellReady=false;
        }

        if (isSpellReady)
        {
            fireEffects.gameObject.SetActive(true);
        }
        else
        {
            fireEffects.gameObject.SetActive(false);
        }
    }

    public void AddPalmUpPose()
    {
        counter++;
        spell += "Zeno" + " ";
        spellText.text = spell;
    }

    public void AddFistBumpPose()
    {
        counter++;
        spell += "Acta" + " ";
        spellText.text = spell;
    }

    public void AddShakaPose()
    {
        counter++;
        spell += "Gamat";
        spellText.text = spell;
    }
}
