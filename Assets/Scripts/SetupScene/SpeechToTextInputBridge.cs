using System.Collections;
using TMPro;
using UnityEngine;
using Meta.XR.BuildingBlocks.AIBlocks;

public class SpeechToTextInputBridge : MonoBehaviour
{
    [Header("Speech To Text")]
    [SerializeField] private SpeechToTextAgent speechToTextAgent;

    [Header("UI")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject listeningIndicator;

    [Header("Settings")]
    [SerializeField] private bool addSpaceBetweenTranscripts = true;

    private bool isListening;
    private Coroutine restartCoroutine;

    private void Awake()
    {
        if (listeningIndicator != null)
            listeningIndicator.SetActive(false);

        Debug.Log("SpeechToTextInputBridge loaded.");
    }

    // Mic button OnClick should call this
    public void ToggleMic()
    {
        Debug.Log("Mic button clicked.");

        if (speechToTextAgent == null)
        {
            Debug.LogError("SpeechToTextAgent is not assigned.");
            return;
        }

        if (inputField == null)
        {
            Debug.LogError("Input field is not assigned.");
            return;
        }

        if (isListening)
            StopListening();
        else
            StartListening();
    }

    public void StartListening()
    {
        if (speechToTextAgent == null)
            return;

        Debug.Log("Starting Speech To Text...");

        isListening = true;

        if (listeningIndicator != null)
            listeningIndicator.SetActive(true);

        speechToTextAgent.ClearLastTranscript();
        speechToTextAgent.StartListening();
    }

    public void StopListening()
    {
        if (speechToTextAgent == null)
            return;

        Debug.Log("Stopping Speech To Text...");

        isListening = false;

        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine);
            restartCoroutine = null;
        }

        speechToTextAgent.StopNow();

        if (listeningIndicator != null)
            listeningIndicator.SetActive(false);
    }

    // Connect SpeechToTextAgent.onTranscript(string) to this
    public void HandleTranscript(string transcript)
    {
        Debug.Log("Transcript received: " + transcript);

        if (inputField == null)
        {
            Debug.LogError("Input field is missing.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(transcript))
        {
            AppendTranscriptToInput(transcript.Trim());
        }
        else
        {
            Debug.LogWarning("Transcript was empty.");
        }

        // Keep listening until the mic is toggled off
        if (isListening)
        {
            if (restartCoroutine != null)
                StopCoroutine(restartCoroutine);

            restartCoroutine = StartCoroutine(RestartListeningAfterTranscript());
        }
    }

    private void AppendTranscriptToInput(string transcript)
    {
        string currentText = inputField.text;

        if (string.IsNullOrWhiteSpace(currentText))
        {
            inputField.SetTextWithoutNotify(transcript);
        }
        else
        {
            string separator = addSpaceBetweenTranscripts ? " " : "";
            inputField.SetTextWithoutNotify(currentText.TrimEnd() + separator + transcript);
        }

        inputField.caretPosition = inputField.text.Length;
        inputField.ForceLabelUpdate();
    }

    private IEnumerator RestartListeningAfterTranscript()
    {
        yield return null;

        if (!isListening)
            yield break;

        Debug.Log("Restarting Speech To Text...");

        speechToTextAgent.ClearLastTranscript();
        speechToTextAgent.StartListening();

        restartCoroutine = null;
    }
}