using UnityEngine;

public class ToggleObjectActive : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject targetObject;

    public void Toggle()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("[ToggleObjectActive] Target object is not assigned.");
            return;
        }

        bool newState = !targetObject.activeSelf;
        targetObject.SetActive(newState);

        Debug.Log("[ToggleObjectActive] " + targetObject.name + " active = " + newState);
    }

    public void Show()
    {
        if (targetObject == null) return;
        targetObject.SetActive(true);
    }

    public void Hide()
    {
        if (targetObject == null) return;
        targetObject.SetActive(false);
    }
}