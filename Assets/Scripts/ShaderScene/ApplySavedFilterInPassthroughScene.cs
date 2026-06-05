using UnityEngine;
using QuestCameraKit.CameraMapping;
using Meta.XR.BuildingBlocks.AIBlocks;

public class ApplySavedFilterInPassthroughScene : MonoBehaviour
{
    [Header("Passthrough Controllers")]
    [SerializeField] private StereoCameraMappingController stereoCameraMappingController;
    [SerializeField] private ObjectDetectionVisualizer objectDetectionVisualizer;

    private void Start()
    {
        ApplySelectedFilter();
    }

    private void ApplySelectedFilter()
    {
        if (objectDetectionVisualizer == null)
        {
            Debug.LogError("[ApplySavedFilter] ObjectDetectionVisualizer missing.");
            return;
        }

        switch (SavedFilterSelection.SelectedFilter)
        {
            case SavedFilterSelection.FilterType.Blur:
                if (stereoCameraMappingController == null)
                {
                    Debug.LogError("[ApplySavedFilter] StereoCameraMappingController missing.");
                    return;
                }

                stereoCameraMappingController.SetShaderMode(StereoCameraMappingController.ShaderMode.Blur);
                objectDetectionVisualizer.SetShaderMode(ObjectDetectionVisualizer.ShaderMode.Blur);
                break;

            case SavedFilterSelection.FilterType.Pixelate:
                if (stereoCameraMappingController == null)
                {
                    Debug.LogError("[ApplySavedFilter] StereoCameraMappingController missing.");
                    return;
                }

                stereoCameraMappingController.SetShaderMode(StereoCameraMappingController.ShaderMode.Pixelate);
                objectDetectionVisualizer.SetShaderMode(ObjectDetectionVisualizer.ShaderMode.Pixelate);
                break;

            case SavedFilterSelection.FilterType.Block:
                if (stereoCameraMappingController == null)
                {
                    Debug.LogError("[ApplySavedFilter] StereoCameraMappingController missing.");
                    return;
                }

                stereoCameraMappingController.SetShaderMode(StereoCameraMappingController.ShaderMode.Block);
                objectDetectionVisualizer.SetShaderMode(ObjectDetectionVisualizer.ShaderMode.Block);
                break;

            case SavedFilterSelection.FilterType.ObjectBlocker:
                if (SavedFilterSelection.SelectedPrefab == null)
                {
                    Debug.LogError("[ApplySavedFilter] ObjectBlocker selected, but SelectedPrefab is null.");
                    return;
                }

                objectDetectionVisualizer.SetObjectBlockerPrefab(SavedFilterSelection.SelectedPrefab);
                objectDetectionVisualizer.SetObjectBlockerOpacity(SavedFilterSelection.ObjectBlockerOpacity);
                objectDetectionVisualizer.SetObjectBlockerScaleMultiplier(SavedFilterSelection.ObjectBlockerScaleMultiplier);
                break;

            default:
                Debug.LogWarning("[ApplySavedFilter] No saved filter selected.");
                break;
        }
    }
}