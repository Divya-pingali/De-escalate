using UnityEngine;

public class RightHandPixelSizeAdjuster : MonoBehaviour
{
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private Material pixelateMaterial;

    [SerializeField] private float step = 0.01f;
    [SerializeField] private float pinchThreshold = 0.75f;
    [SerializeField] private float releaseThreshold = 0.65f;
    [SerializeField] private float minPixelSize = 0.001f;
    [SerializeField] private float maxPixelSize = 0.1f;

    private bool wasMiddlePinching;
    private bool wasIndexPinching;

    private static readonly int PixelSizeId = Shader.PropertyToID("_PixelSize");

    private void Update()
    {
        if (rightHand == null || pixelateMaterial == null || !rightHand.IsTracked)
        {
            wasMiddlePinching = false;
            wasIndexPinching = false;
            return;
        }

        float middleStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        float indexStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        bool middlePinching = middleStrength >= pinchThreshold;
        bool indexPinching = indexStrength >= pinchThreshold;

        bool middleStarted = middlePinching && !wasMiddlePinching;
        bool indexStarted = indexPinching && !wasIndexPinching;

        if (middleStarted)
        {
            IncreasePixelSize();
        }

        if (indexStarted)
        {
            DecreasePixelSize();
        }

        wasMiddlePinching = middleStrength > releaseThreshold;
        wasIndexPinching = indexStrength > releaseThreshold;
    }

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