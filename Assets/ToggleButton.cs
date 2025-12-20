using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPokeToggleButton : MonoBehaviour
{
    private XRSimpleInteractable interactable;
    private bool hasTriggered = false;
    private bool isToggled = false;
    private bool isInteractable = true;


    [Header("Visuals")]
    public Image targetImage;
    public Color normalColor = Color.white;
    public Color toggledColor = Color.green;

    private Color disabledColor = Color.grey;


    [Header("Toggle Events")]
    public UnityEvent onToggledOn;
    public UnityEvent onToggledOff;

    [Header("Debug")]
    public bool enableDebugLog = true;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnPokeEntered);
        interactable.hoverExited.AddListener(OnPokeExited);

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        // Initialize to normal color
        SetNormalColor();
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
        if (!isInteractable) return;
        if (args.interactorObject is XRPokeInteractor && !hasTriggered)
        {
            hasTriggered = true;
            isToggled = !isToggled;

            if (enableDebugLog)
            {
                Debug.Log(
                    $"[XRPokeToggleButton] '{gameObject.name}' toggled → {isToggled}",
                    this
                );
            }

            if (isToggled)
                onToggledOn?.Invoke();
            else
                onToggledOff?.Invoke();
        }
    }

    private void OnPokeExited(HoverExitEventArgs args)
    {
        if (!interactable.enabled) return;

        if (args.interactorObject is XRPokeInteractor)
        {
            hasTriggered = false;
        }
    }

    public bool IsToggled() => isToggled;

    public void SetToggledColor()
    {
        if (targetImage != null)
            targetImage.color = toggledColor;

    if (interactable != null)
        interactable.enabled = true;
    }

    public void SetNormalColor()
    {
        if (targetImage != null)
            targetImage.color = normalColor;

    isInteractable = true;
    }

    public void SetDisableColor()
    {
        if (targetImage != null)
        {
            targetImage.color = disabledColor;
        }
        isInteractable = false;
    }
    public void SetToggled(bool value, bool fireEvents = false)
    {
        if (isToggled == value)
            return;

        isToggled = value;

        if (fireEvents)
        {
            if (isToggled)
                onToggledOn?.Invoke();
            else
                onToggledOff?.Invoke();
        }
    }

    public void ForceReset()
    {
        hasTriggered = false;
        isToggled = false;

        if (interactable != null)
            interactable.enabled = true;
    }

    void OnDisable()
    {
        hasTriggered = false;
    }

}
