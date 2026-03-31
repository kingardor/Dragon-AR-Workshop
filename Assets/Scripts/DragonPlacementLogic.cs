using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DragonPlacementLogic : MonoBehaviour
{
    public ObjectSpawner spawner;

    void Start()
    {
        if (spawner != null)
            spawner.objectSpawned += OnDragonSpawned;
    }

    void OnDragonSpawned(GameObject newObj)
    {
        // Face the camera on spawn
        Vector3 forward = Camera.main.transform.position - newObj.transform.position;
        forward.y = 0;
        if (forward != Vector3.zero)
            newObj.transform.rotation = Quaternion.LookRotation(forward);
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.objectSpawned -= OnDragonSpawned;
    }
}
