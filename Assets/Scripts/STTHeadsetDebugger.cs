using UnityEngine;
using TMPro;
using Meta.XR.BuildingBlocks.AIBlocks;
using System.Collections;

public class STTHeadsetDebugger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text debugText;

    [Header("Settings")]
    [SerializeField] private int maxLines = 8;
    [SerializeField] private bool startListeningOnStart = true;
    [SerializeField] private float startDelay = 0.5f;

    [Header("STT")]
    [SerializeField] private GameObject speechToTextAgent;

    private SpeechToTextAgent speechToText;
    private string log = "";

    private void Awake()
    {
        AddLine("Debugger Awake.");

        if (speechToTextAgent == null)
        {
            AddLine("ERROR: SpeechToTextAgent GameObject not assigned.");
            Debug.LogError("[STT DEBUG] SpeechToTextAgent GameObject not assigned.");
            return;
        }

        AddLine("STT object assigned: " + speechToTextAgent.name);
        AddLine("STT active: " + speechToTextAgent.activeInHierarchy);

        speechToText = speechToTextAgent.GetComponent<SpeechToTextAgent>();

        if (speechToText == null)
        {
            AddLine("ERROR: SpeechToTextAgent component missing.");
            Debug.LogError("[STT DEBUG] SpeechToTextAgent component missing on " + speechToTextAgent.name);
            return;
        }

        AddLine("SpeechToTextAgent component found.");
    }

    private void Start()
    {
        AddLine("Debugger Start.");

        if (startListeningOnStart)
        {
            StartCoroutine(StartListeningDelayed());
        }
    }

    private IEnumerator StartListeningDelayed()
    {
        AddLine("Waiting before StartListening...");

        yield return new WaitForSeconds(startDelay);

        if (speechToText == null)
        {
            AddLine("Cannot start. STT is null.");
            yield break;
        }

        if (!speechToTextAgent.activeInHierarchy)
        {
            AddLine("Cannot start. STT object inactive.");
            yield break;
        }

        AddLine("Calling StartListening().");
        Debug.Log("[STT DEBUG] Calling StartListening().");

        speechToText.StartListening();
    }

    public void OnTranscriptReceived(string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript))
        {
            AddLine("Empty transcript received.");
            return;
        }

        AddLine("STT: " + transcript);
        Debug.Log("[STT DEBUG] Transcript: " + transcript);
    }

    public void OnListeningStarted()
    {
        AddLine("Listening started.");
        Debug.Log("[STT DEBUG] Listening started.");
    }

    public void OnListeningStopped()
    {
        AddLine("Listening stopped.");
        Debug.Log("[STT DEBUG] Listening stopped.");
    }

    public void OnSTTError(string error)
    {
        AddLine("STT Error: " + error);
        Debug.LogError("[STT DEBUG] Error: " + error);
    }

    private void OnDisable()
    {
        Debug.LogWarning("[STT DEBUG] STTHeadsetDebugger disabled.");
    }

    private void AddLine(string message)
    {
        string newLine = $"[{Time.time:F1}] {message}";
        log += newLine + "\n";

        string[] lines = log.Split('\n');

        if (lines.Length > maxLines)
        {
            log = "";

            for (int i = Mathf.Max(0, lines.Length - maxLines - 1); i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    log += lines[i] + "\n";
            }
        }

        if (debugText != null)
        {
            debugText.text = log;
        }

        Debug.Log("[STT HEADSET UI] " + message);
    }
}