# Infinian Scene Integration — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add an InfinianScene alongside DragonScene, featuring the Infinian Lineage Series character with Spawn→CombatIdle animation, full drag/pinch gesture support, and a top-right toggle button that switches between the two scenes.

**Architecture:** Duplicate DragonScene into InfinianScene (removing dragon-selector UI), create InfinianPrefab with the same XRGrabInteractable+ARTransformer stack as the dragon prefabs, and add an identical SceneSwitcher Canvas to both scenes. LoadScene(Single) destroys the current scene and restarts AR/CoachingUI fresh.

**Tech Stack:** Unity 6000.4.0f1, ARFoundation 6.4.1, XR Interaction Toolkit 3.4.0, URP 17.4.0, C# MonoBehaviours, Unity YAML assets

---

## File Map

| Action | Path | Responsibility |
|--------|------|----------------|
| Modify | `Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX.meta` | Fix globalScale 1→0.01 |
| Overwrite | `Assets/infinian-lineage-series/source/Mon_Infinian_001_A_MI.mat` | URP Lit material for part A |
| Overwrite | `Assets/infinian-lineage-series/source/Mon_Infinian_001_B_MI.mat` | URP Lit material for part B |
| Create | `Assets/Scripts/InfinianSetup.cs` | Disable artifacts + size BoxCollider at DefaultExecutionOrder(-1000) |
| Create | `Assets/Scripts/InfinianPlacementLogic.cs` | Face-camera on spawn + delete suppression |
| Create | `Assets/Scripts/SceneSwitcher.cs` | Top-right toggle that loads the other scene |
| Create | `Assets/InfinianAnimatorController.controller` | Spawn (default) → exit-time → CombatIdle |
| Create | `Assets/InfinianPrefab.prefab` | Full prefab with all components wired |
| Create | `Assets/Scenes/InfinianScene.unity` | Duplicate of DragonScene, Infinian-specific |
| Modify | `Assets/Scenes/DragonScene.unity` | Add SceneSwitcher Canvas |
| Modify | `ProjectSettings/EditorBuildSettings.asset` | Register both scenes |

---

## Task 1: Fix FBX Import Scale

The Infinian FBX uses centimetre units (like the Tarisland FBX). `globalScale` must be 0.01 so the model spawns at human-scale in AR.

**Files:**
- Modify: `Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX.meta`

- [ ] **Step 1: Edit the .meta file**

Open `Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX.meta`.
Find both occurrences of `globalScale: 1` (one under `meshes:`, one under `humanDescription:`) and change them to `globalScale: 0.01`.

The two relevant lines currently read:
```
    globalScale: 1          ← under meshes: section (line ~37)
...
    globalScale: 1          ← under humanDescription: section (line ~94)
```

Change both to:
```
    globalScale: 0.01
```

- [ ] **Step 2: Refresh Unity and confirm no import errors**

Use `mcp__UnityMCP__refresh_unity` then `mcp__UnityMCP__read_console`.
Expected: no errors about the FBX. The Infinian mesh should appear at roughly human-scale (not 100× too large).

- [ ] **Step 3: Commit**

```bash
git add Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX.meta
git commit -m "fix: set Infinian FBX globalScale to 0.01 (cm → m)"
```

---

## Task 2: Fix Material A (Mon_Infinian_001_A_MI.mat)

The file currently contains Unreal Engine export text. Replace it with Unity URP Lit YAML. The material's GUID (`e9c66409bb85f4a42a65a2e8e95e93a2`) must stay the same — only overwrite the file content, never the `.meta`.

**Texture GUIDs (from `.meta` files already present):**
- Albedo `_A_D.png`: `4bd5ff32c872f46bc8884a73924c38a2`
- Normal `_A_N.png`: `9377ffdbc023b42a2beb69aefe18a5a5`
- Metallic/ORM `_A_M.png`: `c3a529902bf784938be3fa48a2d492fe`

**Files:**
- Overwrite: `Assets/infinian-lineage-series/source/Mon_Infinian_001_A_MI.mat`

- [ ] **Step 1: Overwrite the material file**

Write the following content exactly to `Assets/infinian-lineage-series/source/Mon_Infinian_001_A_MI.mat`:

