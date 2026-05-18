using UnityEngine;

public class FilterUISelector : MonoBehaviour
{
    [System.Serializable]
    public class FilterUIItem
    {
        public string itemName;
        public GameObject[] objectsToActivate;
    }

    [Header("All Possible UI Objects")]
    [SerializeField] private GameObject[] allFilterUIObjects;

    [Header("Per Item UI Settings")]
    [SerializeField] private FilterUIItem[] filterItems;

    public void SelectItem(int index)
    {
        foreach (GameObject obj in allFilterUIObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (index < 0 || index >= filterItems.Length)
            return;

        foreach (GameObject obj in filterItems[index].objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
}