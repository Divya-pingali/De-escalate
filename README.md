# De-Escalate

De-Escalate is an AR application for glasses that helps people with specific phobias be more in control when encountering unexpected triggers. In real time, the app detects the trigger and changes its appearance using filters like blur, pixelation, blocking, or calming overlays.

Instead of removing the trigger completely, De-Escalate lets users choose how much detail they want to see based on their comfort level. This makes the encounter feel safer, less overwhelming, and more manageable.

This app was developed and tested on Meta Quest 3, with future extension to other Unity XR-compatible devices through the Android build pipeline.
## Features

### 1. Onboarding and User Settings

The app begins with an onboarding process where users can enter basic information about their phobia, chosen trigger, trigger details, comfort level, and emergency contact. These settings help personalize the experience so the system can respond based on the user’s specific fear and preferences.

### 2. Adjustable Filters

Users can change how the detected object appears in real time using filters such as blur, pixelation, blocking, or visual replacement. The intensity of each filter can be increased or decreased at any time, allowing users to move from stronger visual protection to lighter softening as they feel more comfortable.

### 3. Voice Instructions for Changing Filters

Users can control the filters hands-free using voice commands. For example, they can say commands like “increase pixelation,” and “decrease blur". This allows users to adjust the experience quickly without opening menus during a stressful moment.

### 4. AI Voice Mode

AI Voice Mode lets users talk to an AI assistant during the encounter. The assistant can provide calming guidance, breathing prompts, grounding support, and positive reframing of the trigger. This makes the experience more personal and supportive, while keeping the user in control.

## Tech Stack

- Unity game engine
- Meta XR SDK for passthrough XR and headset interaction
- Ultralytics YOLOv8 for object detection based on the user’s chosen trigger
- GPU shaders for real-time visual filters and appearance changes
- Object overlays for replacing or softening detected triggers
- OpenAI Realtime API for AI Voice Mode integration
- Unity UI Assets for onboarding, settings, and filter controls

## Setup

1. Clone the repository and open in Unity Hub

2. Set up object detection model (YOLOv8):
- Add your trained or fine-tuned YOLOv8 model asset to the project.
- In the scene object that owns detection/inference settings, assign your model and label mapping.
- Make sure the active model/label set matches the trigger selected in user settings so detections and filters align with the selected target.

3. Add your OpenAI API key for AI Voice Mode:
- Preferred: use a local-only config or your API key manager flow.
- Current project fallback: set the key in the Inspector on `OpenAIRealtimeQuestDirect` (`openAiApiKey`) for local testing only.

4. Build and deploy to Quest:
- In Unity: `File -> Build Settings -> Android`.
- Click `Add Open Scenes`.
- Click `Build` (or `Build And Run`) and deploy to the headset.
