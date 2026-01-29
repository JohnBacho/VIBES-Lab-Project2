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
    [SerializeField] private Image targetImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color toggledColor = Color.green;
    private Color disabledColor = Color.grey;

    [Header("Toggle Events")]
    [SerializeField] private UnityEvent onToggledOn;
    [SerializeField] private UnityEvent onToggledOff;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLog = true;

    private static readonly float volume = 0.2f;
    private static readonly float pitch = 2.2f;


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

            if(targetImage.color == toggledColor && interactable.enabled)
            {
                isToggled = false;
            }

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

    public void DisableInteraction()
    {
        isInteractable = false;
    }

    public void EnableInteraction()
    {
        isInteractable = true;
    }

    public void ForceReset()
    {
        isToggled = false;
        lastInteractionTime = Time.time;
        if (interactable != null)
            interactable.enabled = true;
    }

    public void PlayUISound()
    {
        SoundManager.SoundManager.PlaySound3D(SoundType.uiButton, transform.position, volume, pitch);
    }


}