```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &-459692763446879162
MonoBehaviour:
  m_ObjectHideFlags: 11
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: d0353a89b1f911e48b9e16bdc9f2e058, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Unity.RenderPipelines.Universal.Editor::UnityEditor.Rendering.Universal.AssetVersion
  version: 10
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: Mon_Infinian_001_A_MI
  m_Shader: {fileID: 4800000, guid: 933532a4fcc9baf4fa0491de14d08ed7, type: 3}
  m_Parent: {fileID: 0}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords:
  - _NORMALMAP
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {}
  disabledShaderPasses: []
  m_LockedProperties: 
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {fileID: 2800000, guid: 4bd5ff32c872f46bc8884a73924c38a2, type: 3}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _BumpMap:
        m_Texture: {fileID: 2800000, guid: 9377ffdbc023b42a2beb69aefe18a5a5, type: 3}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _EmissionMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _MetallicGlossMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _OcclusionMap:
        m_Texture: {fileID: 2800000, guid: c3a529902bf784938be3fa48a2d492fe, type: 3}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    m_Ints: []
    m_Floats:
    - _BumpScale: 1
    - _Metallic: 0
    - _OcclusionStrength: 1
    - _Smoothness: 0.3
    m_Colors:
    - _BaseColor: {r: 1, g: 1, b: 1, a: 1}
    - _EmissionColor: {r: 0, g: 0, b: 0, a: 1}
  m_BuildTextureStacks: []
  m_AllowLocking: 1
```

- [ ] **Step 2: Refresh Unity and verify no material errors**

Use `mcp__UnityMCP__refresh_unity` then `mcp__UnityMCP__read_console`.
Expected: no errors about `Mon_Infinian_001_A_MI.mat`.

---

## Task 3: Fix Material B (Mon_Infinian_001_B_MI.mat)

Same fix as Task 2 for the B-part material.

**Texture GUIDs:**
- Albedo `_B_D.png`: `b274014103f404b1690356b3eabbc892`
- Normal `_B_N.png`: `00c1a7f30e1254d86b7a62dd3d42fbbb`
- Metallic/ORM `_B_M.png`: `6a15e6f4210584a0993a0d88454c20f7`

**Files:**
- Overwrite: `Assets/infinian-lineage-series/source/Mon_Infinian_001_B_MI.mat`

- [ ] **Step 1: Overwrite the material file**

Write the following content exactly to `Assets/infinian-lineage-series/source/Mon_Infinian_001_B_MI.mat`:

```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &-459692763446879162
MonoBehaviour:
  m_ObjectHideFlags: 11
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: d0353a89b1f911e48b9e16bdc9f2e058, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Unity.RenderPipelines.Universal.Editor::UnityEditor.Rendering.Universal.AssetVersion
  version: 10
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: Mon_Infinian_001_B_MI
  m_Shader: {fileID: 4800000, guid: 933532a4fcc9baf4fa0491de14d08ed7, type: 3}
  m_Parent: {fileID: 0}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords:
  - _NORMALMAP
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {}
  disabledShaderPasses: []
  m_LockedProperties: 
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {fileID: 2800000, guid: b274014103f404b1690356b3eabbc892, type: 3}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _BumpMap:
        m_Texture: {fileID: 2800000, guid: 00c1a7f30e1254d86b7a62dd3d42fbbb, type: 3}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _EmissionMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _MetallicGlossMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _OcclusionMap:
        m_Texture: {fileID: 2800000, guid: 6a15e6f4210584a0993a0d88454c20f7, type: 3}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    m_Ints: []
    m_Floats:
    - _BumpScale: 1
    - _Metallic: 0
    - _OcclusionStrength: 1
    - _Smoothness: 0.3
    m_Colors:
    - _BaseColor: {r: 1, g: 1, b: 1, a: 1}
    - _EmissionColor: {r: 0, g: 0, b: 0, a: 1}
  m_BuildTextureStacks: []
  m_AllowLocking: 1
```

- [ ] **Step 2: Refresh Unity and verify no errors**

Use `mcp__UnityMCP__refresh_unity` then `mcp__UnityMCP__read_console`.
Expected: no material load errors.

- [ ] **Step 3: Commit mat + meta fixes**

```bash
git add Assets/infinian-lineage-series/source/Mon_Infinian_001_A_MI.mat \
        Assets/infinian-lineage-series/source/Mon_Infinian_001_B_MI.mat
git commit -m "fix: replace Unreal-format Infinian .mat files with Unity URP Lit YAML"
```

---

## Task 4: Write InfinianSetup.cs

Mirrors `TarislandDragonSetup.cs`. Runs before `XRGrabInteractable.Awake()`, disables artifact children, and adds a world-correct BoxCollider.

