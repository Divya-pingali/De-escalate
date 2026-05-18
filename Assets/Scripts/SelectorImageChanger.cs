using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorImageChanger : MonoBehaviour
{
    public enum ImageMode
    {
        SpriteImage,
        RawImage
    }

    [Header("Mode")]
    public ImageMode imageMode = ImageMode.SpriteImage;

    [Header("Normal UI Image")]
    public Image targetImage;
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Raw UI Image")]
    public RawImage targetRawImage;
    public List<Texture> textures = new List<Texture>();

    public void ChangeImage(int index)
    {
        if (imageMode == ImageMode.SpriteImage)
        {
            ChangeSpriteImage(index);
        }
        else
        {
            ChangeRawImage(index);
        }
    }

    void ChangeSpriteImage(int index)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("Target Image is missing.", this);
            return;
        }

        if (index < 0 || index >= sprites.Count)
        {
            Debug.LogWarning("Sprite index is out of range: " + index, this);
            return;
        }

        targetImage.sprite = sprites[index];
        targetImage.gameObject.SetActive(sprites[index] != null);
    }

    void ChangeRawImage(int index)
    {
        if (targetRawImage == null)
        {
            Debug.LogWarning("Target RawImage is missing.", this);
            return;
        }

        if (index < 0 || index >= textures.Count)
        {
            Debug.LogWarning("Texture index is out of range: " + index, this);
            return;
        }

        targetRawImage.texture = textures[index];
        targetRawImage.gameObject.SetActive(textures[index] != null);
    }
}