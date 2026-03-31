using UnityEngine;

// Run before XRGrabInteractable (which uses DefaultExecutionOrder(-1))
// so the BoxCollider exists when XRIT scans for colliders in Awake.
[DefaultExecutionOrder(-1000)]
public class DragonSetup : MonoBehaviour
{
    void Awake()
    {
        // Disable FBX floor artifacts by name
        SetChildActive("Hemi", false);
        SetChildActive("Plane", false);

        // The FBX root has localScale 0.01 (cm → m conversion).
        // We add a BoxCollider directly on the root, sizing it in root-local
        // space so it covers the dragon in world space.
        var col = gameObject.GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        // Desired world-space extents: ~1.5 m wide, ~2 m tall, ~1.5 m deep,
        // centred ~1 m above the spawn point.
        float invScale = 1f / Mathf.Abs(transform.lossyScale.x);
        col.center = new Vector3(0f, 1f * invScale, 0f);
        col.size   = new Vector3(1.5f * invScale, 2f * invScale, 1.5f * invScale);
    }

    void SetChildActive(string childName, bool active)
    {
        var t = FindDeep(transform, childName);
        if (t != null)
            t.gameObject.SetActive(active);
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