**Files:**
- Create: `Assets/Scripts/InfinianSetup.cs`

- [ ] **Step 1: Create the script**

Create `Assets/Scripts/InfinianSetup.cs` with the following content:

```csharp
using UnityEngine;

// Run before XRGrabInteractable (DefaultExecutionOrder(-1)) so the
// BoxCollider exists when XRIT scans for colliders in Awake.
[DefaultExecutionOrder(-1000)]
public class InfinianSetup : MonoBehaviour
{
    // Populate in the Inspector with the exact names of any floor/artifact
    // child GameObjects found when expanding the FBX hierarchy.
    // Leave empty if none exist.
    [SerializeField] string[] artifactChildNames = {};

    void Awake()
    {
        foreach (var childName in artifactChildNames)
            SetChildActive(childName, false);

        var col = GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        float sx = Mathf.Abs(transform.lossyScale.x);
        float invScale = sx > 1e-6f ? 1f / sx : 1f;
        col.center = new Vector3(0f, 1f * invScale, 0f);
        col.size   = new Vector3(1.5f * invScale, 2f * invScale, 1.5f * invScale);
    }

    void SetChildActive(string childName, bool active)
    {
        var t = FindDeep(transform, childName);
        if (t != null) t.gameObject.SetActive(active);
    }

    Transform FindDeep(Transform parent, string targetName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName) return child;
            var result = FindDeep(child, targetName);
            if (result != null) return result;
        }
        return null;
    }
}
```

- [ ] **Step 2: Verify compilation**

Use `mcp__UnityMCP__read_console` and wait for compilation to complete (poll `editor_state` resource `isCompiling` field until false).
Expected: no errors mentioning `InfinianSetup`.

---

## Task 5: Write InfinianPlacementLogic.cs

Mirrors `DragonPlacementLogic.cs`. Handles face-camera rotation on spawn and tap-through delete suppression. Attach to the ObjectSpawner GameObject in InfinianScene.

**Files:**
- Create: `Assets/Scripts/InfinianPlacementLogic.cs`

- [ ] **Step 1: Create the script**

Create `Assets/Scripts/InfinianPlacementLogic.cs`:

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class InfinianPlacementLogic : MonoBehaviour
{
    public ObjectSpawner spawner;

    bool suppressNextSpawn;

    public bool HasInfinian => spawner != null && spawner.transform.childCount > 0;

    void OnEnable()
    {
        if (spawner != null)
            spawner.objectSpawned += OnInfinianSpawned;
    }

    void OnDisable()
    {
        if (spawner != null)
            spawner.objectSpawned -= OnInfinianSpawned;
    }

    void OnInfinianSpawned(GameObject newObj)
    {
        if (suppressNextSpawn)
        {
            Destroy(newObj);
            return;
        }

        DestroyAllChildren(except: newObj);

        Vector3 forward = Camera.main.transform.position - newObj.transform.position;
        forward.y = 0;
        if (forward != Vector3.zero)
            newObj.transform.rotation = Quaternion.LookRotation(forward);
    }

    // Wire as persistent onClick on the Delete button in InfinianScene.
    public void DeleteCurrentInfinian()
    {
        suppressNextSpawn = true;
        DestroyAllChildren();
        StartCoroutine(ClearSuppression());
    }

    void DestroyAllChildren(GameObject except = null)
    {
        for (int i = spawner.transform.childCount - 1; i >= 0; i--)
        {
            var child = spawner.transform.GetChild(i).gameObject;
            if (child != except)
                Destroy(child);
        }
    }

    IEnumerator ClearSuppression()
    {
        yield return null;
        yield return null;
        yield return null;
        suppressNextSpawn = false;
    }
}
```

- [ ] **Step 2: Verify compilation**

Use `mcp__UnityMCP__read_console`.
Expected: no errors.

---

## Task 6: Write SceneSwitcher.cs

Top-right toggle button. Reads the active scene name and loads the other scene with `LoadScene(Single)`.

**Files:**
- Create: `Assets/Scripts/SceneSwitcher.cs`

- [ ] **Step 1: Create the script**

Create `Assets/Scripts/SceneSwitcher.cs`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] string dragonSceneName = "DragonScene";
    [SerializeField] string infinianSceneName = "InfinianScene";

    // Wired to the toggle Button.onClick in the Inspector.
    public void SwitchScene()
    {
        string current = SceneManager.GetActiveScene().name;
        string target = current == dragonSceneName ? infinianSceneName : dragonSceneName;
        SceneManager.LoadScene(target, LoadSceneMode.Single);
    }
}
```

