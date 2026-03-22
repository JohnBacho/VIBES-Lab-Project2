using System.Collections;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeTrackerManager : MonoBehaviour
{
    [Tooltip("Auto-launch calibration when the framework is ready")]
    [SerializeField] private bool EyeCalibration = true;
    [SerializeField] private float calibrationTimeout = 10f;
    [SerializeField] private bool enableRetry = true;
    [SerializeField] private float initialDelay = 2.5f;

    void Start()
    {
        if (EyeCalibration)
        {
            StartCoroutine(LaunchEyeCalibrationCoroutine());
        }
    }

    public IEnumerator LaunchEyeCalibrationCoroutine()
    {
        Debug.Log("[EyeTrackerManager] Waiting for SRanipal instance to exist...");
        yield return new WaitForSeconds(initialDelay);

        float timer = 0f;

        while (SRanipal_Eye_Framework.Instance == null && timer < calibrationTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (SRanipal_Eye_Framework.Instance == null)
        {
            Debug.LogError("[EyeTrackerManager] SRanipal_Eye_Framework instance never appeared. Is the framework object in the scene?");

            yield break;
        }

        Debug.Log("[EyeTrackerManager] Instance found. Waiting for WORKING status...");
        timer = 0f;

        while (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING && timer < calibrationTimeout)
        {
            if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT ||
                SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.STOP)
            {
                Debug.LogError($"[EyeTrackerManager] Framework entered terminal state: {SRanipal_Eye_Framework.Status}");
    
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
        {
            Debug.LogError($"[EyeTrackerManager] SRanipal did not become WORKING after {calibrationTimeout}s. Status: {SRanipal_Eye_Framework.Status}");

            yield break;
        }

        Debug.Log("[EyeTrackerManager] SRanipal is WORKING. Attempting calibration...");
        bool calibrationLaunched = TryLaunchEyeCalibration();

        if (!calibrationLaunched && enableRetry)
        {
            Debug.Log("[EyeTrackerManager] Calibration failed. Retrying in 0.5s...");
            yield return new WaitForSeconds(0.5f);
            calibrationLaunched = TryLaunchEyeCalibration();
        }

        if (calibrationLaunched)
        {
            Debug.Log("[EyeTrackerManager] Calibration launched. Verifying eye data...");
            yield return StartCoroutine(WaitAndCheckEyeData());
        }
        else
        {
            Debug.LogError("[EyeTrackerManager] Calibration failed after retry.");
        }
    }

    private bool TryLaunchEyeCalibration()
    {
        try
        {
            if (SRanipal_Eye.LaunchEyeCalibration()) return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[EyeTrackerManager] v1 calibration threw: {e.Message}");
        }

        try
        {
            if (SRanipal_Eye_v2.LaunchEyeCalibration()) return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[EyeTrackerManager] v2 calibration threw: {e.Message}");
        }

        Debug.LogWarning("[EyeTrackerManager] Both v1 and v2 calibration calls returned false.");
        return false;
    }

    private IEnumerator WaitAndCheckEyeData()
    {
        yield return new WaitForSeconds(5f);

        if (SRanipal_Eye_Framework.Instance == null)
        {
            Debug.LogError("[EyeTrackerManager] Framework instance lost during calibration wait.");

            yield break;
        }

        VerboseData data;
        bool valid = false;

        try
        {
            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback)
            {
                SRanipal_Eye.GetVerboseData(out data);
                valid = data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY) &&
                        data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
            }
            else
            {
                valid = SRanipal_Eye_v2.GetVerboseData(out data) &&
                        data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY) &&
                        data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[EyeTrackerManager] Exception reading eye data: {e.Message}");

            yield break;
        }

        if (!valid)
        {
            Debug.LogError("[EyeTrackerManager] Eye data invalid after calibration.");

        }
        else
        {
            Debug.Log("[EyeTrackerManager] Eye tracking data valid. Calibration successful.");
        }
    }
}