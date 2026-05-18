using UnityEngine;

public static class SavedFilterSelection
{
    public enum FilterType
    {
        None,
        Blur,
        Pixelate,
        Block
    }

    public static FilterType SelectedFilter = FilterType.None;

    public static Material SelectedMaterial;
    public static GameObject SelectedPrefab;

    public static float BlurRadius;
    public static float BlurTintStrength;

    public static float PixelSize;
    public static float PixelTintStrength;

    public static float BlockStrength;
}