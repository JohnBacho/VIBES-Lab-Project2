using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using sxr_internal;


public enum OutcomeType
{
    Win,
    Loss,
    NearMiss,
    EffortTask,
}

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
    public int[] slotRow = new int[3];
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
    public bool SlotFirst = true;
    [SerializeField] private SlotTrialData[] slotTrials = new SlotTrialData[16];
    [SerializeField] private ParlayTrialData[] parlayTrials = new ParlayTrialData[16];
    public GameObject SlotMachine;
    public GameObject Parlay;
    public GameObject EffortTask;
    public BetManager betManager;
    public SlotHandler slotHandler;
    public GazeHandler gazeHandler;
    public Bucket bucket;
    public List<Bucket> EffortTaskBucket;
    public AudioSource TimeIsUpAudioSource;
    private Coroutine offMusicCoroutine;
    private int offMusicToken = 0;
    [SerializeField] private float slotTrialDuration = 30f;
    [SerializeField] private float parlayTrialDuration = 105f;
    [SerializeField] private float effortTaskDuration = 45f;
    [SerializeField] private int effortTaskTrialIndex = 6;
    [SerializeField] private int lastTrialIndex = 16;
    private ParlayTrialData currentparlayTrial;
    private SlotTrialData currentSlotTrial;


[SerializeField] XRInteractorLineVisual rayLineVisual;

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
        HideRayLine();
    }

    IEnumerator switchContextAfterDelay()
    {
        yield return null;
        EffortTask.SetActive(false);
        if(SlotFirst)
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

        StartNextTrial();
    }


    void StartDataTrackers()
    {
        sxr.StartRecordingCameraPos();
        sxr.StartRecordingEyeTrackerInfo();
        SetGamblingType();
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

    public void StartNextTrial()
    {
        sxr.SetParlaySelection(",,,,,,,,,,");
        sxr.SetTotalOdds(0f);
        if (sxr.GetTrial() >= lastTrialIndex)
        {
            SwitchGamblingtype();
            return;
        }
        if(sxr.GetTrial() == effortTaskTrialIndex)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial());
            return;
        }
        offMusicToken++;
        offMusicCoroutine = StartCoroutine(PlayOffMusic(SlotMachine.activeSelf ? slotTrialDuration : parlayTrialDuration, offMusicToken));
        Debug.Log($"Starting Trial {sxr.GetTrial()}");
        if (SlotMachine.activeSelf)
        {
            currentSlotTrial = slotTrials[sxr.GetTrial()];
            StartCoroutine(RunSlotTrial(currentSlotTrial.outcome, currentSlotTrial.slotRow));
        }
        else
        {
            currentparlayTrial = parlayTrials[sxr.GetTrial()];
            StartCoroutine(RunParlayTrial(currentparlayTrial.outcome));
        }
        SetTypeOutcome();

    }

    private IEnumerator RunSlotTrial(OutcomeType outcome, int[] outcomeRow)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        slotHandler.SetOutcome(outcomeRow);
        slotHandler.StartNewTrial();


        while (!slotHandler.TrialCompleted)
        {
            yield return null;
        }

        Debug.Log($"Trial {sxr.GetTrial()} complete");

        gazeHandler.GrabPupilTrialAverage();
        CancelOffMusic();
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
        SetGamblingType();
        StartNextTrial();
    }

    private IEnumerator RunEffortTaskTrial()
    {
        bool isSlot = SlotMachine.activeSelf;
        CancelOffMusic();
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

        yield return new WaitForSeconds(0.5f);
        gazeHandler.GrabPupilTrialAverage();
        CancelOffMusic();
        betManager.StartNewTrial();
        sxr.NextTrial();
        if(sxr.GetTrial() != lastTrialIndex) betManager.ResetRound();
        StartNextTrial();
    }

    void HideRayLine()
    {
        if (rayLineVisual != null)
            rayLineVisual.enabled = false;
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

    IEnumerator PlayOffMusic(float delay, int token)
    {
        yield return new WaitForSeconds(delay);

        if (token != offMusicToken)
            yield break;

        if (EffortTask.activeSelf)
            yield break;

        TimeIsUpAudioSource.Play();
    }



    void CancelOffMusic()
    {
        offMusicToken++;

        if (offMusicCoroutine != null)
        {
            StopCoroutine(offMusicCoroutine);
            offMusicCoroutine = null;
        }

        if (TimeIsUpAudioSource.isPlaying)
            TimeIsUpAudioSource.Stop();
    }

}
