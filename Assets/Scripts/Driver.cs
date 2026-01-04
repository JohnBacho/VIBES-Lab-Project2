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
    public int[] leg3 = new int[3];
    public int[] leg4 = new int[4];
    public int[] leg5 = new int[5];
}

public class Driver : MonoBehaviour
{
    [SerializeField] private GamblingTypeFirst gamblingTypeFirst = GamblingTypeFirst.Slot;
    [SerializeField] private SlotTrialData[] slotTrials = new SlotTrialData[16];
    [SerializeField] private ParlayTrialData[] parlayTrials = new ParlayTrialData[16];
    [SerializeField] private GameObject SlotMachine;
    [SerializeField] private GameObject Parlay;
    [SerializeField] private GameObject EffortTask;
    [SerializeField] private BetManager betManager;
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
            SlotMachine.SetActive(true);
            Parlay.SetActive(false);
            sxr.SetProgramName("Lilac");
        }
        else
        {
            SlotMachine.SetActive(false);
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
        playOffMusicScript.StartPlayOffMusic(SlotMachine.activeSelf ? slotTrialDuration : parlayTrialDuration, offMusicToken);
        SetTypeOutcome();

    }

    private IEnumerator RunSlotTrial(OutcomeType outcome, int[] outcomeRow, float multiplier)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        slotHandler.SetOutcome(outcomeRow);
        slotHandler.SetMultiplier(multiplier);
        slotHandler.StartNewTrial();


        while (!slotHandler.TrialCompleted)
        {
            yield return null;
        }

        Debug.Log($"Trial {sxr.GetTrial()} complete");

        gazeHandler.GrabPupilTrialAverage();
        playOffMusicScript.CancelOffMusic();
        slotHandler.StartNewTrial();
        sxr.NextTrial();
        if(sxr.GetTrial() != lastTrialIndex) slotHandler.rest();
        StartNextTrial();
    }

    private void SwitchGamblingtype()
    {
        sxr.NextPhase();
        SlotMachine.SetActive(!SlotMachine.activeSelf);
        Parlay.SetActive(!Parlay.activeSelf);
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
                EffortTaskBucket[i].SetWallet(betManager);
            }
        }
        EffortTask.SetActive(true);
        Object.FindAnyObjectByType<BallSpawner>().StartSpawning();
        Object.FindAnyObjectByType<CountdownTimer>().StartCountdown();
        yield return new WaitForSeconds(effortTaskDuration);
        Object.FindAnyObjectByType<BallSpawner>().StopSpawning();
        Object.FindAnyObjectByType<BallSpawner>().DestroyAllBalls();
        gazeHandler.GrabPupilTrialAverage();
        sxr.NextTrial();

        EffortTask.SetActive(false);
        if(isSlot)
        {
            SlotMachine.SetActive(true);
            SetGamblingType();            
        }
        else
        {
            Parlay.SetActive(true);
            betManager.UpdateLeaderboard();
            SetGamblingType();
        }
        StartNextTrial();

    }

    public void ParlayOutcome(int legCount)
    {
        int index = sxr.GetTrial();
        List<int> Outcome = null;

        switch (legCount)
        {
            case 3:
                Outcome = new List<int>(currentparlayTrial.leg3);
                break;

            case 4:
                Outcome = new List<int>(currentparlayTrial.leg4);
                break;

            case 5:
                Outcome = new List<int>(currentparlayTrial.leg5);
                break;

            default:
                Debug.LogError("Invalid parlay size");
                return;
        }

        betManager.SetOutcome(Outcome);
    }

    private IEnumerator RunParlayTrial(OutcomeType outcome)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        betManager.StartNewTrial();
        betManager.UpdateOddsText();
        while (!betManager.TrialCompleted)
        {
            yield return null;
        }

        Debug.Log($"Trial {sxr.GetTrial()} complete");
        gazeHandler.GrabPupilTrialAverage();
        playOffMusicScript.CancelOffMusic();
        betManager.StartNewTrial();
        sxr.NextTrial();
        if(sxr.GetTrial() != lastTrialIndex) betManager.ResetRound();
        StartNextTrial();
    }

    void SetGamblingType()
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

    void SetTypeOutcome()
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
}
