using UnityEngine;

public sealed class FitObjectToBoundingBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetObject;   // the 3D object to scale
    [SerializeField] private Transform boundingBox;    // the bbox quad / box transform

    [Header("Options")]
    [SerializeField] private bool fitOnStart = true;
    [SerializeField] private bool fitEveryFrame = false;
    [SerializeField] private float depthScaleMultiplier = 1f;
    [SerializeField] private bool preserveObjectDepthRatio = true;

    private Renderer[] _renderers;

    private void Awake()
    {
        if (targetObject != null)
            _renderers = targetObject.GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        if (fitOnStart)
            Fit();
    }

    private void LateUpdate()
    {
        if (fitEveryFrame)
            Fit();
    }

    public void Fit()
    {
        if (targetObject == null || boundingBox == null)
            return;

        if (_renderers == null || _renderers.Length == 0)
            _renderers = targetObject.GetComponentsInChildren<Renderer>();

        Bounds objectBounds;
        if (!TryGetRendererBounds(out objectBounds))
            return;

        // Bounding box world size
        Vector3 boxSize = boundingBox.lossyScale;

        // Renderer world size
        Vector3 objectSize = objectBounds.size;

        if (objectSize.x <= 0.0001f || objectSize.y <= 0.0001f || objectSize.z <= 0.0001f)
            return;

        // Scale factors needed to match bbox
        float scaleX = boxSize.x / objectSize.x;
        float scaleY = boxSize.y / objectSize.y;

        Vector3 newScale = targetObject.localScale;

        if (preserveObjectDepthRatio)
        {
            float uniformXY = Mathf.Min(scaleX, scaleY);
            newScale *= uniformXY;
            newScale.z *= depthScaleMultiplier;
        }
        else
        {
            float scaleZ = (boxSize.z / objectSize.z) * depthScaleMultiplier;
            newScale = new Vector3(
                newScale.x * scaleX,
                newScale.y * scaleY,
                newScale.z * scaleZ
            );
        }

        targetObject.localScale = newScale;
        targetObject.position = boundingBox.position;
        targetObject.rotation = boundingBox.rotation;
    }

    private bool TryGetRendererBounds(out Bounds bounds)
    {
        bounds = default;

        if (_renderers == null || _renderers.Length == 0)
            return false;

        bool found = false;
        foreach (Renderer r in _renderers)
        {
            if (r == null || !r.enabled)
                continue;

            if (!found)
            {
                bounds = r.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        return found;
    }
}