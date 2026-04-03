using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using InputDevice = UnityEngine.XR.InputDevice;
using ViveSR.anipal.Eye;

namespace sxr_internal
{
    using ViveSR.anipal.Eye;

    public class GazeHandler : MonoBehaviour {
        public static GazeHandler Instance;

        int lastUpdate; 
        Vector3 gazeOriginCombinedLocal, gazeDirectionCombinedLocal, previousGazeDirectionCombinedLocal;
        VerboseData verboseData = new VerboseData();
        Camera vrCamera; 

        private string toWrite = ""; 
        private string InFocusItem;
        private bool hasExecuted = false;
        private float start;
        private bool recordEyeTracker; 
        private bool headerPrinted;
        private string FocusedGameObject = ""; // used for Sranipal
        private Ray testRay; // used for Sranipal
        private FocusInfo focusInfo; // used for Sranipal
        private Vector3 gazeHitPoint; // used in calculating eye tracking data with collisions
        
        private List<float> TempPupilStorage = new List<float>();
        private List<float> TempBaselinePupilStorage = new List<float>();
        private List<float> EventBaselinePupilStorage = new List<float>();
        private float rightPupilSize = 0;
        private float leftPupilSize = 0;
        private float baseline = 0f;
        private bool baselineValid = false;
        private bool baselineInProgress = false;
        private bool captureEventBaseline = false;

        public void WriteEyeTrackerHeader() {
        ExperimentHandler.Instance.WriteHeaderToTaggedFile("eyetracker",
            "screenFixationX,screenFixationY,gazeFixationX,gazeFixationY," +
            "gazeFixationZ,localGazeX,localGazeY,localGazeZ,leftEyePositionX," +
            "leftEyePositionY,leftEyePositionZ,rightEyePositionX,rightEyePositionY,rightEyePositionZ," +
            "leftEyeRotationX,leftEyeRotationY,leftEyeRotationZ,rightEyeRotationX,rightEyeRotationY," +
            "rightEyeRotationZ,leftEyePupilSize,rightEyePupilSize,baselineLeftEyePupil,baselineRightEyePupil,combinedEyePupilSize,leftEyeOpenAmount,rightEyeOpenAmount," +
            "GazeHitPointX,GazeHitPointY,GazeHitPointZ,GameObjectInFocus,TrialAveragePupilSize,TrialBaselineCorrectedPupilSize,EventBaselineCorrectedPupilSize");

            headerPrinted=true;}
        
        public void StartRecording() {
            if (!headerPrinted) WriteEyeTrackerHeader();
            recordEyeTracker = true; }

        public void PauseRecording()
        {
            recordEyeTracker = false;
            if(toWrite != "") ExperimentHandler.Instance.WriteToTaggedFile("eyetracker", toWrite, includeTimeStepInfo:false);
            toWrite = ""; 
        }

        private string CheckFocusedObject()
        {

            if (!SRanipal_Eye.Focus(GazeIndex.COMBINE, out testRay, out focusInfo) &&
                !SRanipal_Eye.Focus(GazeIndex.LEFT, out testRay, out focusInfo) &&
                !SRanipal_Eye.Focus(GazeIndex.RIGHT, out testRay, out focusInfo))
            {
                return "," + "" + "," + "" + "," + "" + "," + "";
            }

            string focusedGameObject = focusInfo.collider.gameObject.name;
            Vector3 gazeHitPoint = focusInfo.point;
            return "," + gazeHitPoint.x + "," + gazeHitPoint.y + "," + gazeHitPoint.z + "," + focusedGameObject;
        }

        public bool RecordingGaze() { return recordEyeTracker; }

        public bool LaunchEyeCalibration()
        {
            if (SRanipal_Eye.LaunchEyeCalibration()) return true; 
            else if (SRanipal_Eye_v2.LaunchEyeCalibration()) return true;
            Debug.Log("Failed to complete eye calibration");
            return false; 
        }

        public string GetFullGazeInfo(){
            return (GetScreenFixationPoint() +","+ GazeFixation() +"," + GetGazeCombinedGazeRayLocal() + "," 
                    + LeftEyePosition() +","+ RightEyePosition() +","+
                    LeftEyeRotation() +","+ RightEyeRotation() +","+ LeftEyePupilSize() +","+ RightEyePupilSize() 
                    + "," + BaselineLeftEyePupilSize() + "," + BaselineRightEyePupilSize() + "," + CombinedEyePupilSize() 
                    + "," + LeftEyeOpenAmount() +","+ RightEyeOpenAmount()).Replace("(","").Replace(")","");
        }
        
