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
    
    void Start()
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
    
    private void OnPokeEntered(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor && !hasTriggered)
        {            
            onButtonPressed?.Invoke();
            
            hasTriggered = true;
        }
    }
    
    private void OnPokeExited(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor)
        {            
            onButtonReleased?.Invoke();
            
            hasTriggered = false;
        }
    }
}