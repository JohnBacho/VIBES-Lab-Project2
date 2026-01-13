using UnityEngine;
using UnityEngine.XR;

public class HapticsManager : MonoBehaviour
{

    public void CustomTriggerHaptic(float amplitude, float duration)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (device.isValid && device.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
                device.SendHapticImpulse(0, amplitude, duration);
        }
    }

        public void TriggerHaptic()
    {
        float amplitude = 0.5f;
        float duration = 0.1f;
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (device.isValid && device.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(0, amplitude, duration);
            }
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

