using UnityEngine;

public class STTWakeCommand : MonoBehaviour
{
    [Header("Voice Commands")]
    [SerializeField] private string wakeCommand = "hey assistant";
    [SerializeField] private string sleepCommand = "bye";

    [Header("Objects to control")]
    [SerializeField] private GameObject[] activateOnWake;
    [SerializeField] private GameObject[] deactivateOnBye;

    [Header("Optional inverse actions")]
    [SerializeField] private GameObject[] deactivateOnWake;
    [SerializeField] private GameObject[] activateOnBye;

    [Header("Settings")]
    [SerializeField] private bool requireWakeBeforeBye = true;
    [SerializeField] private float commandCooldown = 0.75f;
    [SerializeField] private bool debugLogs = true;

    private bool isAwakeMode = false;
    private float lastCommandTime = -999f;

    // Helps when your textbox keeps appending transcripts
    private string lastFullTranscript = "";

    /// <summary>
    /// Connect your Speech-To-Text transcript event to this function.
    /// </summary>
    public void CheckTranscript(string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript))
            return;

        string normalizedFull = Normalize(transcript);

        // If transcript text is cumulative, only check the newly added part
        string newText = normalizedFull;

        if (!string.IsNullOrEmpty(lastFullTranscript) &&
            normalizedFull.StartsWith(lastFullTranscript))
        {
            newText = normalizedFull.Substring(lastFullTranscript.Length).Trim();
        }

        lastFullTranscript = normalizedFull;

        if (string.IsNullOrWhiteSpace(newText))
            newText = normalizedFull;

        if (Time.time - lastCommandTime < commandCooldown)
            return;

        if (ContainsCommand(newText, wakeCommand))
        {
            WakeUp();
        }
        else if (ContainsCommand(newText, sleepCommand))
        {
            if (!requireWakeBeforeBye || isAwakeMode)
            {
                GoToSleep();
            }
        }
    }

    private void WakeUp()
    {
        lastCommandTime = Time.time;
        isAwakeMode = true;

        SetObjectsActive(activateOnWake, true);
        SetObjectsActive(deactivateOnWake, false);

        if (debugLogs)
            Debug.Log("Wake command detected: Hey Assistant");
    }

    private void GoToSleep()
    {
        lastCommandTime = Time.time;
        isAwakeMode = false;

        SetObjectsActive(deactivateOnBye, false);
        SetObjectsActive(activateOnBye, true);

        if (debugLogs)
            Debug.Log("Sleep command detected: Bye");
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    private bool ContainsCommand(string text, string command)
    {
        string normalizedText = " " + Normalize(text) + " ";
        string normalizedCommand = " " + Normalize(command) + " ";

        return normalizedText.Contains(normalizedCommand);
    }

    private string Normalize(string text)
    {
        text = text.ToLower();

        // Remove punctuation so "Hey, Assistant" still works
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
}