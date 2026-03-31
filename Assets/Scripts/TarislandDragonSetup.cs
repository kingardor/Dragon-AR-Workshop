using UnityEngine;

// Run before XRGrabInteractable (DefaultExecutionOrder(-1)) so the
// BoxCollider exists when XRIT scans for colliders in Awake.
[DefaultExecutionOrder(-1000)]
public class TarislandDragonSetup : MonoBehaviour
{
    // In the Inspector: add the exact names of any floor/artifact child GameObjects
    // found when expanding the FBX hierarchy (e.g. "Plane001", "Ground").
    // Leave empty if none exist.
    [SerializeField] string[] artifactChildNames = {};

    void Awake()
    {
        foreach (var childName in artifactChildNames)
            SetChildActive(childName, false);

        var col = GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        // Compensate for FBX import scale (check Model Import Settings → Scale Factor).
        // The Black Dragon FBX uses 0.01 (cm→m). Adjust if this model differs.
        // Note: assumes uniform scale. If non-uniform, use lossyScale.y for height.
        float invScale = 1f / Mathf.Abs(transform.lossyScale.x);
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
