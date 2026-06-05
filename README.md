
# De-Escalate

De-Escalate is an AR assistive system designed to help people manage specific phobias in everyday environments by modifying how feared objects appear in real time. The system uses a combination of realtime object detection, GPU shaders, and adaptive overlays to reduce visual details that trigger distress, while keeping the user present in their surroundings. Users remain in control via adjustable intensity sliders and hands-free voice commands; an AI-guided voice mode offers contextual reframing, grounding guidance, and breathing prompts to provide emotional support during encounters.

This project is prototyped for passthrough XR devices (development tested on Meta Quest 3) and demonstrates how layered visual filters and conversational assistance can create a graded exposure pathway — from strong visual blocking to subtle softening — tailored to user comfort.

## Features

- Real-time detection (YOLOv8 prototype): a detection pipeline identifies visual triggers in the camera feed and supplies bounding regions for overlays. Prototyped with YOLOv8 on mobile and Quest devices to validate latency and reliability.
- Visual filters (blur, pixelate, block, replace): GPU-powered shaders and prefab overlays alter the appearance of detected objects. Filters range from full blocking to partial softening, allowing graded exposure.
- Adjustable intensity & controls: sliders in the UI and voice commands let users increase or decrease effect strength on the fly, enabling gradual tolerance building without removing situational awareness.
- AI Voice Mode: a contextual voice assistant that offers purposeful reframing, guided breathing, and short coaching prompts. Designed for hands-free emotional support rather than clinical treatment.
- Onboarding, safety & settings: first-run walkthrough, emergency stop/quick-exit, and per-user settings (phobia preferences, emergency contacts, AI voice options) to keep testing safe and configurable.


## Setup

1. Clone the repository:

```bash
git clone <repo-url>
```

2. Open the project in Unity Hub (select the project folder or open ShaderObjectDetection.slnx) and allow Unity to resolve packages.

3. Install/enable required packages via Package Manager: `TextMesh Pro`, `Input System` (optional), `XR Plugin Management` and the appropriate Quest/Oculus loader if testing on headset.

4. Open a sample scene under `Assets/Scenes/` and press Play to test in the Editor.

5. To build: `File -> Build Settings`, choose platform, add scenes, and click Build.
