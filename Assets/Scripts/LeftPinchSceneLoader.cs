using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftPinchSceneLoader : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneName = "NewScene";

    [Header("Hand Input")]
    [SerializeField] private OVRHand hand;

    [SerializeField] private OVRHand.HandFinger handActionFinger = OVRHand.HandFinger.Index;

    [SerializeField] private float handActionCooldown = 1.0f;

    private bool wasHandActionActive;
    private float lastHandActionTime;

    private void Update()
    {
        HandleHandInput();
    }

    private void HandleHandInput()
    {
        if (hand == null) return;

        bool isHandActionActive = hand.GetFingerIsPinching(handActionFinger);

        if (
            isHandActionActive &&
            !wasHandActionActive &&
            Time.time - lastHandActionTime > handActionCooldown
        )
        {
            lastHandActionTime = Time.time;
            OpenNewScene();
        }

        wasHandActionActive = isHandActionActive;
    }

    private void OpenNewScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}