- [ ] **Step 2: Verify compilation**

Use `mcp__UnityMCP__read_console`.
Expected: no errors.

- [ ] **Step 3: Commit scripts**

```bash
git add Assets/Scripts/InfinianSetup.cs \
        Assets/Scripts/InfinianPlacementLogic.cs \
        Assets/Scripts/SceneSwitcher.cs
git commit -m "feat: add InfinianSetup, InfinianPlacementLogic, SceneSwitcher scripts"
```

---

## Task 7: Discover Infinian Animation Clip FileIDs

The Animator Controller YAML must reference the exact fileIDs of the animation clips embedded in the Infinian FBX. These are assigned by Unity on import and must be read from the Editor.

**FBX GUID:** `9a6490557b56b4030965d5faca0c80b2`

- [ ] **Step 1: List sub-assets of the Infinian FBX**

Call `mcp__UnityMCP__manage_asset` with:
```json
{
  "action": "get",
  "path": "infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX"
}
```

Look for sub-assets named `Mon_Infinian_001_Skeleton|Spawn` and `Mon_Infinian_001_Skeleton|CombatIdle` in the result. Record their `fileID` values — you need them for Task 8.

If `manage_asset` does not return sub-asset fileIDs, use:
```json
{
  "action": "list",
  "path": "infinian-lineage-series/source"
}
```
Then look at the generated `.meta` for any `internalIDToNameTable` entries that list clip names. Alternatively, open the FBX in Unity's Inspector (Project window → expand arrow) — the clip names and their Object IDs appear there.

- [ ] **Step 2: Record the two fileIDs**

Note them as:
- `SPAWN_FILE_ID` = (fileID for `Mon_Infinian_001_Skeleton|Spawn`)
- `COMBAT_IDLE_FILE_ID` = (fileID for `Mon_Infinian_001_Skeleton|CombatIdle`)

These are used in Task 8.

---

## Task 8: Create InfinianAnimatorController.controller

Two states: `Spawn` (default entry) and `CombatIdle`. Spawn transitions to CombatIdle at exit time (end of clip), with no loop on Spawn.

**Files:**
- Create: `Assets/InfinianAnimatorController.controller`

**FBX GUID:** `9a6490557b56b4030965d5faca0c80b2`
Replace `SPAWN_FILE_ID` and `COMBAT_IDLE_FILE_ID` with values discovered in Task 7.

- [ ] **Step 1: Write the controller file**

Create `Assets/InfinianAnimatorController.controller`:

```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1102 &1102000001
AnimatorState:
  serializedVersion: 6
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: Spawn
  m_Speed: 1
  m_CycleOffset: 0
  m_Transitions:
  - {fileID: 1109000001}
  m_StateMachineBehaviours: []
  m_Position: {x: 200, y: 0, z: 0}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 1
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {fileID: SPAWN_FILE_ID, guid: 9a6490557b56b4030965d5faca0c80b2, type: 3}
  m_Tag: 
  m_SpeedParameter: 
  m_MirrorParameter: 
  m_CycleOffsetParameter: 
  m_TimeParameter: 
--- !u!1102 &1102000002
AnimatorState:
  serializedVersion: 6
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: CombatIdle
  m_Speed: 1
  m_CycleOffset: 0
  m_Transitions: []
  m_StateMachineBehaviours: []
  m_Position: {x: 450, y: 0, z: 0}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 1
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {fileID: COMBAT_IDLE_FILE_ID, guid: 9a6490557b56b4030965d5faca0c80b2, type: 3}
  m_Tag: 
  m_SpeedParameter: 
  m_MirrorParameter: 
  m_CycleOffsetParameter: 
  m_TimeParameter: 
--- !u!1101 &1109000001
AnimatorStateTransition:
  serializedVersion: 3
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: 
  m_Conditions: []
  m_DstStateMachine: {fileID: 0}
  m_DstState: {fileID: 1102000002}
  m_Solo: 0
  m_Mute: 0
  m_IsExit: 0
  serializedVersion: 3
  m_TransitionDuration: 0.1
  m_TransitionOffset: 0
  m_ExitTime: 1
  m_HasExitTime: 1
  m_HasFixedDuration: 1
  m_InterruptionSource: 0
  m_OrderedInterruption: 1
  m_CanTransitionToSelf: 1
--- !u!1107 &1107000001
AnimatorStateMachine:
  serializedVersion: 7
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: Base Layer
  m_ChildStates:
  - serializedVersion: 1
    m_State: {fileID: 1102000001}
    m_Position: {x: 200, y: 0, z: 0}
  - serializedVersion: 1
    m_State: {fileID: 1102000002}
    m_Position: {x: 450, y: 0, z: 0}
  m_ChildStateMachines: []
  m_AnyStateTransitions: []
  m_EntryTransitions: []
  m_StateMachineTransitions: {}
  m_StateMachineBehaviours: []
  m_AnyStatePosition: {x: 50, y: 20, z: 0}
  m_EntryPosition: {x: 50, y: 120, z: 0}
  m_ExitPosition: {x: 800, y: 120, z: 0}
  m_ParentStateMachinePosition: {x: 800, y: 20, z: 0}
  m_DefaultState: {fileID: 1102000001}
--- !u!91 &9100000
AnimatorController:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: InfinianAnimatorController
  serializedVersion: 6
  m_AnimatorParameters: []
  m_AnimatorLayers:
  - serializedVersion: 5
    m_Name: Base Layer
    m_StateMachine: {fileID: 1107000001}
    m_Mask: {fileID: 0}
    m_Motions: []
    m_Behaviours: []
    m_BlendingMode: 0
    m_SyncedLayerIndex: -1
    m_DefaultWeight: 0
    m_IKPass: 0
    m_SyncedLayerAffectsTiming: 0
    m_Controller: {fileID: 9100000}
  m_EvaluateTransitionsOnStart: 1
```

