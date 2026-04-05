using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticsManager : MonoBehaviour
{
    private InputDevice controller;

    void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        bool isRight = args.interactorObject.transform.name.ToLower().Contains("right");
        controller = InputDevices.GetDeviceAtXRNode(isRight ? XRNode.RightHand : XRNode.LeftHand);
    }

    public void setHandHaptics(bool isRightHand)
    {
        controller = InputDevices.GetDeviceAtXRNode(isRightHand ? XRNode.RightHand : XRNode.LeftHand);
        TriggerHaptic();
    }

    public void CustomTriggerHaptic(float amplitude, float duration)
    {
        if (controller.isValid && controller.TryGetHapticCapabilities(out HapticCapabilities capabilities))
            if (capabilities.supportsImpulse)
                controller.SendHapticImpulse(0, amplitude, duration);
    }

    public void TriggerHaptic()      => CustomTriggerHaptic(0.5f, 0.1f);
    public void HapticIncreaseBet()  => CustomTriggerHaptic(0.7f, 0.1f);
    public void HapticDecreaseBet()  => CustomTriggerHaptic(0.5f, 0.15f);

    public void HapticUntoggle()
    {
        CustomTriggerHaptic(0.5f, 0.05f);
        Invoke(nameof(SecondUntogglePulse), 0.07f);
    }

    private void SecondUntogglePulse() => CustomTriggerHaptic(0.5f, 0.05f);
}