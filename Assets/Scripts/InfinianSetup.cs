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
