using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using sxr_internal;

[System.Serializable]
public enum OutcomeType
{
    Win,
    Loss,
    NearMiss,
    EffortTask,
}
[System.Serializable]
public enum GamblingType
{
    Slot,
    Parlay,
    EffortTask
}
[System.Serializable]
public class SlotTrialData
{   
    public OutcomeType outcome;
    public float multiplier = 2f;
    public int[] slotRow = new int[3];
}

[System.Serializable]
public enum GamblingTypeFirst
{
    Slot,
    Parlay
}

[System.Serializable]
public class ParlayTrialData
{
    public OutcomeType outcome;
    public bool[] leg3 = new bool[3];
    public bool[] leg4 = new bool[4];
    public bool[] leg5 = new bool[5];
}

public class Driver : MonoBehaviour
{
    [SerializeField] private GamblingTypeFirst gamblingTypeFirst = GamblingTypeFirst.Slot;
    [SerializeField] private SlotTrialData[] slotTrials = new SlotTrialData[16];
    [SerializeField] private ParlayTrialData[] parlayTrials = new ParlayTrialData[16];
    [SerializeField] private GameObject SlotMachine;
    [SerializeField] private GameObject Parlay;
    [SerializeField] private GameObject Tablet;
    [SerializeField] private GameObject EffortTask;
    [SerializeField] private ParlayHandler parlayHandler;
    [SerializeField] private SlotHandler slotHandler;
    [SerializeField] private GazeHandler gazeHandler;
    [SerializeField] XRInteractorLineVisual rayLineVisual;
    [SerializeField] private PlayOffMusic playOffMusicScript;
    [SerializeField] private EndProgram endProgramScript;
    [SerializeField] private List<Bucket> EffortTaskBucket;

    int offMusicToken = 0;
    const float slotTrialDuration = 30f;
    const float parlayTrialDuration = 105f;
    const float effortTaskDuration = 45f;
    const int lastTrialIndex = 16;
    const int lastPhase = 2; 
    const float disableTime = 2f;
    const float disableButtonBaseline = 1.1f;
    private ParlayTrialData currentparlayTrial;
    private SlotTrialData currentSlotTrial;
    void Start()
    {
        StartCoroutine(switchContextAfterDelay());
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        StartDataTrackers();
        StartNextTrial();
        if (rayLineVisual != null)
            rayLineVisual.enabled = false;
    }

    IEnumerator switchContextAfterDelay()
    {
        yield return null;
        EffortTask.SetActive(false);
        if(gamblingTypeFirst == GamblingTypeFirst.Slot)
        {
            slotHandler.DisableButtons(disableTime);
            SlotMachine.SetActive(true);
            Tablet.SetActive(false);
            Parlay.SetActive(false);
            sxr.SetProgramName("Lilac");
        }
        else
        {
            parlayHandler.DisableButtons(disableTime);
            SlotMachine.SetActive(false);
            Tablet.SetActive(true);
            Parlay.SetActive(true);
            sxr.SetProgramName("Sunflower");
        }
    }


    void StartDataTrackers()
    {
        sxr.StartRecordingCameraPos();
        sxr.StartRecordingEyeTrackerInfo();
        SetGamblingType();
    }

    public void StartNextTrial()
    {
        sxr.SetParlaySelection(",,,,,,,,,,");
        sxr.RestartTimer();
        if (sxr.GetTrial() >= lastTrialIndex)
        {
            if(sxr.GetPhase() >= lastPhase)
            {
                Debug.Log("All trials complete. Ending program.");
                Parlay.SetActive(false);
                SlotMachine.SetActive(false);
                endProgramScript.StartProgramEnding();
                return;
            } 
            else
            {
                SwitchGamblingtype();
            }
            return;
        }
        Debug.Log($"Starting Trial {sxr.GetTrial()}");
        if (SlotMachine.activeSelf)
        {
            currentSlotTrial = slotTrials[sxr.GetTrial()];
            if(currentSlotTrial.outcome == OutcomeType.EffortTask)
            {
                Debug.Log("Running Effort Task Trial");
                StartCoroutine(RunEffortTaskTrial());
                return;
            }
            else
            {
                StartCoroutine(RunSlotTrial(currentSlotTrial.outcome, currentSlotTrial.slotRow, currentSlotTrial.multiplier));
            }
        }
        else
        {
            currentparlayTrial = parlayTrials[sxr.GetTrial()];
            if(currentparlayTrial.outcome == OutcomeType.EffortTask)
            {
                Debug.Log("Running Effort Task Trial");
                StartCoroutine(RunEffortTaskTrial());
                return;
            }
            else
            {
                StartCoroutine(RunParlayTrial(currentparlayTrial.outcome));                
            }
        }
        offMusicToken++;
        slotHandler.DisableButtons(disableButtonBaseline);
        parlayHandler.DisableButtons(disableButtonBaseline);
        gazeHandler.StartBaseline();
        playOffMusicScript.StartPlayOffMusic(SlotMachine.activeSelf ? slotTrialDuration : parlayTrialDuration, offMusicToken);
        SetTypeOutcome();
    }

