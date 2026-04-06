using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VIVE.OpenXR.EyeTracker;
using VIVE.OpenXR.FacialTracking;
using sxr_internal;

namespace VIVE.OpenXR.Samples.FacialTracking
{
    public class GazeHandler2 : MonoBehaviour
    {
        public static GazeHandler2 Instance;

        private Camera vrCamera;
        private float flushTimer = 0f;

        private readonly StringBuilder writeBuffer = new StringBuilder(1024 * 64);
        private StreamWriter writer;

        private bool recordEyeTracker;
        private bool headerPrinted;

        private string filePath;

        private Vector3 combinedGazeOrigin;
        private Vector3 combinedGazeDirection;

        private Ray gazeRay;
        private RaycastHit hit;

        // -------- PUPIL / BASELINE --------
        private List<float> TempPupilStorage         = new List<float>();
        private List<float> TempBaselinePupilStorage  = new List<float>();
        private List<float> EventBaselinePupilStorage = new List<float>();

        private float leftPupilSize  = 0;
        private float rightPupilSize = 0;

        private float baseline           = 0f;
        private bool  baselineValid      = false;
        private bool  baselineInProgress = false;
        private bool  captureEventBaseline = false;

        private readonly float[] eyeExp = new float[14];
        private readonly float[] lipExp = new float[37];

        private static readonly XrEyeExpressionHTC[] EyeExprEnums =
        {
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_BLINK_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_WIDE_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_BLINK_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_WIDE_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_SQUEEZE_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_SQUEEZE_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_DOWN_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_DOWN_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_OUT_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_IN_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_IN_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_OUT_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_UP_HTC,
            XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_UP_HTC,
        };

        private static readonly XrLipExpressionHTC[] LipExprEnums =
        {
            XrLipExpressionHTC.XR_LIP_EXPRESSION_JAW_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_JAW_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_JAW_FORWARD_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_JAW_OPEN_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_APE_SHAPE_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_OVERTURN_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_OVERTURN_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_POUT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_RAISER_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_RAISER_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_STRETCHER_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_STRETCHER_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_CHEEK_PUFF_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_CHEEK_PUFF_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_CHEEK_SUCK_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_UPRIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_UPLEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_DOWNRIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_DOWNLEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_UPPER_INSIDE_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_INSIDE_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_LOWER_OVERLAY_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_LONGSTEP1_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_LEFT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_RIGHT_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_UP_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_DOWN_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_ROLL_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_LONGSTEP2_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_UPRIGHT_MORPH_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_UPLEFT_MORPH_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_DOWNRIGHT_MORPH_HTC,
            XrLipExpressionHTC.XR_LIP_EXPRESSION_TONGUE_DOWNLEFT_MORPH_HTC,
        };

        // -------- INIT --------
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject.transform.root);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            string folderPath = "/sdcard/Download/Experiments";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            filePath = Path.Combine(folderPath, $"eyetracker_{timestamp}.csv");

            writer = new StreamWriter(filePath, append: false, encoding: Encoding.UTF8, bufferSize: 65536);
            writer.AutoFlush = false;

