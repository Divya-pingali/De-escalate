using UnityEngine;
using Meta.XR.BuildingBlocks.AIBlocks;
using System.Reflection;

public class ConversationManager : MonoBehaviour
{
    [Header("Agents")]
    [SerializeField] private GameObject speechToTextAgent;
    [SerializeField] private GameObject llmAgentHelper;
    [SerializeField] private GameObject textToSpeechAgent;

    private SpeechToTextAgent speechToText;
    private LlmAgentHelper helper;
    private TextToSpeechAgent textToSpeech;

    private FieldInfo userInputField;

    private void Awake()
    {
        speechToText = speechToTextAgent.GetComponent<SpeechToTextAgent>();
        helper = llmAgentHelper.GetComponent<LlmAgentHelper>();
        textToSpeech = textToSpeechAgent.GetComponent<TextToSpeechAgent>();

        if (speechToText == null) Debug.LogError("Missing SpeechToTextAgent");
        if (helper == null) Debug.LogError("Missing LlmAgentHelper");
        if (textToSpeech == null) Debug.LogError("Missing TextToSpeechAgent");

        userInputField = typeof(LlmAgentHelper).GetField(
            "userInput",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (userInputField == null)
            Debug.LogError("Could not find private field 'userInput' on LlmAgentHelper.");
    }

    private void Start()
    {
        Debug.Log("Starting listening...");
        speechToText.StartListening();
    }

    public void GetPrompt()
    {
        Debug.Log("Restarting listening...");
        speechToText.StartListening();
    }

    public void PassPrompt(string transcript)
    {
        Debug.Log("Transcript received: " + transcript);

        if (string.IsNullOrWhiteSpace(transcript))
        {
            Debug.LogWarning("Transcript was empty.");
            GetPrompt();
            return;
        }

        if (userInputField == null)
        {
            Debug.LogError("Cannot send prompt because userInputField is null.");
            return;
        }

        userInputField.SetValue(helper, transcript);
        helper.SendPrompt();

        // Do NOT restart listening here.
        // Restart from TextToSpeechAgent.onSpeakFinished instead.
    }

    public void SpeakResponse(string response)
    {
        Debug.Log("LLM response: " + response);

        if (string.IsNullOrWhiteSpace(response))
        {
            GetPrompt();
            return;
        }

        textToSpeech.SpeakText(response);
    }
}