using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public Volume postProcessVolume;
    private ColorAdjustments col;
    public float postExposurePenalty;
    public float minPostExposureValue;
    
    void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet<ColorAdjustments>(out col);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DarkenScreen()
    {
        if(col.postExposure.value - postExposurePenalty >= minPostExposureValue)
        {
            col.postExposure.value -= postExposurePenalty;
        }
    }
}
