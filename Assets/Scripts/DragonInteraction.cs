using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(XRGrabInteractable))]
public class DragonInteraction : MonoBehaviour
{
    // Touch must be held longer than this to count as "move" vs "tap"
    [SerializeField] float holdToMoveDelay = 0.35f;

    Animator animator;
    XRGrabInteractable grabInteractable;

    bool isAnimating;
    bool isSelected;
    bool movedDuringHold;
    float selectTime;
    Vector3 savedPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        isSelected = true;
        movedDuringHold = false;
        selectTime = Time.time;
        savedPosition = transform.position;
    }

    void Update()
    {
        if (!isSelected) return;

        if (Time.time - selectTime < holdToMoveDelay)
        {
            // Lock position during the tap threshold window
            transform.position = savedPosition;
        }
        else if (!movedDuringHold)
        {
            // Threshold passed — this is a hold, allow ARTransformer to move freely
            movedDuringHold = true;
        }
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        isSelected = false;

        if (!movedDuringHold)
        {
            // Short tap: toggle animation
            isAnimating = !isAnimating;
            animator.SetBool("IsIdle", isAnimating);
        }
        // Long hold (drag/reposition): leave animation state unchanged
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
            DragonEvents.NotifyDestroyed();
    }
}
