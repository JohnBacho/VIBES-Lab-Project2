using UnityEngine;
using UnityEngine.XR;

public class HapticsManager : MonoBehaviour
{
    InputDevice Controller;

    void Awake()
    {
        Controller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    public void setDominateHand(bool isRightHanded)
    {
        if (!isRightHanded)
        {
            Controller = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
    }
    public void CustomTriggerHaptic(float amplitude, float duration)
    {

        if (Controller.isValid && Controller.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
                Controller.SendHapticImpulse(0, amplitude, duration);
        }
    }

        public void TriggerHaptic()
    {
        float amplitude = 0.5f;
        float duration = 0.1f;

        if (Controller.isValid && Controller.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
                Controller.SendHapticImpulse(0, amplitude, duration);
        }
    }

    public void HapticIncreaseBet()
    {
        CustomTriggerHaptic(0.7f, 0.1f);
    }

    public void HapticDecreaseBet()
    {
        CustomTriggerHaptic(0.5f, 0.15f);
    }

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
