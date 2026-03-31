using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(XRGrabInteractable))]
public class DragonInteraction : MonoBehaviour
{
    // Touch must be held longer than this to count as "move" vs "tap"
    [SerializeField] float holdToMoveDelay = 1f;

    Animator animator;
    XRGrabInteractable grabInteractable;

    bool isAnimating;
    bool isSelected;
    bool movedDuringHold;
    float selectTime;
    Vector3 savedPosition;
    Vector3 savedScale;

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
        savedScale = transform.localScale;
    }

    void Update()
    {
        if (!isSelected) return;

        // A pinch gesture changes scale before holdToMoveDelay expires — treat it as a move.
        if (!movedDuringHold && transform.localScale != savedScale)
            movedDuringHold = true;

        if (Time.time - selectTime < holdToMoveDelay)
        {
            if (!movedDuringHold)
                transform.position = savedPosition;
        }
        else if (!movedDuringHold)
            movedDuringHold = true;
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
