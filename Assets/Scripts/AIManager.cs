using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Meta.XR.BuildingBlocks.AIBlocks;

public class AIManager : MonoBehaviour
{
    [Header("Agents")]
    public SpeechToTextAgent speechToTextAgent;
    public LlmAgentHelper llmAgentHelper;
    public TextToSpeechAgent textToSpeechAgent;

    [Header("Editor Testing")]
    public Key listenKey = Key.Space;
    public Key fakePromptKey = Key.T;

    [TextArea]
    public string fakePrompt = "Help me calm down. I saw a moth.";

    [Header("Quest Hand Gesture")]
    public OVRHand hand;
    public OVRHand.HandFinger pinchFinger = OVRHand.HandFinger.Index;
    public float pinchCooldown = 1.0f;

    private bool wasPinching;
    private float lastPinchTime;

    private void OnEnable()
    {
        if (speechToTextAgent != null)
        {
            speechToTextAgent.onTranscript.AddListener(SendToLLM);
        }
    }

    private void OnDisable()
    {
        if (speechToTextAgent != null)
        {
            speechToTextAgent.onTranscript.RemoveListener(SendToLLM);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleEditorInput();
#else
        HandleHandInput();
#endif
    }

    private void HandleEditorInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[listenKey].wasPressedThisFrame)
        {
            StartListening();
        }

        if (Keyboard.current[fakePromptKey].wasPressedThisFrame)
        {
            SendToLLM(fakePrompt);
        }
    }

    private void HandleHandInput()
    {
        if (hand == null) return;

        bool isPinching = hand.GetFingerIsPinching(pinchFinger);

        if (isPinching && !wasPinching && Time.time - lastPinchTime > pinchCooldown)
        {
            lastPinchTime = Time.time;
            StartListening();
        }

        wasPinching = isPinching;
    }

    private void StartListening()
    {
        if (speechToTextAgent == null)
        {
            Debug.LogError("SpeechToTextAgent is missing.");
            return;
        }

        Debug.Log("Listening...");
        speechToTextAgent.StartListening();
    }

    private void SendToLLM(string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript)) return;

        Debug.Log("User said: " + transcript);

        if (llmAgentHelper == null)
        {
            Debug.LogError("LlmAgentHelper is missing.");
            return;
        }

        Type helperType = llmAgentHelper.GetType();

        MethodInfo sendPromptWithString = helperType.GetMethod(
            "SendPrompt",
            new Type[] { typeof(string) }
        );

        if (sendPromptWithString != null)
        {
            sendPromptWithString.Invoke(llmAgentHelper, new object[] { transcript });
            return;
        }

        string[] possiblePromptFields =
        {
            "prompt",
            "_prompt",
            "inputPrompt",
            "_inputPrompt",
            "userInput",
            "_userInput",
            "userPrompt",
            "_userPrompt",
            "message",
            "_message"
        };

        foreach (string fieldName in possiblePromptFields)
        {
            FieldInfo field = helperType.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (field != null && field.FieldType == typeof(string))
            {
                field.SetValue(llmAgentHelper, transcript);
                CallSendPromptNoArgs(helperType);
                return;
            }
        }

        foreach (string propertyName in possiblePromptFields)
        {
            PropertyInfo property = helperType.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (property != null && property.PropertyType == typeof(string) && property.CanWrite)
            {
                property.SetValue(llmAgentHelper, transcript);
                CallSendPromptNoArgs(helperType);
                return;
            }
        }

        Debug.LogError("Could not find a prompt field/property on LlmAgentHelper.");
    }

    private void CallSendPromptNoArgs(Type helperType)
    {
        MethodInfo sendPromptNoArgs = helperType.GetMethod(
            "SendPrompt",
            Type.EmptyTypes
        );

        if (sendPromptNoArgs == null)
        {
            Debug.LogError("Could not find SendPrompt() on LlmAgentHelper.");
            return;
        }

        sendPromptNoArgs.Invoke(llmAgentHelper, null);
    }

    public void SpeakResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response)) return;

        Debug.Log("AI response: " + response);

        if (textToSpeechAgent == null)
        {
            Debug.LogWarning("TextToSpeechAgent missing. AI said: " + response);
            return;
        }

        textToSpeechAgent.SpeakText(response);
    }
}
