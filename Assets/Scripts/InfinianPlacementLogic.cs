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
