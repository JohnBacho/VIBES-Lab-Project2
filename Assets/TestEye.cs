using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VIVE.OpenXR.EyeTracker;
using VIVE.OpenXR.FacialTracking;

namespace VIVE.OpenXR.Samples.FacialTracking
{
    public class GazeHandler : MonoBehaviour
    {
        public static GazeHandler Instance;

        private Camera vrCamera;

        private string toWrite = "";
        private bool recordEyeTracker;
        private bool headerPrinted;

        private string filePath;

        private Vector3 combinedGazeOrigin;
        private Vector3 combinedGazeDirection;

        private Ray gazeRay;
        private RaycastHit hit;

        // -------- PUPIL / BASELINE --------
        private List<float> TempPupilStorage = new List<float>();
        private List<float> TempBaselinePupilStorage = new List<float>();
        private List<float> EventBaselinePupilStorage = new List<float>();

        private float leftPupilSize = 0;
        private float rightPupilSize = 0;

        private float baseline = 0f;
        private bool baselineValid = false;
        private bool baselineInProgress = false;
        private bool captureEventBaseline = false;

        // -------- INIT --------
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject.transform.root);
            }
            else Destroy(gameObject);

            string folderPath = Path.Combine(Application.dataPath, "Experiments");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            filePath = Path.Combine(folderPath, $"eyetracker_{timestamp}.csv");
        }

        void Start()
        {
            vrCamera = Camera.main;
        }

        // -------- FILE WRITING --------
        void WriteHeader()
        {
            string header =
                "time," +
                "gazeOriginX,gazeOriginY,gazeOriginZ," +
                "gazeDirX,gazeDirY,gazeDirZ," +
                "leftPupil,rightPupil,combinedPupil," +
                "baselineCorrected,eventBaselineCorrected," +
                "hitX,hitY,hitZ,objectName," +

                // 👁 Eye expressions
                "eyeBlinkLeft,eyeBlinkRight,eyeWideLeft,eyeWideRight,eyeSqueezeLeft,eyeSqueezeRight," +

                // 👄 Lip expressions
                "jawOpen,smileLeft,smileRight,pucker," +

                "trialAvgPupil\n";

            File.WriteAllText(filePath, header);
            headerPrinted = true;
        }

        void FlushToFile()
        {
            if (toWrite == "") return;

            File.AppendAllText(filePath, toWrite);
            toWrite = "";
        }

