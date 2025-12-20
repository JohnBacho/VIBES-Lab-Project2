using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedback : MonoBehaviour
{
    // Start is called before the first frame update
    public void TriggerHaptic()
    {
        sxr.SendHaptic(0.5f, 1.0f, true, 0);
    }
}
