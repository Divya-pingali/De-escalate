using UnityEngine;
using UnityEngine.SceneManagement;

public class AppNavigationManager : MonoBehaviour
{
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}