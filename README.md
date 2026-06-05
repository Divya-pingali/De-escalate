# De-Escalate

De-Escalate is an AR glasses assistive application designed to help people manage specific phobias when they unexpectedly encounter triggers in everyday environments. In real time, the system detects the fear object and changes how it appears through visual filters such as blur, pixelation, blocking, or calming overlays, so the user can reduce frightening details while still staying aware of their surroundings.

This project is important because specific phobias can make everyday situations feel overwhelming, even when the object is not physically dangerous. De-Escalate gives users more control in the moment by letting them adjust how much of the trigger they want to see. Instead of fully avoiding the trigger, the system supports a gradual and more manageable experience based on the user’s comfort level.

This app was developed and tested on Meta Quest 3, with future extension to other Unity XR-compatible devices through the Android build pipeline.
## Features

### 1. Onboarding and User Settings

The app begins with an onboarding process where users can enter basic information about their phobia, chosen fear object, trigger details, comfort level, and emergency contact. These settings help personalize the experience so the system can respond based on the user’s specific fear and preferences.

### 2. Adjustable Filters

Users can change how the detected object appears in real time using filters such as blur, pixelation, blocking, or visual replacement. The intensity of each filter can be increased or decreased at any time, allowing users to move from stronger visual protection to lighter softening as they feel more comfortable.

### 3. Voice Instructions for Changing Filters

Users can control the filters hands-free using voice commands. For example, they can say commands like “increase pixelation,” “decrease blur". This allows users to adjust the experience quickly without opening menus during a stressful moment.

### 4. AI Voice Mode

AI Voice Mode lets users talk to an AI assistant during the encounter. The assistant can provide calming guidance, breathing prompts, grounding support, and positive reframing of the trigger. This makes the experience more personal and supportive, while keeping the user in control.

## Tech Stack

- Unity game engine
- Meta XR SDK for passthrough XR and headset interaction
- Ultralytics YOLOv8 for object detection based on the user’s chosen fear object
- GPU shaders for real-time visual filters and appearance changes
- Object overlays for replacing or softening detected triggers
- OpenAI Realtime API for AI Voice Mode integration
- Unity UI Assets for onboarding, settings, and filter controls

## Setup

1. Clone the repository:

```bash
git clone <repo-url>
cd ShaderObjectDetection
```

2. Open the project in Unity Hub:
- In Unity Hub, click Add and select the project folder (`ShaderObjectDetection`).
- Open the project and allow Unity to finish importing assets and resolving packages.

3. Verify required Unity packages in `Window -> Package Manager`:
- `TextMesh Pro`
- `XR Plugin Management`
- `Meta XR SDK` (this project uses `com.meta.xr.sdk.all`)
- `Input System` (enable if your local setup requires the new input backend)

4. Set up object detection model (YOLOv8):
- Add your trained or fine-tuned YOLOv8 model asset to the project (for example under `Assets/MetaXR/` or another tracked model folder).
- In the scene object that owns detection/inference settings (the object with `ObjectDetectionVisualizer`), assign your model and label mapping.
- Make sure the active model/label set matches the fear object selected in user settings (phobia profile/model dropdown) so detections and filters align with the selected target.

5. Add your OpenAI API key for AI Voice Mode:
- Preferred: use a local-only config or your API key manager flow.
- Current project fallback: set the key in the Inspector on `OpenAIRealtimeQuestDirect` (`openAiApiKey`) for local testing only.
- Optional local env example:

```bash
OPENAI_API_KEY=your_api_key_here
```

6. Test on Meta Quest:
- Connect the headset over USB or wireless ADB.
- Ensure Developer Mode is enabled on the device.

7. Build and deploy to Quest:
- In Unity: `File -> Build Settings -> Android`.
- Click `Add Open Scenes`.
- Click `Build` (or `Build And Run`) and deploy to the headset.
- Test the passthrough experience end-to-end on device.
