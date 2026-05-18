using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PixelateFilterSettingsUI : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material pixelateActualMaterial;
    [SerializeField] private Material pixelatePreviewTemplateMaterial;

    [Header("Prefab")]
    [SerializeField] private GameObject selectedPrefab;

    [Header("Scene")]
    [SerializeField] private string passthroughSceneName = "PassthroughScene";

    [Header("Preview UI")]
    [SerializeField] private RawImage previewRawImage;

    [Header("Sliders")]
    [SerializeField] private Slider pixelSizeSlider;
    [SerializeField] private Slider tintStrengthSlider;

    private Material runtimePreviewMaterial;

    private static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
    private static readonly int TintStrength = Shader.PropertyToID("_TintStrength");
    private static readonly int Tint = Shader.PropertyToID("_Tint");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        CreatePreviewMaterial();
        pixelSizeSlider.onValueChanged.AddListener(OnPixelSizeChanged);
        tintStrengthSlider.onValueChanged.AddListener(OnTintStrengthChanged);
    }

    private void CreatePreviewMaterial()
    {
        if (runtimePreviewMaterial != null)
            Destroy(runtimePreviewMaterial);

        runtimePreviewMaterial = new Material(pixelatePreviewTemplateMaterial);
        runtimePreviewMaterial.SetTexture(MainTex, previewRawImage.texture);

        runtimePreviewMaterial.SetFloat(PixelSize, pixelateActualMaterial.GetFloat(PixelSize));
        runtimePreviewMaterial.SetFloat(TintStrength, pixelateActualMaterial.GetFloat(TintStrength));
        runtimePreviewMaterial.SetColor(Tint, pixelateActualMaterial.GetColor(Tint));

        previewRawImage.material = runtimePreviewMaterial;

        pixelSizeSlider.SetValueWithoutNotify(runtimePreviewMaterial.GetFloat(PixelSize));
        tintStrengthSlider.SetValueWithoutNotify(runtimePreviewMaterial.GetFloat(TintStrength));

        previewRawImage.SetMaterialDirty();
    }

    private void OnPixelSizeChanged(float value)
    {
        if (runtimePreviewMaterial == null) return;

        runtimePreviewMaterial.SetFloat(PixelSize, value);
        previewRawImage.SetMaterialDirty();
    }

    private void OnTintStrengthChanged(float value)
    {
        if (runtimePreviewMaterial == null) return;

        runtimePreviewMaterial.SetFloat(TintStrength, value);
        previewRawImage.SetMaterialDirty();
    }

    public void Save()
    {
        if (runtimePreviewMaterial == null) return;

        pixelateActualMaterial.SetFloat(PixelSize, runtimePreviewMaterial.GetFloat(PixelSize));
        pixelateActualMaterial.SetFloat(TintStrength, runtimePreviewMaterial.GetFloat(TintStrength));
        pixelateActualMaterial.SetColor(Tint, runtimePreviewMaterial.GetColor(Tint));

        SavedFilterSelection.SelectedFilter = SavedFilterSelection.FilterType.Pixelate;
        SavedFilterSelection.SelectedMaterial = pixelateActualMaterial;
        SavedFilterSelection.SelectedPrefab = selectedPrefab;

        SavedFilterSelection.PixelSize = runtimePreviewMaterial.GetFloat(PixelSize);
        SavedFilterSelection.PixelTintStrength = runtimePreviewMaterial.GetFloat(TintStrength);

        SceneManager.LoadScene(passthroughSceneName);
    }

    public void Cancel()
    {
        CreatePreviewMaterial();
        SceneManager.LoadScene(passthroughSceneName);
    }

    private void OnDestroy()
    {
        if (runtimePreviewMaterial != null)
            Destroy(runtimePreviewMaterial);
    }
}