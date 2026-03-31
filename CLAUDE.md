# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Dragon-AR-Workshop is a mobile AR application built in Unity 6000.4.0f1. It uses ARFoundation to detect real-world surfaces, then spawns a rigged 3D dragon model that users can place, tap to animate, and drag/pinch to reposition and scale in AR.

## Build & Deployment

This is a Unity project — there are no CLI build commands available outside the Unity Editor. Builds are triggered from within Unity Editor or via Unity's `BuildPipeline` API.

- **Android APK output:** `build/dragon.apk` (target: Android API 30+, ARM32+64)
- **iOS:** Bundle ID `com.unity.template.armobile`, minimum iOS 15.0
- **Application ID (Android):** `com.unity.template.ar_mobile`
- **Only scene in build:** `Assets/Scenes/DragonScene.unity`

Android debugging: use the **Android Logcat** package (Window > Analysis > Android Logcat) — it's already included.

## MCPForUnity

The project includes MCPForUnity (`Assets/MCPForUnity/`), which exposes a local MCP server so Claude Code (and other AI clients) can control the Unity Editor directly. Check `Assets/MCPForUnity/README.md` for setup instructions and client configuration.

## Architecture

### Custom Scripts (`Assets/Scripts/`)

**`DragonPlacementLogic.cs`** — Attached to the ObjectSpawner GameObject in DragonScene. Subscribes to `ObjectSpawner.objectSpawned` and rotates each newly spawned dragon to face the camera.

**`DragonInteraction.cs`** — Attached to `DragonPrefab`. Tap (< 0.35s hold) toggles the idle animation on/off. Long hold + drag repositions the dragon via `ARTransformer`. Uses `IsIdle` bool on the Animator.

**`DragonSetup.cs`** — Attached to `DragonPrefab`. Runs at `DefaultExecutionOrder(-1000)` so it executes before `XRGrabInteractable.Awake()`. Disables the FBX floor artifacts ("Hemi", "Plane" child objects) and resizes the root `BoxCollider` to world-correct dimensions (compensating for the 0.01 FBX import scale).

### Key Dependencies
- **AR:** `com.unity.xr.arfoundation` 6.4.1 + `com.unity.xr.arcore` 6.4.1 + `com.unity.xr.arkit` 6.4.2
- **Interaction:** `com.unity.xr.interaction.toolkit` 3.4.0 — `XRGrabInteractable`, `ObjectSpawner`, `ARTransformer` (scale/translate on AR planes)
- **Rendering:** URP 17.4.0 via `Assets/Settings/URP-Performant.asset` (MSAA disabled, mobile-optimized)
- **Input:** New Input System 1.19.0

### Dragon Prefab (`Assets/DragonPrefab.prefab`)
Variant of `Assets/black-dragon-with-idle-animation/source/Dragon.fbx`. Root components: `Animator` + `BoxCollider` + `Rigidbody` + `XRGrabInteractable` + `ARTransformer` + `DragonInteraction` + `DragonSetup`. Animator controller at `Assets/DragonAnimatorController.controller` — Static ↔ Idle states driven by `IsIdle` bool.

### Scene Structure (`Assets/Scenes/DragonScene.unity`)
- **AR Camera** (ARFoundation managed)
- **XR Origin (AR Rig)** — contains Screen Space Ray Interactor, ARInteractorSpawnTrigger (tap-to-place), ObjectSpawner with DragonPrefab
- **UI Canvas** — Delete button, Coaching UI prompts ("Scan Surfaces", "Tap to Place")
- **ARPlaneManager** — detects horizontal/vertical surfaces

### Template Assets (`Assets/MobileARTemplateAssets/Scripts/`)
- `ARTemplateMenuManager.cs` — manages Delete button and object deletion
- `GoalManager.cs` — coaching/goal prompt progression ("Scan Surfaces" → "Tap to Place" → placed)
- `ARPlaneMeshVisualizerFader.cs` — fades AR plane meshes after placement