string GetFacialData()
{
    // Eye expressions
    float eyeBlinkLeft = FacialTrackingData.EyeExpression(XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_BLINK_HTC);
    float eyeBlinkRight = FacialTrackingData.EyeExpression(XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_BLINK_HTC);

    float eyeWideLeft = FacialTrackingData.EyeExpression(XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_WIDE_HTC);
    float eyeWideRight = FacialTrackingData.EyeExpression(XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_WIDE_HTC);

    // Lip / Mouth expressions (valid enum values)
    float jawOpen = FacialTrackingData.LipExpression(XrLipExpressionHTC.XR_LIP_EXPRESSION_JAW_OPEN_HTC);
    float mouthPout = FacialTrackingData.LipExpression(XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_POUT_HTC);
    float mouthUpperRight = FacialTrackingData.LipExpression(XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_RIGHT_HTC);
    float mouthUpperLeft = FacialTrackingData.LipExpression(XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_LEFT_HTC);
    float mouthLowerRight = FacialTrackingData.LipExpression(XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_RIGHT_HTC);
    float mouthLowerLeft = FacialTrackingData.LipExpression(XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_LEFT_HTC);

    return $"{eyeBlinkLeft},{eyeBlinkRight},{eyeWideLeft},{eyeWideRight}," +
           $"{jawOpen},{mouthPout},{mouthUpperRight},{mouthUpperLeft},{mouthLowerRight},{mouthLowerLeft}";
}

        // -------- RECORDING --------
        public void StartRecording()
        {
            if (!headerPrinted) WriteHeader();
            recordEyeTracker = true;
        }

        public void PauseRecording()
        {
            recordEyeTracker = false;
            FlushToFile();
        }

        public bool RecordingGaze() => recordEyeTracker;

        // -------- UPDATE --------
        void Update()
        {
            UpdateGaze();

            if (recordEyeTracker)
            {
                toWrite += Time.time.ToString("F4") + "," + GetDataString() + "\n";
            }
        }

        // -------- GAZE --------
        void UpdateGaze()
        {
            XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] gazes);

            var left = gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var right = gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

            if (!left.isValid && !right.isValid) return;

            Vector3 leftPos = left.gazePose.position.ToUnityVector();
            Vector3 rightPos = right.gazePose.position.ToUnityVector();

            Vector3 leftDir = left.gazePose.orientation.ToUnityQuaternion() * Vector3.forward;
            Vector3 rightDir = right.gazePose.orientation.ToUnityQuaternion() * Vector3.forward;

            combinedGazeOrigin = (leftPos + rightPos) / 2f;
            combinedGazeDirection = (leftDir + rightDir).normalized;
        }

        // -------- PUPIL --------
        void UpdatePupil()
        {
            XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupils);

            var left = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var right = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

            leftPupilSize = left.isDiameterValid ? left.pupilDiameter : -1;
            rightPupilSize = right.isDiameterValid ? right.pupilDiameter : -1;

            if (leftPupilSize > 0) TempPupilStorage.Add(leftPupilSize);
            if (rightPupilSize > 0) TempPupilStorage.Add(rightPupilSize);
        }

        float? CombinedPupil()
        {
            if (leftPupilSize > 0 && rightPupilSize > 0)
                return (leftPupilSize + rightPupilSize) / 2f;

            if (leftPupilSize > 0) return leftPupilSize;
            if (rightPupilSize > 0) return rightPupilSize;

            return null;
        }

        float? BaselineCorrected(float value)
        {
            if (!baselineValid || baselineInProgress) return null;

            float corrected = value - baseline;

            TempBaselinePupilStorage.Add(corrected);
            if (captureEventBaseline) EventBaselinePupilStorage.Add(corrected);

            return corrected;
        }

        // -------- RAYCAST --------
        string CheckFocus()
        {
            gazeRay = new Ray(combinedGazeOrigin, combinedGazeDirection);

            if (Physics.Raycast(gazeRay, out hit))
            {
                return $"{hit.point.x},{hit.point.y},{hit.point.z},{hit.collider.gameObject.name}";
            }

            return ",,,";
        }

        // -------- DATA --------
        string GetDataString()
        {
            UpdatePupil();

            float? combined = CombinedPupil();
            float? baselineCorrected = combined.HasValue ? BaselineCorrected(combined.Value) : null;

            return $"{combinedGazeOrigin.x},{combinedGazeOrigin.y},{combinedGazeOrigin.z}," +
                $"{combinedGazeDirection.x},{combinedGazeDirection.y},{combinedGazeDirection.z}," +
                $"{leftPupilSize},{rightPupilSize},{combined}," +
                $"{baselineCorrected}," +
                $"{(captureEventBaseline && baselineCorrected.HasValue ? baselineCorrected : null)}," +
                $"{CheckFocus()}," +
                $"{GetFacialData()}";
        }

        // -------- TRIAL AVG --------
        public void GrabPupilTrialAverage()
        {
            if (TempPupilStorage.Count == 0) return;

            float avg = TempPupilStorage.Average();

            toWrite += Time.time.ToString("F4") + "," + GetDataString() + $",{avg}\n";

            TempPupilStorage.Clear();
            TempBaselinePupilStorage.Clear();
            EventBaselinePupilStorage.Clear();
            captureEventBaseline = false;
        }

        // -------- BASELINE --------
        public void StartBaseline()
        {
            StartCoroutine(SetBaseline());
        }

        IEnumerator SetBaseline()
        {
            TempPupilStorage.Clear();
            baselineInProgress = true;
            baselineValid = false;

            yield return new WaitForSeconds(1f);

            if (TempPupilStorage.Count > 0)
            {
                baseline = TempPupilStorage.Average();
                baselineValid = true;
            }
            else
            {
                baseline = 0f;
                baselineValid = false;
            }

            baselineInProgress = false;

            TempPupilStorage.Clear();
            TempBaselinePupilStorage.Clear();
        }

        public void SetCaptureEventBaseline()
        {
            captureEventBaseline = true;
        }

        void OnApplicationQuit()
        {
            FlushToFile();
        }
    }
}