- [ ] **Step 2: Refresh Unity and verify controller imports cleanly**

Use `mcp__UnityMCP__refresh_unity` then `mcp__UnityMCP__read_console`.
Expected: no errors. The controller asset should appear in `Assets/` in the Project window.

- [ ] **Step 3: Verify Spawn→CombatIdle transition in the Animator window**

Use `mcp__UnityMCP__manage_asset` with `action: "get"` on `InfinianAnimatorController.controller` and confirm both states are present. If Unity reports a missing motion reference (wrong fileID), re-run Task 7 step 1 to get the correct IDs and update the file.

- [ ] **Step 4: Commit**

```bash
git add Assets/InfinianAnimatorController.controller
git commit -m "feat: add InfinianAnimatorController (Spawn → CombatIdle)"
```

---

## Task 9: Create InfinianPrefab

Create the prefab in Unity Editor via MCP tools. The prefab root is an instance of the Infinian FBX with `Animator`, `BoxCollider`, `Rigidbody`, `XRGrabInteractable`, `ARTransformer`, and `InfinianSetup` components.

**FBX GUID:** `9a6490557b56b4030965d5faca0c80b2`  
**InfinianAnimatorController GUID:** discovered after Task 8 (check `Assets/InfinianAnimatorController.controller.meta`).

- [ ] **Step 1: Create a prefab from the FBX**

Use `mcp__UnityMCP__manage_prefabs` with:
```json
{
  "action": "create",
  "prefab_path": "Assets/InfinianPrefab.prefab",
  "source_path": "Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX"
}
```

- [ ] **Step 2: Add Rigidbody (kinematic)**

Use `mcp__UnityMCP__manage_components` with:
```json
{
  "action": "add",
  "target": "InfinianPrefab",
  "component_type": "Rigidbody",
  "properties": {
    "isKinematic": true,
    "useGravity": false
  }
}
```

- [ ] **Step 3: Add XRGrabInteractable**

Use `mcp__UnityMCP__manage_components` with:
```json
{
  "action": "add",
  "target": "InfinianPrefab",
  "component_type": "UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable",
  "properties": {
    "trackPosition": true,
    "trackRotation": true,
    "throwOnDetach": false
  }
}
```

- [ ] **Step 4: Add ARTransformer**

Use `mcp__UnityMCP__manage_components` with:
```json
{
  "action": "add",
  "target": "InfinianPrefab",
  "component_type": "UnityEngine.XR.Interaction.Toolkit.AR.ARTransformer"
}
```

- [ ] **Step 5: Add InfinianSetup**

Use `mcp__UnityMCP__manage_components` with:
```json
{
  "action": "add",
  "target": "InfinianPrefab",
  "component_type": "InfinianSetup"
}
```

- [ ] **Step 6: Set the Animator Controller**

Use `mcp__UnityMCP__manage_components` with:
```json
{
  "action": "modify",
  "target": "InfinianPrefab",
  "component_type": "Animator",
  "properties": {
    "runtimeAnimatorController": "Assets/InfinianAnimatorController.controller",
    "applyRootMotion": false
  }
}
```

