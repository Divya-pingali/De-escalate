using System.Collections;
using UnityEngine;
using Meta.XR.BuildingBlocks.AIBlocks;

public class STTCommandManager : MonoBehaviour
{
    [Header("Agents")]
    [SerializeField] private GameObject speechToTextAgent;

    [Header("Pixel Controller")]
    [SerializeField] private RightHandPixelSizeAdjuster rightHandPixelSizeAdjuster;

    [Header("Hey Quest Command")]
    [SerializeField] private GameObject objectToActivateOnHeyQuest;
    [SerializeField] private GameObject objectToDeactivateOnHeyQuest;

    [Header("Listening")]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float restartDelay = 0.5f;

    private SpeechToTextAgent speechToText;

    private void Awake()
    {
        if (speechToTextAgent == null)
        {
            Debug.LogError("SpeechToTextAgent GameObject is not assigned.");
            return;
        }

        speechToText = speechToTextAgent.GetComponent<SpeechToTextAgent>();

        if (speechToText == null)
            Debug.LogError("Missing SpeechToTextAgent");

        if (rightHandPixelSizeAdjuster == null)
            Debug.LogWarning("RightHandPixelSizeAdjuster is not assigned.");

        if (objectToActivateOnHeyQuest == null)
            Debug.LogWarning("Object To Activate On Hey Quest is not assigned.");

        if (objectToDeactivateOnHeyQuest == null)
            Debug.LogWarning("Object To Deactivate On Hey Quest is not assigned.");
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        GetPrompt();
    }

    public void GetPrompt()
    {
        if (speechToText == null)
            return;

        Debug.Log("Starting STT listening...");
        speechToText.StartListening();
    }

    public void PassPrompt(string transcript)
    {
        Debug.Log("Transcript received: " + transcript);

        if (string.IsNullOrWhiteSpace(transcript))
        {
            Debug.LogWarning("Transcript was empty.");
            StartCoroutine(RestartListening());
            return;
        }

        string command = transcript.ToLower();

        if (command.Contains("hey quest"))
        {
            Debug.Log("Command detected: Hey Quest");
            HandleHeyQuestCommand();
            StartCoroutine(RestartListening());
            return;
        }

        if (command.Contains("increase") && command.Contains("pixel"))
        {
            Debug.Log("Command detected: Increase pixel size");

            if (rightHandPixelSizeAdjuster != null)
                rightHandPixelSizeAdjuster.IncreasePixelSize();
        }
        else if (command.Contains("decrease") && command.Contains("pixel"))
        {
            Debug.Log("Command detected: Decrease pixel size");

            if (rightHandPixelSizeAdjuster != null)
                rightHandPixelSizeAdjuster.DecreasePixelSize();
        }
        else
        {
            Debug.Log("No command matched.");
        }

        StartCoroutine(RestartListening());
    }

    private void HandleHeyQuestCommand()
    {
        if (objectToActivateOnHeyQuest != null)
        {
            objectToActivateOnHeyQuest.SetActive(true);
            Debug.Log("Hey Quest object activated.");
        }

        if (objectToDeactivateOnHeyQuest != null)
        {
            objectToDeactivateOnHeyQuest.SetActive(false);
            Debug.Log("Hey Quest object deactivated.");
        }
    }

    private IEnumerator RestartListening()
    {
        yield return new WaitForSeconds(restartDelay);
        GetPrompt();
    }
}