using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DragonSelector : MonoBehaviour
{
    [SerializeField] ObjectSpawner objectSpawner;

    // One entry per dragon card, in the same order as ObjectSpawner.objectPrefabs
    // (index 0 = Black Dragon, index 1 = Tarisland).
    [SerializeField] DragonCardUI[] cards;

    void Start()
    {
        SelectDragon(0);
    }

    // Called by each card's Button.onClick via the Inspector.
    // Wire each card's Button → DragonSelector.SelectDragon(N) in the scene.
    public void SelectDragon(int index)
    {
        if (objectSpawner == null || index < 0 || index >= cards.Length) return;
        objectSpawner.spawnOptionIndex = index;
        for (int i = 0; i < cards.Length; i++)
            cards[i].SetSelected(i == index);
    }
}