- [ ] **Step 7: Read console for errors**

Use `mcp__UnityMCP__read_console`.
Expected: no missing component or script errors.

- [ ] **Step 8: Verify prefab in Unity**

Use `mcp__UnityMCP__manage_prefabs` with `action: "get"` on `Assets/InfinianPrefab.prefab` and confirm all components are present: `Animator`, `BoxCollider`, `Rigidbody`, `XRGrabInteractable`, `ARTransformer`, `InfinianSetup`.

- [ ] **Step 9: Commit**

```bash
git add Assets/InfinianPrefab.prefab Assets/InfinianPrefab.prefab.meta
git commit -m "feat: add InfinianPrefab with full AR interaction stack"
```

---

## Task 10: Create InfinianScene

Duplicate `DragonScene.unity` as `InfinianScene.unity`. Remove the dragon-selector panel. Wire the Infinian prefab into the ObjectSpawner and wire the Delete button to `InfinianPlacementLogic`.

- [ ] **Step 1: Duplicate the scene file**

```bash
cp Assets/Scenes/DragonScene.unity Assets/Scenes/InfinianScene.unity
```
Then use `mcp__UnityMCP__refresh_unity` so Unity discovers the new scene.

- [ ] **Step 2: Open InfinianScene**

Use `mcp__UnityMCP__manage_scene` with:
```json
{ "action": "load", "scene_path": "Assets/Scenes/InfinianScene.unity" }
```

- [ ] **Step 3: Remove the DragonSelectorPanel**

Use `mcp__UnityMCP__find_gameobjects` to locate the `DragonSelectorPanel` (or equivalent name for the card-selector UI). Then use `mcp__UnityMCP__manage_gameobject` with `action: "delete"` to remove it.

- [ ] **Step 4: Remove DragonSelector component from ObjectSpawner**

Use `mcp__UnityMCP__find_gameobjects` to find the GameObject with `DragonSelector` component (likely the ObjectSpawner or a parent). Use `mcp__UnityMCP__manage_components` with `action: "remove"` and `component_type: "DragonSelector"`.

- [ ] **Step 5: Replace DragonPlacementLogic with InfinianPlacementLogic**

First remove the old component:
```json
{
  "action": "remove",
  "target": "ObjectSpawner",
  "component_type": "DragonPlacementLogic"
}
```

Then add the new one:
```json
{
  "action": "add",
  "target": "ObjectSpawner",
  "component_type": "InfinianPlacementLogic"
}
```

Then set the `spawner` reference:
```json
{
  "action": "modify",
  "target": "ObjectSpawner",
  "component_type": "InfinianPlacementLogic",
  "properties": {
    "spawner": "ObjectSpawner"
  }
}
```

- [ ] **Step 6: Set InfinianPrefab on ObjectSpawner**

Use `mcp__UnityMCP__manage_components` to set the `objectPrefabs` list on the `ObjectSpawner` component (on the ObjectSpawner GameObject) to contain only `Assets/InfinianPrefab.prefab`:
```json
{
  "action": "modify",
  "target": "ObjectSpawner",
  "component_type": "ObjectSpawner",
  "properties": {
    "objectPrefabs": ["Assets/InfinianPrefab.prefab"],
    "spawnOptionIndex": 0
  }
}
```

- [ ] **Step 7: Wire Delete button to InfinianPlacementLogic.DeleteCurrentInfinian**

Use `mcp__UnityMCP__find_gameobjects` to locate the Delete button GameObject. Use `mcp__UnityMCP__manage_components` to add a persistent onClick listener pointing to `InfinianPlacementLogic.DeleteCurrentInfinian` on the ObjectSpawner GameObject.

- [ ] **Step 8: Save InfinianScene**

Use `mcp__UnityMCP__manage_scene` with:
```json
{ "action": "save" }
```

- [ ] **Step 9: Read console for errors**

Use `mcp__UnityMCP__read_console`.
Expected: no missing reference warnings.

---

## Task 11: Add SceneSwitcher UI to Both Scenes

Add an identical SceneSwitcher Canvas to InfinianScene and DragonScene. The button sits top-right, labelled with a swap icon or "Switch".

### InfinianScene

- [ ] **Step 1: Load InfinianScene (if not already active)**

Use `mcp__UnityMCP__manage_scene` with `action: "load"` and `scene_path: "Assets/Scenes/InfinianScene.unity"`.

