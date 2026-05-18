using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Unity.WebRTC;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class OpenAIRealtimeQuestDirect : MonoBehaviour
{
    [Header("OpenAI")]
    [SerializeField] private string openAiApiKey = "PASTE_YOUR_API_KEY_HERE";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource micAudioSource;
    [SerializeField] private AudioSource modelAudioSource;

    [Header("Realtime Settings")]
    [SerializeField] private string model = "gpt-realtime-2";
    [SerializeField] private string voice = "marin";

    [Header("System Prompt")]
    [TextArea(12, 35)]
    [SerializeField] private string systemPrompt =
@"You are an AI support companion for Toby Fung, a female user in Hong Kong with a phobia of moths.

Toby's profile:
Name: Toby Fung
Occupation: Illustrator and designer
Location: Hong Kong
Phobia: Moths

Background:
Toby is highly creative and visually sensitive. Her fear is mainly visual because she notices detailed features very strongly and has a vivid imagination.
Her phobia began when she was young after seeing a large detailed picture of a moth in an encyclopedia. The close-up details, including hairy structures, patterns, textures, and small features, made moths feel creepy and overwhelming to her.
Her fear is also shaped by Chinese cultural associations where moths can be linked with negative energy or dead people. This created a darker emotional association for her.
She tried exposure therapy in college by making a collage of moths, but it became too overwhelming and she could not finish it. Exposure therapy did not work for her.
Her phobia affects daily life. For example, she once went hiking with her partner during moth mating season and saw hundreds of moths, which scared her.
She says her phobia is not extremely severe, but it is still present and distressing.
She would rather not see a moth than touch one. Seeing the moth is the main trigger.

App context:
This is a Meta Quest AR glasses app.
The app uses the camera to detect moths and visually block or alter them when detected.
A passthrough camera snapshot may be sent automatically whenever Toby starts speaking.
The snapshot is only extra visual context. Do not assume it is always relevant.
Do not talk about the snapshot unless the user's message needs visual context.
Do not claim to see live video. You only receive occasional snapshots.

Behavior rules:
Keep responses brief, warm, natural, and supportive.
Do not over-comfort Toby in every reply.
Do not repeatedly mention moth appearance, culture, safety, or calming unless the user's message calls for it.
Read Toby's actual words and respond to what she is asking or feeling.
If she shows strong fear, panic, disgust, or distress, calmly validate her feeling and help her feel safe.
If she talks about visual features of moths, respond gently about the visual features without graphic detail.
If she talks about Chinese cultural associations, gently reframe them without dismissing her background.
If she asks about usefulness, explain moth usefulness in nature briefly and positively.
If she asks what the app is doing, explain that the app is helping by visually blocking or changing the moth so she does not have to directly see it.
Do not force exposure.
Do not tell her to look at, touch, approach, or study moths.
Do not pressure her to overcome the fear.
Do not say exposure therapy is required.
Do not provide medical claims.
Do not replace professional mental health care.
Avoid graphic or detailed moth descriptions unless Toby specifically asks about those features.
If no moth is visible or no relevant visual context is available, answer normally instead of forcing moth-related reassurance.
If Toby is casually talking, talk casually.
If Toby asks a technical or app question, answer technically and directly.";

    [Header("Passthrough Snapshot")]
    [SerializeField] private QuestPassthroughSnapshot passthroughSnapshot;
    [SerializeField] private bool captureSnapshotOnSpeechStarted = true;
    [SerializeField] private float speechSnapshotCooldownSeconds = 1f;
    [SerializeField] private string speechSnapshotPrompt = "This passthrough camera snapshot was captured when Toby started speaking. Use it only if it is relevant to her spoken message. Do not mention the snapshot unless it helps answer her.";

    private const string RealtimeCallsUrl = "https://api.openai.com/v1/realtime/calls";

    private RTCPeerConnection peerConnection;
    private RTCDataChannel dataChannel;
    private AudioStreamTrack micTrack;
    private MediaStream receiveStream;

    private bool realtimeReady;
    private bool pendingSpeechSnapshot;
    private float lastSpeechSnapshotTime = -100f;

    private void Start()
    {
        StartCoroutine(StartRealtime());
    }

    private void Update()
    {
        if (!pendingSpeechSnapshot)
        {
            return;
        }

        pendingSpeechSnapshot = false;

        if (!captureSnapshotOnSpeechStarted)
        {
            return;
        }

        if (Time.time - lastSpeechSnapshotTime < speechSnapshotCooldownSeconds)
        {
            return;
        }

        lastSpeechSnapshotTime = Time.time;
        SendSpeechStartedPassthroughSnapshot();
    }

    private IEnumerator StartRealtime()
    {
        if (string.IsNullOrWhiteSpace(openAiApiKey) || openAiApiKey.Contains("PASTE_YOUR"))
        {
            Debug.LogError("Missing OpenAI API key. Paste it into the Inspector.");
            yield break;
        }

        if (micAudioSource == null)
        {
            Debug.LogError("Mic Audio Source is not assigned in the Inspector.");
            yield break;
        }

        if (modelAudioSource == null)
        {
            Debug.LogError("Model Audio Source is not assigned in the Inspector.");
            yield break;
        }

        if (passthroughSnapshot == null)
        {
            Debug.LogError("QuestPassthroughSnapshot is not assigned in the Inspector.");
            yield break;
        }

        yield return RequestMicrophonePermission();

        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.LogError("Microphone permission not granted.");
            yield break;
        }

        StartCoroutine(WebRTC.Update());

        RTCConfiguration config = default;
        peerConnection = new RTCPeerConnection(ref config);

        SetupRemoteAudio();
        SetupDataChannel();

        yield return SetupMicrophoneTrack();

        yield return CreateOfferAndConnect();

        realtimeReady = true;

        Debug.Log("OpenAI Realtime setup complete.");
    }

    private IEnumerator RequestMicrophonePermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            yield return new WaitForSeconds(1f);
        }
