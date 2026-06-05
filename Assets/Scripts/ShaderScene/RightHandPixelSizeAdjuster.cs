using UnityEngine;

public class RightHandPixelSizeAdjuster : MonoBehaviour
{
    [SerializeField] private Material pixelateMaterial;

    [SerializeField] private float step = 0.01f;
    [SerializeField] private float minPixelSize = 0.001f;
    [SerializeField] private float maxPixelSize = 0.1f;

    private static readonly int PixelSizeId = Shader.PropertyToID("_PixelSize");

    public void IncreasePixelSize()
    {
        if (pixelateMaterial == null)
            return;

        float pixelSize = pixelateMaterial.GetFloat(PixelSizeId);
        pixelSize = Mathf.Clamp(pixelSize + step, minPixelSize, maxPixelSize);

        pixelateMaterial.SetFloat(PixelSizeId, pixelSize);

        Debug.Log("IncreasePixelSize called. Pixel size: " + pixelSize);
    }

    public void DecreasePixelSize()
    {
        if (pixelateMaterial == null)
            return;

        float pixelSize = pixelateMaterial.GetFloat(PixelSizeId);
        pixelSize = Mathf.Clamp(pixelSize - step, minPixelSize, maxPixelSize);

        pixelateMaterial.SetFloat(PixelSizeId, pixelSize);

        Debug.Log("DecreasePixelSize called. Pixel size: " + pixelSize);
    }
}