- [ ] **Step 2: Create the SceneSwitcher Canvas**

Use `mcp__UnityMCP__manage_gameobject` with `action: "create"`:
```json
{
  "action": "create",
  "name": "SceneSwitcherCanvas",
  "components": ["Canvas", "CanvasScaler", "GraphicRaycaster"]
}
```

- [ ] **Step 3: Configure the Canvas**

Use `mcp__UnityMCP__manage_components` to set Canvas properties:
```json
{
  "action": "modify",
  "target": "SceneSwitcherCanvas",
  "component_type": "Canvas",
  "properties": {
    "renderMode": 0,
    "sortingOrder": 10
  }
}
```
(`renderMode: 0` = Screen Space Overlay, renders on top of everything.)

- [ ] **Step 4: Add a Button child**

Use `mcp__UnityMCP__manage_gameobject` with `action: "create"` to add a `SwitchButton` child under `SceneSwitcherCanvas`:
```json
{
  "action": "create",
  "name": "SwitchButton",
  "parent": "SceneSwitcherCanvas",
  "components": ["RectTransform", "Button", "Image"]
}
```

- [ ] **Step 5: Anchor the button top-right**

Use `mcp__UnityMCP__manage_components` to set RectTransform on `SwitchButton`:
```json
{
  "action": "modify",
  "target": "SwitchButton",
  "component_type": "RectTransform",
  "properties": {
    "anchorMin": {"x": 1, "y": 1},
    "anchorMax": {"x": 1, "y": 1},
    "pivot": {"x": 1, "y": 1},
    "anchoredPosition": {"x": -20, "y": -20},
    "sizeDelta": {"x": 80, "y": 80}
  }
}
```

- [ ] **Step 6: Add label text child**

Use `mcp__UnityMCP__manage_gameobject` to create a `Text (TMP)` child under `SwitchButton`:
```json
{
  "action": "create",
  "name": "Label",
  "parent": "SwitchButton",
  "components": ["RectTransform", "TMPro.TextMeshProUGUI"]
}
```

Set text to `"⇄"` (or `"Switch"`) via `mcp__UnityMCP__manage_components`.

- [ ] **Step 7: Add SceneSwitcher component to SceneSwitcherCanvas**

```json
{
  "action": "add",
  "target": "SceneSwitcherCanvas",
  "component_type": "SceneSwitcher",
  "properties": {
    "dragonSceneName": "DragonScene",
    "infinianSceneName": "InfinianScene"
  }
}
```

- [ ] **Step 8: Wire Button.onClick to SceneSwitcher.SwitchScene**

Use `mcp__UnityMCP__manage_components` to add a persistent onClick listener on the Button component of `SwitchButton`, pointing to `SceneSwitcher.SwitchScene` on `SceneSwitcherCanvas`.

- [ ] **Step 9: Save InfinianScene**

Use `mcp__UnityMCP__manage_scene` with `action: "save"`.

### DragonScene

- [ ] **Step 10: Load DragonScene**

Use `mcp__UnityMCP__manage_scene` with `action: "load"` and `scene_path: "Assets/Scenes/DragonScene.unity"`.

- [ ] **Step 11: Create the SceneSwitcher Canvas in DragonScene**

Use `mcp__UnityMCP__manage_gameobject` with `action: "create"`:
```json
{
  "action": "create",
  "name": "SceneSwitcherCanvas",
  "components": ["Canvas", "CanvasScaler", "GraphicRaycaster"]
}
```

- [ ] **Step 12: Configure the Canvas**

```json
{
  "action": "modify",
  "target": "SceneSwitcherCanvas",
  "component_type": "Canvas",
  "properties": {
    "renderMode": 0,
    "sortingOrder": 10
  }
}
```

- [ ] **Step 13: Add a Button child**

```json
{
  "action": "create",
  "name": "SwitchButton",
  "parent": "SceneSwitcherCanvas",
  "components": ["RectTransform", "Button", "Image"]
}
```

- [ ] **Step 14: Anchor the button top-right**

```json
{
  "action": "modify",
  "target": "SwitchButton",
  "component_type": "RectTransform",
  "properties": {
    "anchorMin": {"x": 1, "y": 1},
    "anchorMax": {"x": 1, "y": 1},
    "pivot": {"x": 1, "y": 1},
    "anchoredPosition": {"x": -20, "y": -20},
    "sizeDelta": {"x": 80, "y": 80}
  }
}
```

- [ ] **Step 15: Add label text child**

