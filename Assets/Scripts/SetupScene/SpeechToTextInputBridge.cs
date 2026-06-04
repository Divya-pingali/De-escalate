using System.Collections;
using TMPro;
using UnityEngine;
using Meta.XR.BuildingBlocks.AIBlocks;

public class SpeechToTextMultiInputBridge : MonoBehaviour
{
    [System.Serializable]
    public class SpeechInputTarget
    {
        public string label;
        public TMP_InputField inputField;
        public GameObject listeningIndicator;

        [HideInInspector] public string latestText;
    }

    [Header("Speech To Text")]
    [SerializeField] private SpeechToTextAgent speechToTextAgent;

    [Header("Input Targets")]
    [SerializeField] private SpeechInputTarget[] targets;

    [Header("Settings")]
    [SerializeField] private bool addSpaceBetweenTranscripts = true;

    private bool isListening;
    private int activeTargetIndex = -1;
    private Coroutine restartCoroutine;

    private void Awake()
    {
        HideAllListeningIndicators();

        if (targets != null)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                int capturedIndex = i;

                if (targets[i] != null && targets[i].inputField != null)
                {
                    targets[i].latestText = targets[i].inputField.text;

                    targets[i].inputField.onValueChanged.AddListener(value =>
                    {
                        targets[capturedIndex].latestText = value;
                    });
                }
            }
        }

        Debug.Log("SpeechToTextMultiInputBridge loaded.");
    }

    public void ToggleMicForTarget(int targetIndex)
    {
        Debug.Log("Mic button clicked for target index: " + targetIndex);

        if (speechToTextAgent == null)
        {
            Debug.LogError("SpeechToTextAgent is not assigned.");
            return;
        }

        if (!IsValidTargetIndex(targetIndex))
        {
            Debug.LogError("Invalid target index: " + targetIndex);
            return;
        }

        if (targets[targetIndex].inputField == null)
        {
            Debug.LogError("Input field is not assigned for target index: " + targetIndex);
            return;
        }

        if (isListening && activeTargetIndex == targetIndex)
        {
            StopListening();
            return;
        }

        if (isListening)
        {
            StopListening();
        }

        StartListeningForTarget(targetIndex);
    }

    public void StartListeningForTarget(int targetIndex)
    {
        if (speechToTextAgent == null)
            return;

        if (!IsValidTargetIndex(targetIndex))
            return;

        if (targets[targetIndex].inputField == null)
            return;

        Debug.Log("Starting Speech To Text for target index: " + targetIndex);

        activeTargetIndex = targetIndex;
        isListening = true;

        targets[targetIndex].latestText = targets[targetIndex].inputField.text;

        HideAllListeningIndicators();

        if (targets[targetIndex].listeningIndicator != null)
            targets[targetIndex].listeningIndicator.SetActive(true);

        speechToTextAgent.ClearLastTranscript();
        speechToTextAgent.StartListening();
    }

    public void StopListening()
    {
        if (speechToTextAgent == null)
            return;

        Debug.Log("Stopping Speech To Text...");

        isListening = false;
        activeTargetIndex = -1;

        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine);
            restartCoroutine = null;
        }

        speechToTextAgent.StopNow();

        HideAllListeningIndicators();
    }

    public void HandleTranscript(string transcript)
    {
        Debug.Log("Transcript received: " + transcript);

        if (!isListening || activeTargetIndex < 0)
        {
            Debug.LogWarning("Transcript received, but no active input target is selected.");
            return;
        }

        if (!IsValidTargetIndex(activeTargetIndex))
        {
            Debug.LogError("Active target index is invalid: " + activeTargetIndex);
            return;
        }

        TMP_InputField activeInputField = targets[activeTargetIndex].inputField;

        if (activeInputField == null)
        {
            Debug.LogError("Active input field is missing.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(transcript))
        {
            AppendTranscriptToInput(activeTargetIndex, transcript.Trim());
        }
        else
        {
            Debug.LogWarning("Transcript was empty.");
        }

        if (isListening)
        {
            if (restartCoroutine != null)
                StopCoroutine(restartCoroutine);

            restartCoroutine = StartCoroutine(RestartListeningAfterTranscript());
        }
    }

    private void AppendTranscriptToInput(int targetIndex, string transcript)
    {
        SpeechInputTarget target = targets[targetIndex];
        TMP_InputField targetInputField = target.inputField;

        string currentText = target.latestText;

        string newText;

        if (string.IsNullOrWhiteSpace(currentText))
        {
            newText = transcript;
        }
        else
        {
            string separator = addSpaceBetweenTranscripts ? " " : "";
            newText = currentText.TrimEnd() + separator + transcript;
        }

        target.latestText = newText;

        targetInputField.text = newText;
        targetInputField.caretPosition = targetInputField.text.Length;
        targetInputField.stringPosition = targetInputField.text.Length;
        targetInputField.ForceLabelUpdate();
    }

    private IEnumerator RestartListeningAfterTranscript()
    {
        yield return null;

        if (!isListening || activeTargetIndex < 0)
            yield break;

        Debug.Log("Restarting Speech To Text for target index: " + activeTargetIndex);

        speechToTextAgent.ClearLastTranscript();
        speechToTextAgent.StartListening();

        restartCoroutine = null;
    }

    private bool IsValidTargetIndex(int index)
    {
        return targets != null && index >= 0 && index < targets.Length;
    }

    private void HideAllListeningIndicators()
    {
        if (targets == null)
            return;

        foreach (SpeechInputTarget target in targets)
        {
            if (target != null && target.listeningIndicator != null)
                target.listeningIndicator.SetActive(false);
        }
    }

}