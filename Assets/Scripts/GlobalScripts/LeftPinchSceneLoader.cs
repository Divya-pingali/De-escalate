using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftPinchSceneLoader : MonoBehaviour
{
    [Header("Scene 1")]
    [SerializeField] private string sceneName1 = "NewScene";
    [SerializeField] private OVRHand.HandFinger scene1Finger = OVRHand.HandFinger.Index;

    [Header("Scene 2")]
    [SerializeField] private string sceneName2 = "AnotherScene";
    [SerializeField] private OVRHand.HandFinger scene2Finger = OVRHand.HandFinger.Ring;

    [Header("Hand Input")]
    [SerializeField] private OVRHand hand;

    [SerializeField] private float handActionCooldown = 1.0f;

    private bool wasScene1FingerActive;
    private bool wasScene2FingerActive;

    private float lastHandActionTime;

    private void Update()
    {
        HandleHandInput();
    }

    private void HandleHandInput()
    {
        if (hand == null) return;

        bool isScene1FingerActive = hand.GetFingerIsPinching(scene1Finger);
        bool isScene2FingerActive = hand.GetFingerIsPinching(scene2Finger);

        bool cooldownReady = Time.time - lastHandActionTime > handActionCooldown;

        if (
            isScene1FingerActive &&
            !wasScene1FingerActive &&
            cooldownReady
        )
        {
            lastHandActionTime = Time.time;
            OpenScene(sceneName1);
        }

        if (
            isScene2FingerActive &&
            !wasScene2FingerActive &&
            cooldownReady
        )
        {
            lastHandActionTime = Time.time;
            OpenScene(sceneName2);
        }

        wasScene1FingerActive = isScene1FingerActive;
        wasScene2FingerActive = isScene2FingerActive;
    }

    private void OpenScene(string targetSceneName)
    {
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("[LeftPinchSceneLoader] Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}