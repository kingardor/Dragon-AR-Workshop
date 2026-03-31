using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DragonSelector : MonoBehaviour
{
    [SerializeField] ObjectSpawner objectSpawner;
    [SerializeField] DragonPlacementLogic placementLogic;

    // One entry per dragon card, in the same order as ObjectSpawner.objectPrefabs
    // (index 0 = Black Dragon, index 1 = Tarisland).
    [SerializeField] DragonCardUI[] cards;

    void Start()
    {
        SelectDragon(0);
    }

    // Called by each card's Button.onClick via the Inspector.
    public void SelectDragon(int index)
    {
        if (objectSpawner == null || cards == null || index < 0 || index >= cards.Length) return;

        objectSpawner.spawnOptionIndex = index;

        for (int i = 0; i < cards.Length; i++)
            cards[i].SetSelected(i == index);

        // If a dragon is already placed, swap it immediately.
        if (placementLogic != null && placementLogic.HasDragon)
            placementLogic.SwapDragon(index);
    }
}
