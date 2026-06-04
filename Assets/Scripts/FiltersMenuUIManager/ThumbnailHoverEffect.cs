using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThumbnailHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private RectTransform targetToScale;
    [SerializeField] private float hoverScale = 1.15f;
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Border")]
    [SerializeField] private Image borderImage;
    [SerializeField] private Color borderColor = new Color(0.6f, 0.8f, 1f, 1f);
    [SerializeField] private float borderThickness = 6f;

    private Vector3 normalScale;
    private Vector3 targetScale;
    private bool isHovering;

    private void Awake()
    {
        if (targetToScale == null)
            targetToScale = transform as RectTransform;

        normalScale = targetToScale.localScale;
        targetScale = normalScale;

        SetupBorder();
    }

    private void Update()
    {
        targetToScale.localScale = Vector3.Lerp(
            targetToScale.localScale,
            targetScale,
            Time.unscaledDeltaTime * scaleSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = normalScale * hoverScale;

        if (borderImage != null)
        {
            borderImage.enabled = true;
            borderImage.color = borderColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = normalScale;

        if (borderImage != null)
        {
            borderImage.enabled = false;
        }
    }

    private void SetupBorder()
    {
        if (borderImage == null)
        {
            GameObject borderObject = new GameObject("Hover Border");
            borderObject.transform.SetParent(transform, false);
            borderObject.transform.SetAsFirstSibling();

            RectTransform borderRect = borderObject.AddComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(-borderThickness, -borderThickness);
            borderRect.offsetMax = new Vector2(borderThickness, borderThickness);

            borderImage = borderObject.AddComponent<Image>();
        }

        borderImage.color = borderColor;
        borderImage.raycastTarget = false;
        borderImage.enabled = false;
    }
}