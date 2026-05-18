using UnityEngine;

public class VoicePixelCommandController : MonoBehaviour
{
    [Header("Pixel Controller")]
    [SerializeField] private PixelSizeController pixelSizeController;

    public void HandleFullTranscription(string text)
    {
        if (pixelSizeController == null || string.IsNullOrWhiteSpace(text))
            return;

        string command = text.ToLower();

        if (
            command.Contains("increase the pixel size") ||
            command.Contains("increase pixel") ||
            command.Contains("make the pixels bigger") ||
            command.Contains("make it more pixelated") ||
            command.Contains("more pixelated") ||
            command.Contains("pixelate more")
        )
        {
            pixelSizeController.IncreasePixel();
        }
        else if (
            command.Contains("decrease the pixel size") ||
            command.Contains("decrease pixel") ||
            command.Contains("make the pixels smaller") ||
            command.Contains("make it less pixelated") ||
            command.Contains("less pixelated") ||
            command.Contains("pixelate less") ||
            command.Contains("testing") ||
            command.Contains("test")
        )
        {
            pixelSizeController.DecreasePixel();
        }
    }
}