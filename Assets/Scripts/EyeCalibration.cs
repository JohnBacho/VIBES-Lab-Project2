using System.Runtime.InteropServices;
using UnityEngine;

public class EyeCalibrationLauncher : MonoBehaviour
{
    [DllImport("wvr_api")]
    private static extern int WVR_StartEyeCalibration();

    void Start()
    {
        int result = WVR_StartEyeCalibration();
        Debug.Log("Eye calibration result: " + result);
    }
}