using UnityEngine;

public class BlurStrengthAdjuster : MonoBehaviour
{
    [SerializeField] private Material blurMaterial;

    [SerializeField] private float step = 0.1f;
    [SerializeField] private float minBlurStrength = 0f;
    [SerializeField] private float maxBlurStrength = 1f;

    private static readonly int BlurStrengthId = Shader.PropertyToID("_BlurStrength");

    public void IncreaseBlurStrength()
    {
        if (blurMaterial == null)
            return;

        float blurStrength = blurMaterial.GetFloat(BlurStrengthId);
        blurStrength = Mathf.Clamp(blurStrength + step, minBlurStrength, maxBlurStrength);

        blurMaterial.SetFloat(BlurStrengthId, blurStrength);

        Debug.Log("IncreaseBlurStrength called. Blur strength: " + blurStrength);
    }

    public void DecreaseBlurStrength()
    {
        if (blurMaterial == null)
            return;

        float blurStrength = blurMaterial.GetFloat(BlurStrengthId);
        blurStrength = Mathf.Clamp(blurStrength - step, minBlurStrength, maxBlurStrength);

        blurMaterial.SetFloat(BlurStrengthId, blurStrength);

        Debug.Log("DecreaseBlurStrength called. Blur strength: " + blurStrength);
    }
}