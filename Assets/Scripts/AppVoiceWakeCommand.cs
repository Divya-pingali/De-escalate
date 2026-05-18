using UnityEngine;
using Oculus.Voice;

public class AppVoiceWakeCommand : MonoBehaviour
{
    [Header("App Voice Experience")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private bool startListeningOnStart = true;
    [SerializeField] private bool autoRestartAfterComplete = true;
    [SerializeField] private float restartDelay = 0.25f;

    [Header("Commands")]
    [SerializeField] private string wakeCommand = "hello";
    [SerializeField] private string sleepCommand = "bye";

    [Header("Objects To Control")]
    [SerializeField] private GameObject objectToActivateOnWake;
    [SerializeField] private GameObject objectToDeactivateOnBye;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private bool assistantAwake = false;
    private bool restartQueued = false;

    private void Start()
    {
        if (startListeningOnStart)
        {
            StartListening();
        }
    }

    public void StartListening()
    {
        if (appVoiceExperience == null)
        {
            Debug.LogError("[APP VOICE WAKE] AppVoiceExperience is not assigned.");
            return;
        }

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Starting App Voice Experience...");

        appVoiceExperience.Activate();
    }

    public void StopListening()
    {
        if (appVoiceExperience == null)
            return;

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Stopping App Voice Experience...");

        appVoiceExperience.Deactivate();
    }

    // Connect AppVoiceExperience -> OnFullTranscription(string) to this
    public void OnFullTranscription(string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript))
            return;

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Full transcript: " + transcript);

        CheckCommand(transcript);
    }

    // Optional: connect OnPartialTranscription(string) for debugging only
    public void OnPartialTranscription(string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript))
            return;

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Partial transcript: " + transcript);
    }

    // Connect AppVoiceExperience -> OnComplete() to this
    public void OnVoiceComplete()
    {
        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Voice request complete.");

        if (autoRestartAfterComplete && !restartQueued)
        {
            restartQueued = true;
            Invoke(nameof(RestartListening), restartDelay);
        }
    }

    public void RestartListening()
    {
        restartQueued = false;

        if (appVoiceExperience == null)
            return;

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Restarting listening...");

        appVoiceExperience.Activate();
    }

    private void CheckCommand(string transcript)
    {
        string text = Normalize(transcript);

        if (text.Contains(Normalize(wakeCommand)))
        {
            WakeAssistant();
        }
        else if (text.Contains(Normalize(sleepCommand)))
        {
            SleepAssistant();
        }
    }

    private void WakeAssistant()
    {
        assistantAwake = true;

        if (objectToActivateOnWake != null)
            objectToActivateOnWake.SetActive(true);

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Hey Assistant detected. Object activated.");
    }

    private void SleepAssistant()
    {
        assistantAwake = false;

        if (objectToDeactivateOnBye != null)
            objectToDeactivateOnBye.SetActive(false);

        if (debugLogs)
            Debug.Log("[APP VOICE WAKE] Bye detected. Object deactivated.");
    }

    private string Normalize(string text)
    {
        text = text.ToLower();

        char[] chars = text.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (!char.IsLetterOrDigit(chars[i]) && !char.IsWhiteSpace(chars[i]))
                chars[i] = ' ';
        }

        return new string(chars).Trim();
    }
}