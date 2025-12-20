using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine;

public class PokeButton : MonoBehaviour
{
    private XRSimpleInteractable interactable;
    private bool hasTriggered = false;

    [Header("Button Events")]
    [Tooltip("Triggered once when the button is pressed down")]
    public UnityEvent onButtonPressed;
    [Tooltip("Triggered when the button is released")]
    public UnityEvent onButtonReleased;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnPokeEntered);
        interactable.hoverExited.AddListener(OnPokeExited);
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnPokeEntered);
            interactable.hoverExited.RemoveListener(OnPokeExited);
        }
    }

    public void OnPokeEntered(HoverEnterEventArgs args)
    {
        Debug.Log($"PokeButton OnPokeEntered called ${interactable.enabled}");
        if (!interactable.enabled) return;
        if (args.interactorObject is XRPokeInteractor && !hasTriggered)
        {
            onButtonPressed?.Invoke();
            hasTriggered = true;
        }
    }

    public void OnPokeExited(HoverExitEventArgs args)
    {
        if (!interactable.enabled) return;

        if (args.interactorObject is XRPokeInteractor)
        {
            onButtonReleased?.Invoke();
            hasTriggered = false;
        }
    }

    public void DisableButton()
    {
        if (interactable != null)  // Added null check as extra safety
            interactable.enabled = false;
            hasTriggered = true;
    }

    public void EnableButton()
    {
        if (interactable != null)
        {
            interactable.enabled = true;
            hasTriggered = false;            
        }

    }
}