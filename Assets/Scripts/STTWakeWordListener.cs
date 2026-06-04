using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Meta.XR.BuildingBlocks.AIBlocks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class STTWakeWordListener : MonoBehaviour
{
    [System.Serializable]
    public class WakeWordEvent
    {
        [Header("Wake Word")]
        public string wakeWord;

        [Header("Event fired when this wake word is detected")]
        public UnityEvent onWake;
    }

    [Header("STT")]
    [SerializeField] private GameObject speechToTextAgentObject;
    private SpeechToTextAgent speechToText;

    [Header("Wake Words")]
    [SerializeField] private List<WakeWordEvent> wakeWords = new List<WakeWordEvent>();

    [Header("Repeat Listening")]
    [SerializeField] private bool startListeningOnStart = true;
    [SerializeField] private bool keepListeningForever = true;
    [SerializeField] private float restartDelay = 0.5f;

    [Header("Debug UI")]
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private int maxLines = 8;
    [SerializeField] private bool debugLogs = true;

    private string log = "";
    private bool isRestarting = false;

    private void Awake()
    {
        if (speechToTextAgentObject == null)
        {
            AddLine("ERROR: STT object not assigned.");
            return;
        }

        speechToText = speechToTextAgentObject.GetComponent<SpeechToTextAgent>();

        if (speechToText == null)
        {
            AddLine("ERROR: SpeechToTextAgent component missing.");
            return;
        }

        AddLine("STT Wake Word Listener ready.");
    }

    private void OnEnable()
    {
        if (startListeningOnStart)
        {
            StartListening();
        }
    }

    public void StartListening()
    {
        if (speechToText == null)
        {
            AddLine("Cannot listen. STT is null.");
            return;
        }

        if (!speechToTextAgentObject.activeInHierarchy)
        {
            AddLine("Cannot listen. STT object inactive.");
            return;
        }

        AddLine("Calling StartListening().");
        speechToText.StartListening();
    }

    public void StopLoop()
    {
        keepListeningForever = false;
        AddLine("Listening loop stopped.");

        TryCallStopListening();
    }

    public void StartLoop()
    {
        keepListeningForever = true;
        AddLine("Listening loop started.");
        StartListening();
    }

    // Connect your STT transcript event to this function
    public void OnTranscriptReceived(string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript))
        {
            AddLine("Empty transcript received.");
            QueueRestart();
            return;
        }

        AddLine("Transcript: " + transcript);

        CheckWakeWords(transcript);

        // Restart because the STT block often only listens once
        QueueRestart();
    }

    // Connect STT OnListeningStarted here if available
    public void OnListeningStarted()
    {
        AddLine("Listening started.");
    }

    // Connect STT OnListeningStopped here if available
    public void OnListeningStopped()
    {
        AddLine("Listening stopped.");
        QueueRestart();
    }

    // Connect STT error event here if available
    public void OnSTTError(string error)
    {
        AddLine("STT Error: " + error);
        QueueRestart();
    }

    private void CheckWakeWords(string transcript)
    {
        string cleanTranscript = Normalize(transcript);

        foreach (WakeWordEvent wakeWordEvent in wakeWords)
        {
            if (wakeWordEvent == null)
                continue;

            if (string.IsNullOrWhiteSpace(wakeWordEvent.wakeWord))
                continue;

            string cleanWakeWord = Normalize(wakeWordEvent.wakeWord);

            if (cleanTranscript.Contains(cleanWakeWord))
            {
                AddLine("Wake word detected: " + wakeWordEvent.wakeWord);
                wakeWordEvent.onWake?.Invoke();
            }
        }
    }

    private void QueueRestart()
    {
        if (!keepListeningForever)
            return;

        if (isRestarting)
            return;

        StartCoroutine(RestartListeningRoutine());
    }

    private IEnumerator RestartListeningRoutine()
    {
        isRestarting = true;

        yield return new WaitForSeconds(restartDelay);

        isRestarting = false;

        if (!keepListeningForever)
            yield break;

        if (speechToText == null)
            yield break;

        AddLine("Restarting listening...");
        StartListening();
    }

    private void TryCallStopListening()
    {
        if (speechToText == null)
            return;

        MethodInfo stopMethod = speechToText.GetType().GetMethod("StopListening");

        if (stopMethod != null)
        {
            stopMethod.Invoke(speechToText, null);
            AddLine("StopListening() called through reflection.");
        }
        else
        {
            AddLine("StopListening() method not found. Skipping.");
        }
    }

    private string Normalize(string text)
    {
        text = text.ToLower();

        char[] chars = text.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (!char.IsLetterOrDigit(chars[i]) && !char.IsWhiteSpace(chars[i]))
            {
                chars[i] = ' ';
            }
        }

        return new string(chars).Trim();
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

        if (debugLogs)
        {
            Debug.Log("[STT WAKE WORD] " + message);
        }
    }
}