    private IEnumerator RunSlotTrial(OutcomeType outcome, int[] outcomeRow, float multiplier)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        slotHandler.SetOutcome(outcomeRow);
        slotHandler.SetMultiplier(multiplier);

        while (!slotHandler.TrialCompleted)
        {
            yield return null;
        }

        AdvanceTrialCounter();
        if(sxr.GetTrial() != lastTrialIndex) slotHandler.Reset();
        StartNextTrial();
    }

    private void SwitchGamblingtype()
    {
        sxr.NextPhase();
        SlotMachine.SetActive(!SlotMachine.activeSelf);
        Tablet.SetActive(!Tablet.activeSelf);
        Parlay.SetActive(!Parlay.activeSelf);
        slotHandler.DisableButtons(disableTime);
        parlayHandler.DisableButtons(disableTime);
        sxr.SetTotalLegs(0);
        sxr.SetTotalOdds(0f);
        SetGamblingType();
        StartNextTrial();
    }

    private IEnumerator RunEffortTaskTrial()
    {
        bool isSlot = SlotMachine.activeSelf;
        SetTypeOutcome();
        playOffMusicScript.CancelOffMusic();
        Debug.Log("Starting Effort Task Trial");
        sxr.SetGamblingType(GamblingType.EffortTask.ToString());
        SlotMachine.SetActive(false);
        Tablet.SetActive(false);
        Parlay.SetActive(false);
        if(isSlot)
        {
            for(int i = 0; i < EffortTaskBucket.Count; i++)
            {
                EffortTaskBucket[i].SetWallet(slotHandler);
            }
        }
        else
        {
            for(int i = 0; i < EffortTaskBucket.Count; i++)
            {
                EffortTaskBucket[i].SetWallet(parlayHandler);
            }
        }
        EffortTask.SetActive(true);
        Object.FindAnyObjectByType<BallSpawner>().StartSpawning();
        Object.FindAnyObjectByType<CountdownTimer>().StartCountdown();
        yield return new WaitForSeconds(effortTaskDuration);
        Object.FindAnyObjectByType<BallSpawner>().StopSpawning();
        Object.FindAnyObjectByType<BallSpawner>().DestroyAllBalls();
        AdvanceTrialCounter();

        EffortTask.SetActive(false);
        if(isSlot)
        {
            SlotMachine.SetActive(true);
            SetGamblingType();            
        }
        else
        {
            Parlay.SetActive(true);
            Tablet.SetActive(true);
            parlayHandler.UpdateLeaderboard();
            SetGamblingType();
        }
        StartNextTrial();

    }

    public void ParlayOutcome(int legCount)
    {
        int index = sxr.GetTrial();
        List<bool> Outcome = null;

        switch (legCount)
        {
            case 3:
                Outcome = new List<bool>(currentparlayTrial.leg3);
                break;

            case 4:
                Outcome = new List<bool>(currentparlayTrial.leg4);
                break;

            case 5:
                Outcome = new List<bool>(currentparlayTrial.leg5);
                break;

            default:
                Debug.LogError("Invalid parlay size");
                return;
        }

        parlayHandler.SetOutcome(Outcome);
    }

    private IEnumerator RunParlayTrial(OutcomeType outcome)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        parlayHandler.UpdateOddsText();
        while (!parlayHandler.TrialCompleted)
        {
            yield return null;
        }
        AdvanceTrialCounter();
        if(sxr.GetTrial() != lastTrialIndex) parlayHandler.Reset();
        StartNextTrial();
    }

    private void SetGamblingType()
    {
        if (SlotMachine.activeSelf)
        {
            sxr.SetGamblingType(GamblingType.Slot.ToString());
        }
        else if (Parlay.activeSelf)
        {
            sxr.SetGamblingType(GamblingType.Parlay.ToString());
        }
        else
        {
            sxr.SetGamblingType(GamblingType.EffortTask.ToString());
        }

    }

    private void SetTypeOutcome()
    {
        if (Parlay.activeSelf)
        {
            sxr.SetOutcome(currentparlayTrial.outcome.ToString());            
        }
        else if(SlotMachine.activeSelf)
        {
            sxr.SetOutcome(currentSlotTrial.outcome.ToString());
        }
        else
        {
            sxr.SetOutcome(OutcomeType.EffortTask.ToString());
        }
    }

    private void AdvanceTrialCounter()
    {
        Debug.Log($"Trial {sxr.GetTrial()} complete");
        if (!EffortTask.activeSelf)
        {
            playOffMusicScript.CancelOffMusic();            
        }
        gazeHandler.GrabPupilTrialAverage();
        sxr.NextTrial();
    }
}
