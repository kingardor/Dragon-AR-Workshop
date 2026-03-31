using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DragonPlacementLogic : MonoBehaviour
{
    public ObjectSpawner spawner;

    GameObject currentDragon;

    void OnEnable()
    {
        if (spawner != null)
            spawner.objectSpawned += OnDragonSpawned;
        DragonEvents.OnDragonDestroyed += OnCurrentDragonDestroyed;
    }

    void OnDisable()
    {
        if (spawner != null)
            spawner.objectSpawned -= OnDragonSpawned;
        DragonEvents.OnDragonDestroyed -= OnCurrentDragonDestroyed;
    }

    void OnDragonSpawned(GameObject newObj)
    {
        if (currentDragon != null)
            Destroy(currentDragon);

        currentDragon = newObj;

        // Rotate to face the camera on spawn
        Vector3 forward = Camera.main.transform.position - newObj.transform.position;
        forward.y = 0;
        if (forward != Vector3.zero)
            newObj.transform.rotation = Quaternion.LookRotation(forward);
    }

    void OnCurrentDragonDestroyed()
    {
        currentDragon = null;
    }
}