    public void Update() {
        if (sxrSettings.Instance.RecordThisFrame() & recordEyeTracker) {
            toWrite += ExperimentHandler.Instance.timeStepToWriteInfo() 
                       + GetFullGazeInfo() 
                       + CheckFocusedObject()
                       + "," + null
                       + "\n";
        }
    }
        void UpdateGaze() {
            if (lastUpdate != sxrSettings.Instance.GetCurrentFrame()) {
                previousGazeDirectionCombinedLocal = gazeDirectionCombinedLocal;
                if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out gazeOriginCombinedLocal, out gazeDirectionCombinedLocal)) { }
                else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out gazeOriginCombinedLocal, out gazeDirectionCombinedLocal)) { }
                else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out gazeOriginCombinedLocal, out gazeDirectionCombinedLocal)) { }
                // else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOriginCombinedLocal,
                //     out gazeDirectionCombinedLocal)) { }
                // else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out gazeOriginCombinedLocal,
                //     out gazeDirectionCombinedLocal)) { }
                // else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out gazeOriginCombinedLocal,
                //     out gazeDirectionCombinedLocal)) { }
                else { Debug.LogWarning("Failed to find SRanipal Framework V1 (combinedGazeRayLocal), do you have the SDK installed (and the headset on)?"); }
                
                var interpolatedGazeDirection = UnityEngine.Vector3.Lerp(previousGazeDirectionCombinedLocal, gazeDirectionCombinedLocal,
                    sxrSettings.Instance.interpolateAmount * Time.unscaledDeltaTime);
                gazeDirectionCombinedLocal = sxrSettings.Instance.interpolateGaze
                    ? interpolatedGazeDirection.normalized
                    : gazeDirectionCombinedLocal.normalized;

                if (SRanipal_Eye.GetVerboseData(out verboseData)) { }
                else { Debug.LogWarning("Failed to find SRanipal Framework V1 (verboseData), do you have the SDK installed?"); }
            

                sxr.DebugLog(gazeOriginCombinedLocal.ToString());
                sxr.DebugLog(gazeDirectionCombinedLocal.ToString());

                lastUpdate = sxrSettings.Instance.GetCurrentFrame(); 
            } }
        
        public Vector3 GetGazeCombinedGazeRayLocal() {
            UpdateGaze();
            return gazeDirectionCombinedLocal; }
        public Vector3 GetGazeCombinedPositionLocal() {
            UpdateGaze();
            return gazeDirectionCombinedLocal; }

        public Vector2 GetScreenFixationPoint(){
            UpdateGaze();
            Camera vrCamera = sxrSettings.Instance.vrCamera;
            var screenPos =
                vrCamera.WorldToScreenPoint( GazeFixation());

            float gazeX = screenPos.x / vrCamera.pixelWidth;
            float gazeY = screenPos.y / vrCamera.pixelHeight; 

            return new Vector2(gazeX, gazeY); }

        public Vector3 GazeFixation()
        {
            UpdateGaze(); 
            var trans = sxrSettings.Instance.vrCamera.transform; 
            return trans.position + (trans.rotation.eulerAngles.normalized + gazeDirectionCombinedLocal); 
        }
        
        public Vector3 LeftEyePosition() {
            UpdateGaze();
            return verboseData.left.gaze_origin_mm; }

        public Vector3 RightEyePosition() {
            UpdateGaze();
            return verboseData.right.gaze_origin_mm; }
        
        public Vector3 LeftEyeRotation() {
            UpdateGaze();
            return verboseData.left.gaze_direction_normalized; }

        public Vector3 RightEyeRotation() {
            UpdateGaze();
            return verboseData.right.gaze_direction_normalized; }

        public float LeftEyeOpenAmount() {
            UpdateGaze();
            return verboseData.left.eye_openness; }

        public float RightEyeOpenAmount() {
            UpdateGaze();
            return verboseData.right.eye_openness; }

        public float? LeftEyePupilSize() {
            UpdateGaze();
            float value = verboseData.left.pupil_diameter_mm;
            if(value > 0)
            {
                TempPupilStorage.Add(value);
            }
            leftPupilSize = value;
            return value < 0 ? (float?)null : value;
        }

        public float? RightEyePupilSize() {
            UpdateGaze();
            float value = verboseData.right.pupil_diameter_mm;
            if(value > 0)
            {
                TempPupilStorage.Add(value);
            }
            rightPupilSize = value;
            return value < 0 ? (float?)null : value;
        }

        public float? BaselineLeftEyePupilSize()
        {
            UpdateGaze();

            if (!baselineValid || baselineInProgress)
                return null;

            float value = verboseData.left.pupil_diameter_mm;
            if (value <= 0)
                return null;

            float corrected = value - baseline;
            TempBaselinePupilStorage.Add(corrected);

            if(captureEventBaseline){
                EventBaselinePupilStorage.Add(corrected);
            }
            return corrected;
        }

        public float? BaselineRightEyePupilSize()
        {
            UpdateGaze();

            if (!baselineValid || baselineInProgress)
                return null;

            float value = verboseData.right.pupil_diameter_mm;
            if (value <= 0)
                return null;

            float corrected = value - baseline;
            TempBaselinePupilStorage.Add(corrected);

            if(captureEventBaseline){
                EventBaselinePupilStorage.Add(corrected);
            }
            return corrected;
        }

        public float? CombinedEyePupilSize()
        {
            UpdateGaze();

            if (rightPupilSize > 0 && leftPupilSize > 0)
            {
                float? combinedPupil = (rightPupilSize + leftPupilSize) / 2f;
                return combinedPupil;
            }
            else if (leftPupilSize > 0)
            {
                return leftPupilSize;
            }
            else if (rightPupilSize > 0)
            {
                return rightPupilSize;
            }
            else
            {
                return null;
            }
        }

        public void GrabPupilTrialAverage()
        {
            if (TempPupilStorage.Count == 0)
            {
                return;
            }

            if (recordEyeTracker)
            {
                if(!baselineValid || TempBaselinePupilStorage.Count == 0){
                    toWrite += ExperimentHandler.Instance.timeStepToWriteInfo() 
                    + GetFullGazeInfo() 
                    + CheckFocusedObject() 
                    + "," + TempPupilStorage.Average() + "," + ""
                    + "\n";

                    TempPupilStorage.Clear();
                    TempBaselinePupilStorage.Clear();
                    EventBaselinePupilStorage.Clear();
                    captureEventBaseline = false;
                } else if(!baselineValid || EventBaselinePupilStorage.Count ==0){
                    toWrite += ExperimentHandler.Instance.timeStepToWriteInfo() 
                            + GetFullGazeInfo() 
                            + CheckFocusedObject() 
                            + "," + TempPupilStorage.Average() + "," + TempBaselinePupilStorage.Average()
                            + "\n";

                    TempPupilStorage.Clear();
                    TempBaselinePupilStorage.Clear();
                    EventBaselinePupilStorage.Clear();
                    captureEventBaseline = false;
                } else{
                    toWrite += ExperimentHandler.Instance.timeStepToWriteInfo() 
                            + GetFullGazeInfo() 
                            + CheckFocusedObject() 
                            + "," + TempPupilStorage.Average() + "," + TempBaselinePupilStorage.Average() + "," + EventBaselinePupilStorage.Average()
                            + "\n";

                    TempPupilStorage.Clear();
                    TempBaselinePupilStorage.Clear();
                    EventBaselinePupilStorage.Clear();
                    captureEventBaseline = false;
                }
            }
        }

        public void StartBaseline()
        {
            StartCoroutine(SetBaseline());
        }

        private IEnumerator SetBaseline()
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

        public void SetCaptureEventBaseline(){
            captureEventBaseline = true;
        }

        private void OnApplicationQuit(){
            if(headerPrinted && toWrite != "")
                ExperimentHandler.Instance.WriteToTaggedFile("eyetracker", toWrite, includeTimeStepInfo:false);}

        void Start() { 
            vrCamera = sxrSettings.Instance.vrCamera; 
            gameObject.AddComponent<SRanipal_Eye_Framework>(); }
        void Awake() {
             if ( Instance == null) {Instance = this;  DontDestroyOnLoad(gameObject.transform.root);}
             else Destroy(gameObject); }
    }}

