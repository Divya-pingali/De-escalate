using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public void SwitchWindow(GameObject windowToHide, GameObject windowToShow)
    {
        if (windowToHide != null)
            windowToHide.SetActive(false);

        if (windowToShow != null)
            windowToShow.SetActive(true);
    }

    public void ShowWindow(GameObject windowToShow)
    {
        if (windowToShow != null)
            windowToShow.SetActive(true);
    }

    public void HideWindow(GameObject windowToHide)
    {
        if (windowToHide != null)
            windowToHide.SetActive(false);
    }
}