#endif

        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
    }

    private void SetupRemoteAudio()
    {
        receiveStream = new MediaStream();

        receiveStream.OnAddTrack = e =>
        {
            if (e.Track is AudioStreamTrack audioTrack)
            {
                Debug.Log("Received remote audio track from OpenAI.");
                modelAudioSource.SetTrack(audioTrack);
                modelAudioSource.loop = true;
                modelAudioSource.Play();
            }
        };

        peerConnection.OnTrack = e =>
        {
            if (e.Track.Kind == TrackKind.Audio)
            {
                receiveStream.AddTrack(e.Track);
            }
        };
    }

    private void SetupDataChannel()
    {
        dataChannel = peerConnection.CreateDataChannel("oai-events");

        dataChannel.OnOpen = () =>
        {
            Debug.Log("OpenAI data channel opened.");
            SendSessionUpdate();
        };

        dataChannel.OnClose = () =>
        {
            Debug.Log("OpenAI data channel closed.");
        };

        dataChannel.OnMessage = bytes =>
        {
            string json = Encoding.UTF8.GetString(bytes);
            Debug.Log("OpenAI event: " + json);

            if (json.Contains("\"input_audio_buffer.speech_started\""))
            {
                pendingSpeechSnapshot = true;
            }

            if (json.Contains("\"type\":\"error\"") || json.Contains("\"type\": \"error\""))
            {
                Debug.LogError("OpenAI Realtime error event: " + json);
            }
        };
    }

    private IEnumerator SetupMicrophoneTrack()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found.");
            yield break;
        }

        string deviceName = Microphone.devices[0];

        Debug.Log("Using microphone: " + deviceName);

        micAudioSource.loop = true;
        micAudioSource.clip = Microphone.Start(deviceName, true, 1, 48000);

        while (Microphone.GetPosition(deviceName) <= 0)
        {
            yield return null;
        }

        micAudioSource.Play();

        micTrack = new AudioStreamTrack(micAudioSource);

        MediaStream sendStream = new MediaStream();
        peerConnection.AddTrack(micTrack, sendStream);

        Debug.Log("Microphone track added to WebRTC peer connection.");
    }

    private IEnumerator CreateOfferAndConnect()
    {
        RTCSessionDescriptionAsyncOperation offerOp = peerConnection.CreateOffer();
        yield return offerOp;

        if (offerOp.IsError)
        {
            Debug.LogError("CreateOffer failed: " + offerOp.Error.message);
            yield break;
        }

        RTCSessionDescription offerDesc = offerOp.Desc;

        RTCSetSessionDescriptionAsyncOperation localDescOp =
            peerConnection.SetLocalDescription(ref offerDesc);

        yield return localDescOp;

        if (localDescOp.IsError)
        {
            Debug.LogError("SetLocalDescription failed: " + localDescOp.Error.message);
            yield break;
        }

        Debug.Log("Created local SDP offer. Sending to OpenAI...");

        yield return SendOfferToOpenAI(offerDesc.sdp);
    }

    private IEnumerator SendOfferToOpenAI(string offerSdp)
    {
        string sessionJson = BuildSessionJson();

        List<IMultipartFormSection> form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection(
                "sdp",
                Encoding.UTF8.GetBytes(offerSdp),
                "application/sdp"
            ),
            new MultipartFormDataSection(
                "session",
                Encoding.UTF8.GetBytes(sessionJson),
                "application/json"
            )
        };

        UnityWebRequest req = UnityWebRequest.Post(RealtimeCallsUrl, form);
        req.SetRequestHeader("Authorization", "Bearer " + openAiApiKey);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "OpenAI /realtime/calls failed.\n" +
                "HTTP " + req.responseCode + "\n" +
                req.downloadHandler.text
            );

            req.Dispose();
            yield break;
        }

        string answerSdp = req.downloadHandler.text;
        req.Dispose();

        RTCSessionDescription answerDesc = new RTCSessionDescription
        {
            type = RTCSdpType.Answer,
            sdp = answerSdp
        };

        RTCSetSessionDescriptionAsyncOperation remoteDescOp =
            peerConnection.SetRemoteDescription(ref answerDesc);

        yield return remoteDescOp;

        if (remoteDescOp.IsError)
        {
            Debug.LogError("SetRemoteDescription failed: " + remoteDescOp.Error.message);
            yield break;
        }

        Debug.Log("Remote SDP answer applied. WebRTC connection should now start.");
    }

    private string BuildSessionJson()
    {
        return $@"
        {{
            ""type"": ""realtime"",
            ""model"": ""{EscapeJson(model)}"",
            ""output_modalities"": [""audio""],
            ""audio"": {{
                ""input"": {{
                    ""turn_detection"": {{
                        ""type"": ""semantic_vad"",
                        ""eagerness"": ""medium""
                    }}
                }},
                ""output"": {{
                    ""voice"": ""{EscapeJson(voice)}""
                }}
            }},
            ""instructions"": ""{EscapeJson(systemPrompt)}""
        }}";
    }

    private void SendSessionUpdate()
    {
        if (!IsDataChannelOpen())
        {
            return;
        }

        string json = $@"
        {{
            ""type"": ""session.update"",
            ""session"": {BuildSessionJson()}
        }}";

        dataChannel.Send(json);
    }

    public void UpdateSystemPrompt(string newSystemPrompt)
    {
        systemPrompt = newSystemPrompt;
        SendSessionUpdate();
    }

    private void SendSpeechStartedPassthroughSnapshot()
    {
        if (!realtimeReady)
        {
            Debug.LogWarning("Realtime is not ready yet.");
            return;
        }

        if (!IsDataChannelOpen())
        {
            Debug.LogWarning("Data channel is not open yet.");
            return;
        }

        if (passthroughSnapshot == null)
        {
            Debug.LogWarning("QuestPassthroughSnapshot is not assigned.");
            return;
        }

        string base64Image = passthroughSnapshot.CaptureJpegBase64();

        if (string.IsNullOrEmpty(base64Image))
        {
            Debug.LogWarning("Speech started, but passthrough snapshot capture failed.");
            return;
        }

        Debug.Log("Speech started. Sending passthrough snapshot. Base64 length: " + base64Image.Length);

        SendSnapshotContextOnly(speechSnapshotPrompt, base64Image);
    }

    private void SendSnapshotContextOnly(string text, string base64Image)
    {
        string safeText = EscapeJson(text);

        string createItemJson = $@"
        {{
            ""type"": ""conversation.item.create"",
            ""item"": {{
                ""type"": ""message"",
                ""role"": ""user"",
                ""content"": [
                    {{
                        ""type"": ""input_text"",
                        ""text"": ""{safeText}""
                    }},
                    {{
                        ""type"": ""input_image"",
                        ""image_url"": ""data:image/jpeg;base64,{base64Image}""
                    }}
                ]
            }}
        }}";

        dataChannel.Send(createItemJson);
    }

    public void SendTextMessage(string text)
    {
        if (!IsDataChannelOpen())
        {
            Debug.LogWarning("Data channel is not open yet.");
            return;
        }

        string safeText = EscapeJson(text);

        string createItemJson = $@"
        {{
            ""type"": ""conversation.item.create"",
            ""item"": {{
                ""type"": ""message"",
                ""role"": ""user"",
                ""content"": [
                    {{
                        ""type"": ""input_text"",
                        ""text"": ""{safeText}""
                    }}
                ]
            }}
        }}";

        dataChannel.Send(createItemJson);
        SendAudioResponseCreate();
    }

    public void SendTemporarySystemMessage(string text)
    {
        if (!IsDataChannelOpen())
        {
            Debug.LogWarning("Data channel is not open yet.");
            return;
        }

        string safeText = EscapeJson(text);

        string createItemJson = $@"
        {{
            ""type"": ""conversation.item.create"",
            ""item"": {{
                ""type"": ""message"",
                ""role"": ""system"",
                ""content"": [
                    {{
                        ""type"": ""input_text"",
                        ""text"": ""{safeText}""
                    }}
                ]
            }}
        }}";

        dataChannel.Send(createItemJson);
    }

    private void SendAudioResponseCreate()
    {
        string createResponseJson = @"
        {
            ""type"": ""response.create"",
            ""response"": {
                ""output_modalities"": [""audio""]
            }
        }";

        dataChannel.Send(createResponseJson);
    }

    private bool IsDataChannelOpen()
    {
        return dataChannel != null && dataChannel.ReadyState == RTCDataChannelState.Open;
    }

    private string EscapeJson(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private void OnDestroy()
    {
        realtimeReady = false;

        if (micTrack != null)
        {
            micTrack.Dispose();
            micTrack = null;
        }

        if (dataChannel != null)
        {
            dataChannel.Close();
            dataChannel.Dispose();
            dataChannel = null;
        }

        if (peerConnection != null)
        {
            peerConnection.Close();
            peerConnection.Dispose();
            peerConnection = null;
        }

        if (receiveStream != null)
        {
            receiveStream.Dispose();
            receiveStream = null;
        }
    }
}