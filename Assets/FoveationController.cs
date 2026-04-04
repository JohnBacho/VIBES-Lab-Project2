using VIVE.OpenXR;
using UnityEngine;
using VIVE.OpenXR.Foveation;
using VIVE.OpenXR.Feature;
 namespace VIVE.OpenXR
{
 public class FoveationController : MonoBehaviour
{
    void Start()
    {
        var configs = new Foveation.XrFoveationConfigurationHTC[]
        {
            new Foveation.XrFoveationConfigurationHTC
            {
                level = Foveation.XrFoveationLevelHTC.XR_FOVEATION_LEVEL_HIGH_HTC,
                clearFovDegree = 45f
            },
            new Foveation.XrFoveationConfigurationHTC
            {
                level = Foveation.XrFoveationLevelHTC.XR_FOVEATION_LEVEL_HIGH_HTC,
                clearFovDegree = 45f
            }
        };

        XrResult result = ViveFoveation.ApplyFoveationHTC(
            Foveation.XrFoveationModeHTC.XR_FOVEATION_MODE_FIXED_HTC,
            (uint)configs.Length,
            configs
        );

        Debug.Log("Foveation result: " + result);
    }
}   
}