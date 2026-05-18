using UnityEngine;

public class PixelSizeController : MonoBehaviour
{
    [SerializeField] private Material pixelateMaterial;

    [SerializeField] private float step = 0.01f;
    [SerializeField] private float minPixelSize = 0.001f;
    [SerializeField] private float maxPixelSize = 0.1f;

    [Header("Temporary Test Object")]
    [SerializeField] private GameObject objectToShow;

    private static readonly int PixelSizeId = Shader.PropertyToID("_PixelSize");

    public void IncreasePixel()
    {
        if (pixelateMaterial == null)
            return;

        float pixelSize = pixelateMaterial.GetFloat(PixelSizeId);
        pixelSize = Mathf.Clamp(pixelSize + step, minPixelSize, maxPixelSize);

        pixelateMaterial.SetFloat(PixelSizeId, pixelSize);
    }

    public void DecreasePixel()
    {
        ShowObject();
        //if (pixelateMaterial == null)
        //      return;

        //float pixelSize = pixelateMaterial.GetFloat(PixelSizeId);
        //pixelSize = Mathf.Clamp(pixelSize - step, minPixelSize, maxPixelSize);

        //pixelateMaterial.SetFloat(PixelSizeId, pixelSize);

    }

    public void ShowObject()
    {
        if (objectToShow == null)
        {
            Debug.LogWarning("Object to show is not assigned.");
            return;
        }

        objectToShow.SetActive(true);
        Debug.Log("ShowObject called.");
    }
}