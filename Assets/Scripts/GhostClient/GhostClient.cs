using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostClient : MonoBehaviour
{
    public RecipeData requestedRecipe;

    public PatienceBar patienceBar;

    public bool isSatisfied { get; private set; }

    [Header("Référence vers le comptoir")]
    [SerializeField] private PotionDeliveryCounter deliveryCounter;

    [Header("Apparence")]
    public Renderer ghostRenderer;


    public float colorChangeSpeed = 4f;

    //private bool hasCheckedPotion = false;
    private bool isEvaluatingPotion = false;

    public bool hasReceivedCorrectPotion = false;
    public GhostCycleManager manager;
    public GhostCycleManagerEndless managerEndless;


    private void Start()
    {
        deliveryCounter = FindFirstObjectByType<PotionDeliveryCounter>();
        manager = FindFirstObjectByType<GhostCycleManager>();
        managerEndless = FindFirstObjectByType<GhostCycleManagerEndless>();
    }

    private void Update()
    {
        // Le fantôme vérifie la potion une seule fois lorsqu'il attend
        if (!hasReceivedCorrectPotion && deliveryCounter != null)
        {
            PotionBottle bottle = deliveryCounter.GetCurrentBottle();

            if (bottle != null)
            {
                StartCoroutine(ReceivePotion(3, bottle));
                Destroy(bottle.gameObject, 3f);
            }
        }
    }

    //public IEnumerator ReceivePotion(int time, PotionBottle bottle)
    //{
    //    if (bottle == null || bottle.GetContainedRecipe() == null)
    //    {
    //        Debug.Log("Le client reçoit une fiole vide !");
    //        yield return null;
    //    }

    //    RecipeData received = bottle.GetContainedRecipe();

    //    if (received == requestedRecipe)
    //    {
    //        yield return new WaitForSeconds(time);
    //        hasReceivedCorrectPotion = true;
    //        Debug.Log($"Le client est ravi ! Potion correcte : {received.recipeName}");
    //        isSatisfied = true;

    //        StartCoroutine(ChangeGhostColor(received.potionColor));
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(time);
    //        Debug.Log($"Mauvaise potion : {received.recipeName} au lieu de {requestedRecipe.recipeName}");
    //        isSatisfied = false;

    //        // Application pénalité de temps en cas de mauvaise potion reçue
    //        if (manager != null)
    //        {
    //            manager.ApplyWrongPotionPenalty();
    //        }

    //        StartCoroutine(ChangeGhostColor(Color.black));
    //        if (AudioManager.audioInstance != null) AudioManager.audioInstance.PlayTheGoodSound(9);  // Fantasy Monster Grunt
    //    }
    //}

    public IEnumerator ReceivePotion(float time, PotionBottle bottle)
    {
        if (isEvaluatingPotion)
            yield break;
        isEvaluatingPotion = true;

        if (bottle == null || bottle.GetContainedRecipe() == null)
        {
            isEvaluatingPotion = false;
            yield break;
        }
        
        RecipeData received = bottle.GetContainedRecipe();

        yield return new WaitForSeconds(time);

        if (received == requestedRecipe)
        {
            hasReceivedCorrectPotion = true;
            isSatisfied = true;

            Debug.Log($"✅ Potion correcte : {received.recipeName}");
            StartCoroutine(ChangeGhostColor(received.potionColor));
        }
        else
        {
            Debug.Log($"❌ Mauvaise potion : {received.recipeName}");
            isSatisfied = false;

            if (manager != null)
                manager.ApplyWrongPotionPenalty();
            if (managerEndless != null)
                managerEndless.ApplyWrongPotionPenalty();

            StartCoroutine(ChangeGhostColor(Color.white));

            if (AudioManager.audioInstance != null)
                AudioManager.audioInstance.PlayTheGoodSound(9);
        }

        // 🔓 Autorise une nouvelle potion APRÈS traitement
        isEvaluatingPotion = false;
    }

    private IEnumerator ChangeGhostColor(Color targetColor)
    {
        // 1) Récupérer le renderer s'il n'est pas assigné (child search safe fallback)
        if (ghostRenderer == null)
        {
            ghostRenderer = GetComponentInChildren<MeshRenderer>();
            if (ghostRenderer == null)
            {
                Debug.LogWarning("GhostClient : pas de MeshRenderer trouvé (ghostRenderer null).");
                yield break;
            }
        }

        foreach(Material mat in ghostRenderer.materials)
                    {
                        if ( mat.name == "Ghost (Instance)" ) 
                        {
                            mat.SetColor("_MainColor",Color.Lerp(ghostRenderer.material.GetColor("_MainColor"), targetColor, 1f));
                        }
                    }

        // Renderer rend = ghostRenderer; // MeshRenderer hérite de Renderer
        // Material[] mats = rend.materials; // clones (évite d'altérer sharedMaterial)

        // // 2) Liste de noms possibles pour la propriété couleur (essaye plusieurs variantes)
        // string[] candidateColorProps = new[] { "_MainColor", "_BaseColor", "_Color", "MainColor" };
        // string colorProp = null;

        // // trouver une propriété existante sur le premier matériau
        // Material sample = mats.Length > 0 ? mats[0] : null;
        // if (sample == null)
        // {
        //     Debug.LogWarning("GhostClient : pas de matériau sur le renderer.");
        //     yield break;
        // }

        // foreach (var p in candidateColorProps)
        // {
        //     if (sample.HasProperty(p))
        //     {
        //         colorProp = p;
        //         break;
        //     }
        // }

        // // fallback si aucune propriété trouvée : essaye d'utiliser mat.color via "_Color"
        // if (colorProp == null)
        // {
        //     if (sample.HasProperty("_Color"))
        //         colorProp = "_Color";
        //     else
        //     {
        //         Debug.LogWarning("GhostClient : aucune propriété couleur connue trouvée sur le shader. Tentative d'utilisation de material.color.");
        //     }
        // }

        // // 3) Vérifier si le shader possède une propriété alpha séparée
        // bool hasAlphaProp = sample.HasProperty("_Alpha") || sample.HasProperty("Alpha");
        // string alphaPropName = sample.HasProperty("_Alpha") ? "_Alpha" : (sample.HasProperty("Alpha") ? "Alpha" : null);

        // // 4) Lire les couleurs de départ (par matériau si nécessaire)
        // Color[] startColors = new Color[mats.Length];
        // float[] startAlphas = new float[mats.Length];

        // for (int i = 0; i < mats.Length; i++)
        // {
        //     Material m = mats[i];

        //     if (!string.IsNullOrEmpty(colorProp) && m.HasProperty(colorProp))
        //         startColors[i] = m.GetColor(colorProp);
        //     else
        //         startColors[i] = m.color; // fallback

        //     if (hasAlphaProp && m.HasProperty(alphaPropName))
        //         startAlphas[i] = m.GetFloat(alphaPropName);
        //     else
        //         startAlphas[i] = startColors[i].a;
        // }

        // // 5) Lerp sur la durée
        // float t = 0f;
        // while (t < 1f)
        // {
        //     t += Time.deltaTime * colorChangeSpeed;
        //     float clamped = Mathf.Clamp01(t);

        //     for (int i = 0; i < mats.Length; i++)
        //     {
        //         Material m = mats[i];
        //         Color c = Color.Lerp(startColors[i], targetColor, clamped);

        //         if (!string.IsNullOrEmpty(colorProp) && m.HasProperty(colorProp))
        //         {
        //             m.SetColor(colorProp, c);
        //         }
        //         else
        //         {
        //             m.color = c; // fallback
        //         }

        //         // gérer alpha séparément si la propriété existe
        //         if (hasAlphaProp && m.HasProperty(alphaPropName))
        //         {
        //             float a = Mathf.Lerp(startAlphas[i], targetColor.a, clamped);
        //             m.SetFloat(alphaPropName, a);
        //         }
        //     }

        //     // appliquer les matériaux modifiés au renderer (optionnel, mais rend explicite)
        //     rend.materials = mats;

        //     yield return null;
        // }

        // // s'assurer des valeurs finales
        // for (int i = 0; i < mats.Length; i++)
        // {
        //     Material m = mats[i];
        //     if (!string.IsNullOrEmpty(colorProp) && m.HasProperty(colorProp))
        //         m.SetColor(colorProp, targetColor);
        //     else
        //         m.color = targetColor;

        //     if (hasAlphaProp && m.HasProperty(alphaPropName))
        //         m.SetFloat(alphaPropName, targetColor.a);
        // }

        // rend.materials = mats;
    }

    public void TriggerReceivePotion(float time, PotionBottle bottle)
    {
        StartCoroutine(ReceivePotion(time, bottle)); 
    }
}