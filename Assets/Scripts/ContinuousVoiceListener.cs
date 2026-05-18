using UnityEngine;
using Oculus.Voice;

public class StartVoiceOnLaunch : MonoBehaviour
{
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    private void Start()
    {
        StartListening();
    }

    public void StartListening()
    {
        if (appVoiceExperience == null)
        {
            Debug.LogError("AppVoiceExperience is not assigned.");
            return;
        }

        if (!appVoiceExperience.Active)
        {
            Debug.Log("Starting voice listening...");
            appVoiceExperience.Activate();
        }
        else
        {
            Debug.Log("Voice is already active.");
        }
    }
}