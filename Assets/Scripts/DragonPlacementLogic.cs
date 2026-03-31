using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DragonPlacementLogic : MonoBehaviour
{
    public ObjectSpawner spawner;

    bool suppressNextSpawn;

    // Ground truth: is there actually a dragon in the scene right now?
    public bool HasDragon => spawner != null && spawner.transform.childCount > 0;

    void OnEnable()
    {
        if (spawner != null)
            spawner.objectSpawned += OnDragonSpawned;
    }

    void OnDisable()
    {
        if (spawner != null)
            spawner.objectSpawned -= OnDragonSpawned;
    }

    void OnDragonSpawned(GameObject newObj)
    {
        if (suppressNextSpawn)
        {
            Destroy(newObj);
            return;
        }

        // Destroy any leftover dragons (shouldn't happen, but belt-and-suspenders).
        DestroyAllChildren(except: newObj);

        // Rotate to face camera on spawn.
        Vector3 forward = Camera.main.transform.position - newObj.transform.position;
        forward.y = 0;
        if (forward != Vector3.zero)
            newObj.transform.rotation = Quaternion.LookRotation(forward);
    }

    // Wired as a persistent onClick listener on the Delete button.
    // Suppression is set synchronously here — before ARInteractorSpawnTrigger
    // processes the same touch in Update — so no tap-through respawn occurs.
    public void DeleteCurrentDragon()
    {
        suppressNextSpawn = true;
        DestroyAllChildren();
        StartCoroutine(ClearSuppression());
    }

    // Called by DragonSelector when the user picks a different dragon type.
    public void SwapDragon(int prefabIndex)
    {
        if (!HasDragon) return;
        var prefabs = spawner.objectPrefabs;
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count) return;

        var existing = spawner.transform.GetChild(0);
        var pos = existing.position;
        var rot = existing.rotation;

        DestroyAllChildren();
        Instantiate(prefabs[prefabIndex], pos, rot, spawner.transform);
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
