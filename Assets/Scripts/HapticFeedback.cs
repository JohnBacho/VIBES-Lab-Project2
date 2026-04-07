using UnityEngine;
using UnityEngine.XR;

public class HapticsManager : MonoBehaviour
{
    private bool isRightHanded = true;

    private InputDevice GetController()
    {
        return InputDevices.GetDeviceAtXRNode(
            isRightHanded ? XRNode.RightHand : XRNode.LeftHand
        );
    }

    public void setDominateHand(bool rightHanded)
    {
        isRightHanded = rightHanded;
    }

    public void CustomTriggerHaptic(float amplitude, float duration)
    {
        InputDevice controller = GetController();
        if (controller.isValid && controller.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
                controller.SendHapticImpulse(0, amplitude, duration);
        }
    }

    public void TriggerHaptic()
    {
        CustomTriggerHaptic(0.5f, 0.1f);
    }

    public void HapticIncreaseBet()  { CustomTriggerHaptic(0.7f, 0.1f); }
    public void HapticDecreaseBet()  { CustomTriggerHaptic(0.5f, 0.15f); }

    public void HapticUntoggle()
    {
        CustomTriggerHaptic(0.5f, 0.05f);
        Invoke(nameof(SecondUntogglePulse), 0.07f);
    }

    private void SecondUntogglePulse()
    {
        CustomTriggerHaptic(0.5f, 0.05f);
    }
}