```json
{
  "action": "create",
  "name": "Label",
  "parent": "SwitchButton",
  "components": ["RectTransform", "TMPro.TextMeshProUGUI"]
}
```

Set text to `"⇄"` via `mcp__UnityMCP__manage_components`.

- [ ] **Step 16: Add SceneSwitcher component to SceneSwitcherCanvas**

```json
{
  "action": "add",
  "target": "SceneSwitcherCanvas",
  "component_type": "SceneSwitcher",
  "properties": {
    "dragonSceneName": "DragonScene",
    "infinianSceneName": "InfinianScene"
  }
}
```

- [ ] **Step 17: Wire Button.onClick to SceneSwitcher.SwitchScene**

Use `mcp__UnityMCP__manage_components` to add a persistent onClick listener on `SwitchButton`'s Button component, pointing to `SceneSwitcher.SwitchScene` on `SceneSwitcherCanvas`.

- [ ] **Step 18: Save DragonScene**

Use `mcp__UnityMCP__manage_scene` with `action: "save"`.

---

## Task 12: Register Both Scenes in Build Settings

Both scenes must be in `File > Build Settings > Scenes In Build` for `SceneManager.LoadScene` to work.

- [ ] **Step 1: Add scenes to build**

Use `mcp__UnityMCP__manage_editor` with:
```json
{
  "action": "set_build_scenes",
  "scenes": [
    "Assets/Scenes/DragonScene.unity",
    "Assets/Scenes/InfinianScene.unity"
  ]
}
```

If `manage_editor` does not support `set_build_scenes`, open `ProjectSettings/EditorBuildSettings.asset` and add the InfinianScene entry manually:

```yaml
  scenes:
  - enabled: 1
    path: Assets/Scenes/DragonScene.unity
    guid: <DragonScene_guid>
  - enabled: 1
    path: Assets/Scenes/InfinianScene.unity
    guid: <InfinianScene_guid>
```

Get the guid from `Assets/Scenes/InfinianScene.unity.meta` (created when the file was duplicated and Unity imported it).

- [ ] **Step 2: Verify in Editor**

Use `mcp__UnityMCP__manage_editor` with `action: "get_build_settings"` (or read `ProjectSettings/EditorBuildSettings.asset`) and confirm both scenes are listed and enabled.

---

## Task 13: Smoke Test + Final Commit

- [ ] **Step 1: Enter Play Mode and test InfinianScene**

Use `mcp__UnityMCP__manage_editor` with `action: "enter_play_mode"`.
In the simulator or device:
1. Plane detection coaching UI should appear.
2. Tap a surface → Infinian spawns, plays Spawn animation, transitions to CombatIdle loop.
3. Drag/pinch → repositions and scales correctly.
4. Delete button → Infinian disappears, no respawn.
5. SceneSwitcher toggle → DragonScene loads; coaching UI restarts; dragon tap-to-place works.
6. Toggle back → InfinianScene loads fresh.

- [ ] **Step 2: Check console for errors**

Use `mcp__UnityMCP__read_console`.
Expected: no errors or missing reference warnings.

- [ ] **Step 3: Exit Play Mode**

Use `mcp__UnityMCP__manage_editor` with `action: "exit_play_mode"`.

- [ ] **Step 4: Final commit**

```bash
git add Assets/Scenes/InfinianScene.unity \
        Assets/Scenes/InfinianScene.unity.meta \
        Assets/Scenes/DragonScene.unity \
        Assets/InfinianPrefab.prefab \
        Assets/InfinianPrefab.prefab.meta \
        Assets/InfinianAnimatorController.controller \
        Assets/InfinianAnimatorController.controller.meta \
        Assets/Scripts/InfinianSetup.cs \
        Assets/Scripts/InfinianSetup.cs.meta \
        Assets/Scripts/InfinianPlacementLogic.cs \
        Assets/Scripts/InfinianPlacementLogic.cs.meta \
        Assets/Scripts/SceneSwitcher.cs \
        Assets/Scripts/SceneSwitcher.cs.meta \
        Assets/infinian-lineage-series/source/Mon_Infinian_001_A_MI.mat \
        Assets/infinian-lineage-series/source/Mon_Infinian_001_B_MI.mat \
        Assets/infinian-lineage-series/source/Mon_Infinian_001_Skeleton.FBX.meta \
        ProjectSettings/EditorBuildSettings.asset
git commit -m "feat: add InfinianScene with Spawn→CombatIdle animation and SceneSwitcher toggle"
```
