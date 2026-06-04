using UnityEngine;

public static class SavedFilterSelection
{
    public enum FilterType
    {
        None,
        Blur,
        Pixelate,
        Block,
        ObjectBlocker
    }

    public static FilterType SelectedFilter = FilterType.None;

    public static Material SelectedMaterial;
    public static GameObject SelectedPrefab;

    public static float BlurStrength = 1f;
    public static float BlurTintStrength = 0f;

    public static float PixelSize = 10f;
    public static float PixelTintStrength = 0f;

    public static float BlockStrength = 1f;

    public static float ObjectBlockerOpacity = 1f;
    public static float ObjectBlockerScaleMultiplier = 1.15f;
}