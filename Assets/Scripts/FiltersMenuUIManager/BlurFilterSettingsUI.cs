using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlurFilterSettingsUI : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material blurActualMaterial;
    [SerializeField] private Material blurPreviewTemplateMaterial;

    [Header("Prefab")]
    [SerializeField] private GameObject selectedPrefab;

    [Header("Scene")]
    [SerializeField] private string passthroughSceneName = "PassthroughScene";

    [Header("Preview UI")]
    [SerializeField] private RawImage previewRawImage;

    [Header("Sliders")]
    [SerializeField] private Slider blurStrengthSlider;
    [SerializeField] private Slider tintStrengthSlider;

    private Material runtimePreviewMaterial;

    private const float PreviewBlurRadius = 0.02f;
    private const float MaxTintStrength = 0.02f;

    private static readonly int BlurRadius = Shader.PropertyToID("_BlurRadius");
    private static readonly int BlurStrength = Shader.PropertyToID("_BlurStrength");
    private static readonly int TintStrength = Shader.PropertyToID("_TintStrength");
    private static readonly int Tint = Shader.PropertyToID("_Tint");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        CreatePreviewMaterial();

        blurStrengthSlider.onValueChanged.AddListener(OnBlurStrengthChanged);
        tintStrengthSlider.onValueChanged.AddListener(OnTintStrengthChanged);
    }

    private void CreatePreviewMaterial()
    {
        if (runtimePreviewMaterial != null)
            Destroy(runtimePreviewMaterial);

        runtimePreviewMaterial = new Material(blurPreviewTemplateMaterial);
        runtimePreviewMaterial.SetTexture(MainTex, previewRawImage.texture);

        runtimePreviewMaterial.SetFloat(BlurRadius, PreviewBlurRadius);

        runtimePreviewMaterial.SetFloat(BlurStrength, blurActualMaterial.GetFloat(BlurStrength));
        runtimePreviewMaterial.SetFloat(TintStrength, blurActualMaterial.GetFloat(TintStrength));
        runtimePreviewMaterial.SetColor(Tint, blurActualMaterial.GetColor(Tint));

        previewRawImage.material = runtimePreviewMaterial;

        blurStrengthSlider.SetValueWithoutNotify(runtimePreviewMaterial.GetFloat(BlurStrength));

        float actualTintStrength = runtimePreviewMaterial.GetFloat(TintStrength);
        tintStrengthSlider.SetValueWithoutNotify(actualTintStrength / MaxTintStrength);

        previewRawImage.SetMaterialDirty();
    }

    private void OnBlurStrengthChanged(float value)
    {
        if (runtimePreviewMaterial == null) return;

        runtimePreviewMaterial.SetFloat(BlurStrength, value);
        previewRawImage.SetMaterialDirty();
    }

    private void OnTintStrengthChanged(float sliderValue)
    {
        if (runtimePreviewMaterial == null) return;

        float actualTintStrength = sliderValue * MaxTintStrength;
        runtimePreviewMaterial.SetFloat(TintStrength, actualTintStrength);

        previewRawImage.SetMaterialDirty();
    }

    public void Save()
    {
        if (runtimePreviewMaterial == null) return;

        blurActualMaterial.SetFloat(BlurStrength, runtimePreviewMaterial.GetFloat(BlurStrength));
        blurActualMaterial.SetFloat(TintStrength, runtimePreviewMaterial.GetFloat(TintStrength));
        blurActualMaterial.SetColor(Tint, runtimePreviewMaterial.GetColor(Tint));

        SavedFilterSelection.SelectedFilter = SavedFilterSelection.FilterType.Blur;
        SavedFilterSelection.SelectedMaterial = blurActualMaterial;
        SavedFilterSelection.SelectedPrefab = selectedPrefab;

        SavedFilterSelection.BlurStrength = runtimePreviewMaterial.GetFloat(BlurStrength);
        SavedFilterSelection.BlurTintStrength = runtimePreviewMaterial.GetFloat(TintStrength);

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