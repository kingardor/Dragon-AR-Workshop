using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(XRGrabInteractable))]
public class TarislandDragonInteraction : MonoBehaviour
{
    [SerializeField] float holdToMoveDelay = 0.35f;

    // Populate in the Inspector with the exact Animator state names (Task 6, Step 5).
    // Index 0 = idle/rest (played by Animator Controller default, NOT by tap).
    // Index 1+ = action clips cycled on each tap.
    // Example: { "Idle", "Fly", "Attack", "Death" }
    [SerializeField] string[] animationStates = {};

    Animator animator;
    XRGrabInteractable grabInteractable;

    int currentAnimIndex;
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

    void OnDestroy()
    {
        if (Application.isPlaying)
            DragonEvents.NotifyDestroyed();
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

        if (!movedDuringHold && animationStates != null && animationStates.Length > 0)
        {
            currentAnimIndex = (currentAnimIndex + 1) % animationStates.Length;
            animator.Play(animationStates[currentAnimIndex]);
        }
    }
}
