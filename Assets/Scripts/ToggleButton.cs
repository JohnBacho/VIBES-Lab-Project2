using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPokeToggleButton : MonoBehaviour
{
    private XRSimpleInteractable interactable;
    private bool isToggled = false;
    private bool isInteractable = true;
    
    [Header("Interaction Settings")]
    [SerializeField] private float cooldownTime = 1f;
    private float lastInteractionTime = -999f;

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
        interactable.selectEntered.AddListener(OnPokeSelect);

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        SetNormalColor();
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnPokeSelect);
        }
    }

    private void OnPokeSelect(SelectEnterEventArgs args)
    {
        if (!isInteractable) return;

        if (Time.time - lastInteractionTime < cooldownTime)
            return;

        if (args.interactorObject is XRPokeInteractor)
        {
            lastInteractionTime = Time.time;
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
        isToggled = false;
        lastInteractionTime = -999f;
        if (interactable != null)
            interactable.enabled = true;
    }
}