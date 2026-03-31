using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragonCardUI : MonoBehaviour
{
    [SerializeField] Image cardBackground;
    [SerializeField] Image selectionRing;
    [SerializeField] TextMeshProUGUI nameLabel;

    [Header("Selected Colors")]
    [SerializeField] Color selectedBgColor     = new Color(0.10f, 0.12f, 0.25f, 0.95f);
    [SerializeField] Color selectedRingColor   = new Color(0.85f, 0.10f, 0.10f, 1.00f); // red
    [SerializeField] Color selectedTextColor   = Color.white;

    [Header("Unselected Colors")]
    [SerializeField] Color unselectedBgColor   = new Color(0.05f, 0.05f, 0.10f, 0.55f);
    [SerializeField] Color unselectedRingColor = new Color(1f, 1f, 1f, 0.10f);
    [SerializeField] Color unselectedTextColor = new Color(1f, 1f, 1f, 0.45f);

    public void SetSelected(bool selected)
    {
        if (cardBackground != null) cardBackground.color = selected ? selectedBgColor   : unselectedBgColor;
        if (selectionRing  != null) selectionRing.color  = selected ? selectedRingColor : unselectedRingColor;
        if (nameLabel      != null) nameLabel.color       = selected ? selectedTextColor : unselectedTextColor;
    }
}
