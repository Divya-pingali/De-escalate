using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectBlockerFilterSettingsUI : MonoBehaviour
{
    [System.Serializable]
    public class ObjectBlockerOption
    {
        public string displayName;
        public GameObject prefab;
        public Texture previewTexture;
    }

    [Header("Object Blocker Options")]
    [SerializeField] private ObjectBlockerOption[] options;

    [Header("Scene")]
    [SerializeField] private string passthroughSceneName = "ShaderScene";

    [Header("Preview UI")]
    [SerializeField] private RawImage mainPreviewRawImage;
    [SerializeField] private Text selectedNameText;

    [Header("Selection Defaults")]
    [SerializeField] private int defaultSelectedIndex = 0;

    [Header("Object Blocker Settings")]
    [SerializeField] private float objectBlockerOpacity = 1f;
    [SerializeField] private float objectBlockerScaleMultiplier = 1.15f;

    private int selectedIndex = -1;
    private GameObject selectedPrefab;

    private void Start()
    {
        if (options == null || options.Length == 0)
        {
            Debug.LogError("[ObjectBlockerFilterSettingsUI] No object blocker options assigned.");
            return;
        }

        int safeIndex = Mathf.Clamp(defaultSelectedIndex, 0, options.Length - 1);
        SelectOption(safeIndex);
    }

    public void SelectOption(int index)
    {
        if (options == null || options.Length == 0)
        {
            Debug.LogError("[ObjectBlockerFilterSettingsUI] No options assigned.");
            return;
        }

        if (index < 0 || index >= options.Length)
        {
            Debug.LogError("[ObjectBlockerFilterSettingsUI] Option index out of range: " + index);
            return;
        }

        ObjectBlockerOption option = options[index];

        if (option == null)
        {
            Debug.LogError("[ObjectBlockerFilterSettingsUI] Option is null at index: " + index);
            return;
        }

        if (option.prefab == null)
        {
            Debug.LogError("[ObjectBlockerFilterSettingsUI] Prefab missing for option: " + index);
            return;
        }

        selectedIndex = index;
        selectedPrefab = option.prefab;

        if (mainPreviewRawImage != null)
        {
            mainPreviewRawImage.texture = option.previewTexture;
            mainPreviewRawImage.SetAllDirty();
        }

        if (selectedNameText != null)
        {
            selectedNameText.text = string.IsNullOrWhiteSpace(option.displayName)
                ? option.prefab.name
                : option.displayName;
        }

        Debug.Log("[ObjectBlockerFilterSettingsUI] Selected: " + selectedPrefab.name);
    }

    public void Save()
    {
        if (selectedPrefab == null)
        {
            Debug.LogError("[ObjectBlockerFilterSettingsUI] Cannot save. No prefab selected.");
            return;
        }

        SavedFilterSelection.SelectedFilter = SavedFilterSelection.FilterType.ObjectBlocker;
        SavedFilterSelection.SelectedMaterial = null;
        SavedFilterSelection.SelectedPrefab = selectedPrefab;

        SavedFilterSelection.ObjectBlockerOpacity = objectBlockerOpacity;
        SavedFilterSelection.ObjectBlockerScaleMultiplier = objectBlockerScaleMultiplier;

        Debug.Log("[ObjectBlockerFilterSettingsUI] Saved object blocker prefab: " + selectedPrefab.name);

        SceneManager.LoadScene(passthroughSceneName);
    }

    public void Cancel()
    {
        SceneManager.LoadScene(passthroughSceneName);
    }

    // Optional helper methods if you prefer not to type an int in the Button OnClick.
    public void SelectOption0() => SelectOption(0);
    public void SelectOption1() => SelectOption(1);
    public void SelectOption2() => SelectOption(2);
    public void SelectOption3() => SelectOption(3);
    public void SelectOption4() => SelectOption(4);
    public void SelectOption5() => SelectOption(5);
}