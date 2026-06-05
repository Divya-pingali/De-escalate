# De-Escalate

De-Escalate is an AR assistive system designed to help people manage specific phobias when they unexpectedly encounter feared objects in everyday environments. In real time, the system detects the fear object and changes how it appears through visual filters such as blur, pixelation, blocking, or calming overlays, so the user can reduce frightening details while still staying aware of their surroundings.

This project is important because specific phobias can make everyday situations feel overwhelming, even when the object is not physically dangerous. De-Escalate gives users more control in the moment by letting them adjust how much of the feared object they want to see. Instead of fully avoiding the trigger, the system supports a gradual and more manageable experience based on the user’s comfort level.

## Features

### 1. Onboarding and User Settings

The app begins with an onboarding process where users can enter basic information about their phobia, chosen fear object, trigger details, comfort level, and emergency contact. These settings help personalize the experience so the system can respond based on the user’s specific fear and preferences.

### 2. Adjustable Filters

Users can change how the detected object appears in real time using filters such as blur, pixelation, blocking, or visual replacement. The intensity of each filter can be increased or decreased at any time, allowing users to move from stronger visual protection to lighter softening as they feel more comfortable.

### 3. Voice Instructions for Changing Filters

Users can control the filters hands-free using voice commands. For example, they can say commands like “increase pixelation,” “decrease blur,” “block the object,” or “make it softer.” This allows users to adjust the experience quickly without opening menus during a stressful moment.

### 4. AI Voice Mode

AI Voice Mode lets users talk to an AI assistant during the encounter. The assistant can provide calming guidance, breathing prompts, grounding support, and positive reframing of the feared object. This makes the experience more personal and supportive, while keeping the user in control.

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
