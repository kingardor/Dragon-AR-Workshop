# Infinian Scene Integration — Design Spec

**Date:** 2026-03-31  
**Status:** Approved

---

## Overview

Add a second AR scene featuring the Infinian Lineage Series character alongside the existing DragonScene. A persistent toggle switch in the top-right corner switches between scenes. Both scenes use identical AR mechanics (plane detection, tap-to-place, drag/pinch to reposition/scale). Scene switching destroys the current scene and loads the new one fresh — no background persistence.

---

## Architecture

### Scene Strategy

**Option A — Duplicate DragonScene** (chosen): Copy `DragonScene.unity` to `InfinianScene.unity`. Remove dragon-selector UI panel. Keep all AR infrastructure (XR Origin, ARPlaneManager, ARSession, coaching UI, delete button). Add the Infinian prefab to the ObjectSpawner.

Both scenes carry an identical `SceneSwitcher` prefab (Canvas + toggle button). Pressing it calls `SceneManager.LoadScene(otherScene, LoadSceneMode.Single)`, which destroys the current scene entirely. CoachingUI and plane detection restart fresh on each load — no special handling needed.

### Components

**`SceneSwitcher.cs`** — MonoBehaviour on a persistent UI Canvas (not DontDestroyOnLoad — just present in both scenes). Holds two scene name strings (`dragonScene`, `infinianScene`). Reads `SceneManager.GetActiveScene().name` to know which scene is current, then loads the other one on button press. Button positioned top-right via anchored RectTransform.

**`InfinianPrefab.prefab`** — Root GameObject with:
- `Animator` + `InfinianAnimatorController.controller`
- `BoxCollider` (sized at runtime by `InfinianSetup.cs`)
- `Rigidbody` (kinematic, no gravity)
- `XRGrabInteractable` (trackPosition: true, trackRotation: true)
- `ARTransformer` (translate + scale)
- `InfinianSetup.cs`

**`InfinianSetup.cs`** — `DefaultExecutionOrder(-1000)`. Disables artifact children (any child named "Hemi", "Plane", or similar). Resizes BoxCollider to compensate for FBX import scale (globalScale: 0.01). Mirrors `DragonSetup.cs` / `TarislandDragonSetup.cs` pattern.

**`InfinianPlacementLogic.cs`** — Attached to the ObjectSpawner GameObject in InfinianScene. Subscribes to `ObjectSpawner.objectSpawned`, rotates the newly spawned Infinian to face the camera, and handles delete (same suppression pattern as `DragonPlacementLogic.cs`). Wired to the Delete button as a persistent onClick listener.

**`InfinianAnimatorController.controller`** — Two states:
1. `Spawn` (default entry state) — plays `Mon_Infinian_001_Skeleton|Spawn` clip once (no loop)
2. `CombatIdle` — loops `Mon_Infinian_001_Skeleton|CombatIdle`

Transition: Spawn → CombatIdle triggered by `Has Exit Time = true`, exit time = 1.0 (end of Spawn clip). No Animator parameter needed.

**FBX & Materials:**
- `Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX` — 65MB, under Git 100MB limit, no LFS required
- `Mon_Infinian_001_A_MI.mat` and `Mon_Infinian_001_B_MI.mat` — currently Unreal-format text, must be replaced with Unity URP Lit YAML (same fix applied to Tarisland .mat files)
- FBX globalScale: check and set to 0.01 if cm-scale (same as Tarisland)

**InfinianScene.unity** — Identical AR infrastructure to DragonScene minus DragonSelectorPanel. ObjectSpawner holds InfinianPrefab. SceneSwitcher Canvas added (top-right toggle pointing to DragonScene).

**DragonScene.unity** — Add SceneSwitcher Canvas (top-right toggle pointing to InfinianScene). No other changes.

---

## Data Flow

```
User taps toggle (top-right)
  → SceneSwitcher.OnTogglePressed()
  → SceneManager.LoadScene(otherScene, Single)
  → Current scene destroyed
  → New scene loads: ARSession resets, ARPlaneManager restarts, CoachingUI runs from start
  → User scans surfaces → taps to place Infinian/Dragon
```

```
Infinian placed (ObjectSpawner.objectSpawned fires)
  → InfinianPlacementLogic.OnInfinianSpawned() rotates to face camera
  → Animator plays Spawn state (default)
  → After Spawn clip ends (exit time 1.0) → transitions to CombatIdle loop
```

---

## Scene Build Settings

Both scenes must be added to `File > Build Settings > Scenes In Build`:
- Index 0: `Assets/Scenes/DragonScene.unity`
- Index 1: `Assets/Scenes/InfinianScene.unity`

SceneSwitcher references scenes by name string (robust to index reordering).

---

## Error Handling

- If FBX globalScale is already 0.01, skip — no change needed.
- If InfinianSetup finds no artifact children to disable, it exits silently.
- SceneSwitcher: if the other scene name is not in Build Settings, `LoadScene` will log an error. Mitigated by testing both scene names match exactly.

---

## Out of Scope

- Tap interaction on Infinian (user will specify later)
- Background music / audio
- Saving placed object position across scene switches
- Any additional animation triggers beyond Spawn → CombatIdle
