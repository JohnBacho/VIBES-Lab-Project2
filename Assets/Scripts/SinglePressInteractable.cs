using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SinglePressInteractable : MonoBehaviour
{
    private XRSimpleInteractable interactable;
    private float lastActivationTime = -999f;
    [SerializeField] private float cooldownTime = .7f; // 1 second cooldown for poke
    
    [SerializeField] private UnityEvent onButtonPressed;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        float currentTime = Time.time;
        float timeSinceLast = currentTime - lastActivationTime;
                
        if (timeSinceLast > cooldownTime)
        {
            lastActivationTime = currentTime;
            onButtonPressed?.Invoke();
        }
    }

    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }
}