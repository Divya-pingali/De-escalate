using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlockFilterSettingsUI : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material blockActualMaterial;
    [SerializeField] private Material blockPreviewTemplateMaterial;

    [Header("Prefab")]
    [SerializeField] private GameObject selectedPrefab;

    [Header("Scene")]
    [SerializeField] private string passthroughSceneName = "PassthroughScene";

    [Header("Preview UI")]
    [SerializeField] private RawImage previewRawImage;

    [Header("Sliders")]
    [SerializeField] private Slider blockStrengthSlider;

    private Material runtimePreviewMaterial;

    private static readonly int BlockStrength = Shader.PropertyToID("_BlockStrength");
    private static readonly int BlockColor = Shader.PropertyToID("_BlockColor");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        CreatePreviewMaterial();

        blockStrengthSlider.onValueChanged.AddListener(OnBlockStrengthChanged);
    }

    private void CreatePreviewMaterial()
    {
        if (runtimePreviewMaterial != null)
            Destroy(runtimePreviewMaterial);

        runtimePreviewMaterial = new Material(blockPreviewTemplateMaterial);
        runtimePreviewMaterial.SetTexture(MainTex, previewRawImage.texture);

        runtimePreviewMaterial.SetFloat(BlockStrength, blockActualMaterial.GetFloat(BlockStrength));
        runtimePreviewMaterial.SetColor(BlockColor, blockActualMaterial.GetColor(BlockColor));

        previewRawImage.material = runtimePreviewMaterial;

        blockStrengthSlider.SetValueWithoutNotify(runtimePreviewMaterial.GetFloat(BlockStrength));

        previewRawImage.SetMaterialDirty();
    }

    private void OnBlockStrengthChanged(float value)
    {
        if (runtimePreviewMaterial == null) return;

        runtimePreviewMaterial.SetFloat(BlockStrength, value);
        previewRawImage.SetMaterialDirty();
    }

    public void Save()
    {
        if (runtimePreviewMaterial == null) return;

        blockActualMaterial.SetFloat(BlockStrength, runtimePreviewMaterial.GetFloat(BlockStrength));
        blockActualMaterial.SetColor(BlockColor, runtimePreviewMaterial.GetColor(BlockColor));

        SavedFilterSelection.SelectedFilter = SavedFilterSelection.FilterType.Block;
        SavedFilterSelection.SelectedMaterial = blockActualMaterial;
        SavedFilterSelection.SelectedPrefab = selectedPrefab;

        SavedFilterSelection.BlockStrength = runtimePreviewMaterial.GetFloat(BlockStrength);

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