            Debug.Log("Saving to: " + filePath);
        }

        void Start()
        {
            vrCamera = Camera.main;
        }

        void WriteHeader()
        {
            writer.WriteLine(
                "programName,date,localTime,unityTime,phase,trial,trialTimePassed,outcome,gamblingType,bet,payout,wallet," +
                "hardEffortTask,buttonPresses,totalOdds,totalLegs,parlaySelection," +
                "gazeOriginX,gazeOriginY,gazeOriginZ," +
                "gazeDirectionX,gazeDirectionY,gazeDirectionZ," +
                "leftPupil,rightPupil,combinedPupil," +
                "baselineCorrected,eventBaselineCorrected," +
                "hitX,hitY,hitZ,objectName," +
                "eyeLeftBlink,eyeLeftWide,eyeRightBlink,eyeRightWide," +
                "eyeLeftSqueeze,eyeRightSqueeze," +
                "eyeLeftDown,eyeRightDown," +
                "eyeLeftOut,eyeRightIn," +
                "eyeLeftIn,eyeRightOut," +
                "eyeLeftUp,eyeRightUp," +
                "jawRight,jawLeft,jawForward,jawOpen," +
                "mouthApeShape," +
                "mouthUpperRight,mouthUpperLeft," +
                "mouthLowerRight,mouthLowerLeft," +
                "mouthUpperOverturn,mouthLowerOverturn," +
                "mouthPout," +
                "mouthRaiserRight,mouthRaiserLeft," +
                "mouthStretcherRight,mouthStretcherLeft," +
                "cheekPuffRight,cheekPuffLeft,cheekSuck," +
                "mouthUpperUpright,mouthUpperUpleft," +
                "mouthLowerDownright,mouthLowerDownleft," +
                "mouthUpperInside,mouthLowerInside," +
                "mouthLowerOverlay," +
                "tongueLongstep1," +
                "tongueLeft,tongueRight,tongueUp,tongueDown," +
                "tongueRoll," +
                "tongueLongstep2," +
                "tongueUprightMorph,tongueUpleftMorph," +
                "tongueDownrightMorph,tongueDownleftMorph," +
                "trialAvgPupil,trialBaselineCorrectedPupil,eventBaselineCorrectedPupil"
            );
            headerPrinted = true;
        }

        void FlushToFile()
        {
            if (writeBuffer.Length == 0) return;
            writer.Write(writeBuffer);
            writer.Flush();
            writeBuffer.Clear();
        }

        void AppendFacialData(StringBuilder sb)
        {
            for (int i = 0; i < EyeExprEnums.Length; i++)
                eyeExp[i] = FacialTrackingData.EyeExpression(EyeExprEnums[i]);

            for (int i = 0; i < LipExprEnums.Length; i++)
                lipExp[i] = FacialTrackingData.LipExpression(LipExprEnums[i]);

            for (int i = 0; i < eyeExp.Length; i++)
            {
                sb.Append(eyeExp[i]);
                sb.Append(',');
            }

            for (int i = 0; i < lipExp.Length; i++)
            {
                sb.Append(lipExp[i]);
                if (i < lipExp.Length - 1) sb.Append(',');
            }
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
                AppendDataRow(trialAvg: null);
            }

            flushTimer += Time.deltaTime;
            if (flushTimer >= 5f)
            {
                FlushToFile();
                flushTimer = 0f;
            }
        }

        void AppendDataRow(float? trialAvg, float? baselineTrialAvg = null, float? eventBaselineTrialAvg = null)
        {
            UpdatePupil();

            var eh = ExperimentHandler.Instance;
            if (eh != null)
            {
                writeBuffer.Append(eh.ProgramName);        writeBuffer.Append(',');
                writeBuffer.Append(DateTime.Today.Month + "_" + DateTime.Today.Day); writeBuffer.Append(',');
                writeBuffer.Append(DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second); writeBuffer.Append(',');
                writeBuffer.Append(Time.time.ToString("F4")); writeBuffer.Append(',');
                writeBuffer.Append(eh.phase);              writeBuffer.Append(',');
                writeBuffer.Append(eh.trial);              writeBuffer.Append(',');
                writeBuffer.Append(eh.GetTimePassed());    writeBuffer.Append(',');
                writeBuffer.Append(eh.OutcomeInTrial);     writeBuffer.Append(',');
                writeBuffer.Append(eh.currentGamblingType);writeBuffer.Append(',');
                writeBuffer.Append(eh.BetAmount);          writeBuffer.Append(',');
                writeBuffer.Append(eh.CurrentPayout);      writeBuffer.Append(',');
                writeBuffer.Append(eh.wallet);             writeBuffer.Append(',');
                writeBuffer.Append(eh.HardEffortTask);     writeBuffer.Append(',');
                writeBuffer.Append(eh.ButtonPresses);      writeBuffer.Append(',');
                writeBuffer.Append(eh.TotalOdds);          writeBuffer.Append(',');
                writeBuffer.Append(eh.TotalLegs);          writeBuffer.Append(',');
                writeBuffer.Append('"'); writeBuffer.Append(eh.ParlaySelection); writeBuffer.Append('"'); writeBuffer.Append(',');
            }
            else
            {
                for (int i = 0; i < 17; i++) writeBuffer.Append(','); // was 15, now 17 columns
            }

            // ── cols 17-19: gaze origin ──
            writeBuffer.Append(combinedGazeOrigin.x); writeBuffer.Append(',');
            writeBuffer.Append(combinedGazeOrigin.y); writeBuffer.Append(',');
            writeBuffer.Append(combinedGazeOrigin.z); writeBuffer.Append(',');

            // ── cols 20-22: gaze direction ──
            writeBuffer.Append(combinedGazeDirection.x); writeBuffer.Append(',');
            writeBuffer.Append(combinedGazeDirection.y); writeBuffer.Append(',');
            writeBuffer.Append(combinedGazeDirection.z); writeBuffer.Append(',');

            // ── cols 23-25: pupils ──
            float? combined          = CombinedPupil();
            float? baselineCorrected = combined.HasValue ? BaselineCorrected(combined.Value) : null;
            float? eventBaseline     = (captureEventBaseline && baselineCorrected.HasValue)
                                           ? baselineCorrected
                                           : (float?)null;

            writeBuffer.Append(leftPupilSize);  writeBuffer.Append(',');
            writeBuffer.Append(rightPupilSize); writeBuffer.Append(',');
            if (combined.HasValue) { writeBuffer.Append(combined.Value); }
            writeBuffer.Append(',');

            // ── cols 26-27: baseline-corrected pupils ──
            if (baselineCorrected.HasValue) { writeBuffer.Append(baselineCorrected.Value); }
            writeBuffer.Append(',');
            if (eventBaseline.HasValue) { writeBuffer.Append(eventBaseline.Value); }
            writeBuffer.Append(',');

            // ── cols 28-31: raycast hit ──
            string hitX = "", hitY = "", hitZ = "", hitName = "";
            if (combinedGazeDirection != Vector3.zero)
            {
                Vector3 worldOrigin    = vrCamera.transform.TransformPoint(combinedGazeOrigin);
                Vector3 worldDirection = vrCamera.transform.TransformDirection(combinedGazeDirection);

                gazeRay = new Ray(worldOrigin, worldDirection);
                if (Physics.Raycast(gazeRay, out hit))
                {
                    hitX    = hit.point.x.ToString("F4");
                    hitY    = hit.point.y.ToString("F4");
                    hitZ    = hit.point.z.ToString("F4");
                    hitName = hit.collider.gameObject.name;
                }
            }

            writeBuffer.Append(hitX);    writeBuffer.Append(',');
            writeBuffer.Append(hitY);    writeBuffer.Append(',');
            writeBuffer.Append(hitZ);    writeBuffer.Append(',');
            writeBuffer.Append(hitName); writeBuffer.Append(',');

            // ── cols 32-82: facial expressions (14 eye + 37 lip) ──
            AppendFacialData(writeBuffer);

            // ── col 83-85: trial average pupils ──
            writeBuffer.Append(',');
            if (trialAvg.HasValue)          writeBuffer.Append(trialAvg.Value);
            writeBuffer.Append(',');
            if (baselineTrialAvg.HasValue)  writeBuffer.Append(baselineTrialAvg.Value);
            writeBuffer.Append(',');
            if (eventBaselineTrialAvg.HasValue) writeBuffer.Append(eventBaselineTrialAvg.Value);

            writeBuffer.Append('\n');
        }

        // -------- GAZE --------
        void UpdateGaze()
        {
            XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] gazes);

            var left  = gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var right = gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

            if (!left.isValid && !right.isValid) return;

            Vector3 leftPos  = left.gazePose.position.ToUnityVector();
            Vector3 rightPos = right.gazePose.position.ToUnityVector();

            Vector3 leftDir  = left.gazePose.orientation.ToUnityQuaternion()  * Vector3.forward;
            Vector3 rightDir = right.gazePose.orientation.ToUnityQuaternion() * Vector3.forward;

            combinedGazeOrigin    = (leftPos + rightPos) / 2f;
            combinedGazeDirection = (leftDir + rightDir).normalized;
        }

        // -------- PUPIL --------
        void UpdatePupil()
        {
            XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupils);

            var left  = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var right = pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

            leftPupilSize  = left.isDiameterValid  ? left.pupilDiameter  : -1;
            rightPupilSize = right.isDiameterValid ? right.pupilDiameter : -1;

            if (leftPupilSize  > 0) TempPupilStorage.Add(leftPupilSize);
            if (rightPupilSize > 0) TempPupilStorage.Add(rightPupilSize);
        }

        float? CombinedPupil()
        {
            if (leftPupilSize > 0 && rightPupilSize > 0)
                return (leftPupilSize + rightPupilSize) / 2f;
            if (leftPupilSize  > 0) return leftPupilSize;
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

        // -------- TRIAL AVG --------
        public void GrabPupilTrialAverage()
        {
            if (TempPupilStorage.Count == 0) return;

            float  trialAvg          = TempPupilStorage.Average();
            float? baselineAvg       = (!baselineValid || TempBaselinePupilStorage.Count == 0)
                                        ? (float?)null
                                        : TempBaselinePupilStorage.Average();
            float? eventBaselineAvg  = (!baselineValid || EventBaselinePupilStorage.Count == 0)
                                        ? (float?)null
                                        : EventBaselinePupilStorage.Average();

            AppendDataRow(trialAvg: trialAvg, baselineTrialAvg: baselineAvg, eventBaselineTrialAvg: eventBaselineAvg);

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
            baselineValid      = false;

            yield return new WaitForSeconds(1f);

            if (TempPupilStorage.Count > 0)
            {
                baseline      = TempPupilStorage.Average();
                baselineValid = true;
            }
            else
            {
                baseline      = 0f;
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
            writer?.Close();
        }

        void OnDestroy()
        {
            FlushToFile();
            writer?.Close();
        }
    }
}