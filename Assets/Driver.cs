using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;


public enum OutcomeType
{
    Win,
    Lose,
    NearMiss,
    EffortTask,
}

public enum GamblingType
{
    Slot,
    Parlay,
    EffortTask
}

public class Driver : MonoBehaviour
{
    public bool SlotFirst = true;
    public GameObject SlotMachine; // used for switching GamblingType
    public GameObject Parlay; // used for switching GamblingType
    public GameObject EffortTask;
    public BetManager betManager;
    public SlotHandler slotHandler;
    public AudioSource TimeIsUpAudioSource;
    private Coroutine offMusicCoroutine;
    private float SlotTrialTime = 30f;
    private float ParlayTrialTime = 105f;
    private int offMusicToken = 0;


[SerializeField] XRInteractorLineVisual rayLineVisual;
    private int[][] SlotOutcomeRows = new int[][]
    {
        new int[] {3, 3, 1}, // NearMiss
        new int[] {5, 5, 5}, // Win
        new int[] {5, 3, 8}, // Lose
        new int[] {7, 7, 7},  // Win
        new int[] {2, 5, 9}, // Lose
        new int[] {5, 7, 6}, // Lose
        new int[] {0, 0, 0}, // Dummy for Effort Task
        new int[] {6, 7, 6}, // Lose
        new int[] {2, 7, 5}, // Lose
        new int[] {1, 1, 1}, // Win
        new int[] {4, 4, 4},  // Win
        new int[] {4, 5, 4}, // NearMiss
        new int[] {0, 8, 3}, // Lose
        new int[] {6, 6, 6},  // Win
        new int[] {6, 3, 1}, // Lose
        new int[] {0,1,1} // NearMiss
    };
    private int[][] Parlay3Leg = new int[][]
    {
        new int[] {1, 1, 0}, // NearMiss
        new int[] {1, 1, 1}, // Win
        new int[] {0, 0, 1}, // Lose
        new int[] {1, 1, 1}, // Win
        new int[] {0, 1, 0}, // Lose
        new int[] {1, 0, 0}, // Lose
        new int[] {0, 0, 0}, // Dummy for Effort Task
        new int[] {0, 0, 0}, // Lose
        new int[] {0, 0, 1}, // Lose
        new int[] {1, 1, 1}, // Win
        new int[] {1, 1, 1}, // Win
        new int[] {1, 0, 1}, // NearMiss
        new int[] {0, 0, 1}, // Lose
        new int[] {1, 1, 1}, // Win
        new int[] {0, 0, 0}, // Lose
        new int[] {0, 1, 1} // NearMiss
    };

    private int[][] Parlay4Leg = new int[][]
    {
        new int[] {1, 1, 1, 0}, // NearMiss
        new int[] {1, 1, 1, 1}, // Win
        new int[] {0, 0, 1, 1}, // Lose
        new int[] {1, 1, 1, 1}, // Win
        new int[] {0, 1, 1, 0}, // Lose
        new int[] {1, 0, 1, 0}, // Lose
        new int[] {0, 0, 0, 0}, // Dummy for Effort Task
        new int[] {0, 0, 1, 0}, // Lose
        new int[] {0, 0, 1, 1}, // Lose
        new int[] {1, 1, 1, 1}, // Win
        new int[] {1, 1, 1, 1}, // Win
        new int[] {1, 0, 1, 1}, // NearMiss
        new int[] {0, 0, 1, 1}, // Lose
        new int[] {1, 1, 1, 1}, // Win
        new int[] {0, 1, 0, 0}, // Lose
        new int[] {0, 1, 1, 1} // NearMiss

    };

    private int[][] Parlay5Leg = new int[][]
    {
        new int[] {1, 1, 1, 1, 0}, // NearMiss
        new int[] {1, 1, 1, 1, 1}, // Win
        new int[] {0, 0, 1, 1, 0}, // Lose
        new int[] {1, 1, 1, 1, 1}, // Win
        new int[] {0, 1, 1, 0, 0}, // Lose
        new int[] {0, 0, 1, 1, 0}, // Lose
        new int[] {0, 0, 0, 0, 0}, // Dummy for Effort Task
        new int[] {0, 0, 0, 0, 1}, // Lose
        new int[] {0, 0, 0, 1, 1}, // Lose
        new int[] {1, 1, 1, 1, 1}, // Win
        new int[] {1, 1, 1, 1, 1}, // Win
        new int[] {1, 0, 1, 1, 1}, // NearMiss
        new int[] {0, 0, 0, 1, 1}, // Lose
        new int[] {1, 1, 1, 1, 1}, // Win
        new int[] {0, 1, 0, 1, 0}, // Lose
        new int[] {0, 1, 1, 1, 1} // NearMiss

    };


    private OutcomeType[] SlotOutcomes = new OutcomeType[]
    {
        OutcomeType.NearMiss, // {3, 3, 1}
        OutcomeType.Win,      // {5, 5, 5}
        OutcomeType.Lose,     // {5, 3, 8}
        OutcomeType.Win,      // {7, 7, 7}
        OutcomeType.Lose,     // {2, 5, 9}
        OutcomeType.Lose,     // {5, 7, 6}
        OutcomeType.EffortTask,     // {0, 0, 0} Dummy for Effort Task
        OutcomeType.Lose,     // {6, 7, 6}
        OutcomeType.Lose,     // {2, 7, 5}
        OutcomeType.Win,      // {1, 1, 1}
        OutcomeType.Win,      // {4, 4, 4}
        OutcomeType.NearMiss, // {4, 5, 4}
        OutcomeType.Lose,     // {0, 8, 3}
        OutcomeType.Win,      // {6, 6, 6}
        OutcomeType.Lose,     // {6, 3, 1}
        OutcomeType.NearMiss,  // {0, 1, 1}
    };

        private OutcomeType[] ParlayOutcomes = new OutcomeType[]
    {
        OutcomeType.NearMiss, // {3, 3, 1} // Start of Parlay
        OutcomeType.Win,      // {5, 5, 5}
        OutcomeType.Lose,     // {5, 3, 8}
        OutcomeType.Win,      // {7, 7, 7}
        OutcomeType.Lose,     // {2, 5, 9}
        OutcomeType.Lose,     // {5, 7, 6}
        OutcomeType.EffortTask,     // {0, 0, 0} Dummy for Effort Task
        OutcomeType.Lose,     // {6, 7, 6}
        OutcomeType.Lose,     // {2, 7, 5}
        OutcomeType.Win,      // {1, 1, 1}
        OutcomeType.Win,      // {4, 4, 4}
        OutcomeType.NearMiss, // {4, 5, 4}
        OutcomeType.Lose,     // {0, 8, 3}
        OutcomeType.Win,      // {6, 6, 6}
        OutcomeType.Lose,     // {6, 3, 1}
        OutcomeType.NearMiss  // {0, 1, 1}
    };


    void Start()
    {
        StartCoroutine(switchContextAfterDelay());
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        StartDataTrackers();
        StartNextSlotTrial();
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
    
            StartNextSlotTrial();
        }
        else
        {
            SlotMachine.SetActive(false);
            Parlay.SetActive(true);
            
            StartNextParlayTrial();
        }
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

    public void StartNextSlotTrial()
    {
        sxr.SetParlaySelection(",,,,,,,,,,");
        if (sxr.GetTrial() >= SlotOutcomeRows.Length)
        {
            Debug.Log("Slot trials complete! Starting Parlay trials...");
            SwitchGamblingtype();
            return;
        }
        SetTypeOutcome();
        if(sxr.GetTrial() == 6)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial(true));
            return;
        }
        offMusicToken++;
        offMusicCoroutine = StartCoroutine(PlayOffMusic(SlotMachine.activeSelf ? SlotTrialTime : ParlayTrialTime, offMusicToken));
        Debug.Log($"Starting Slot Trial {sxr.GetTrial()}");
        StartCoroutine(RunSlotTrial(SlotOutcomes[sxr.GetTrial()], SlotOutcomeRows[sxr.GetTrial()]));
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

        CancelOffMusic();
        slotHandler.StartNewTrial();
        sxr.NextTrial();
        slotHandler.rest();
        StartNextSlotTrial();
    }

    private void SwitchGamblingtype()
    {
        sxr.NextPhase();
        SlotMachine.SetActive(!SlotMachine.activeSelf);
        Parlay.SetActive(!Parlay.activeSelf);
        GameManager.Instance.SetWallet(100f);
        SetGamblingType();
        if (SlotMachine.activeSelf)
        {
    
            StartNextSlotTrial();
        }
        else
        {
            
            StartNextParlayTrial();   
        }
    }

    private IEnumerator RunEffortTaskTrial(bool isSlot)
    {
        CancelOffMusic();
        Debug.Log("Starting Effort Task Trial");
        sxr.SetGamblingType(GamblingType.EffortTask.ToString());
        SlotMachine.SetActive(false);
        Parlay.SetActive(false);
        EffortTask.SetActive(true);
        Object.FindAnyObjectByType<BallSpawner>().StartSpawning();
        Object.FindAnyObjectByType<CountdownTimer>().StartCountdown();
        yield return new WaitForSeconds(45f);
        Object.FindAnyObjectByType<BallSpawner>().StopSpawning();
        Object.FindAnyObjectByType<BallSpawner>().DestroyAllBalls();
        sxr.NextTrial();

        EffortTask.SetActive(false);
        if(isSlot)
        {
            SlotMachine.SetActive(true);
            StartNextSlotTrial();
            SetGamblingType();            
        }
        else
        {
            Parlay.SetActive(true);
            
            StartNextParlayTrial();
            SetGamblingType();
        }

    }

    public void ParlayOutcome(int legCount)
    {
        int index = sxr.GetTrial();
        List<int> Outcome = null;

        switch (legCount)
        {
            case 3:
                Outcome = new List<int>(Parlay3Leg[index]);
                break;

            case 4:
                Outcome = new List<int>(Parlay4Leg[index]);
                break;

            case 5:
                Outcome = new List<int>(Parlay5Leg[index]);
                break;

            default:
                Debug.LogError("Invalid parlay size");
                return;
        }

        betManager.SetOutcome(Outcome);
    }


    public void StartNextParlayTrial()
    {
        if (sxr.GetTrial() >= Parlay4Leg.Length)
        {
            SwitchGamblingtype();
            return;
        }
        SetTypeOutcome();
        if(sxr.GetTrial() == 6)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial(false));
            return;
        }
        offMusicToken++;
        offMusicCoroutine = StartCoroutine(PlayOffMusic(SlotMachine.activeSelf ? SlotTrialTime : ParlayTrialTime, offMusicToken));
        Debug.Log($"Starting Parlay Trial {sxr.GetTrial()}");
        StartCoroutine(RunParlayTrial(ParlayOutcomes[sxr.GetTrial()]));

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

        CancelOffMusic();
        betManager.StartNewTrial();
        sxr.NextTrial();
        if(sxr.GetTrial() != 16) betManager.ResetRound();
        StartNextParlayTrial();
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
            sxr.SetOutcome(ParlayOutcomes[sxr.GetTrial()].ToString());            
        }
        else if(SlotMachine.activeSelf)
        {
            sxr.SetOutcome(SlotOutcomes[sxr.GetTrial()].ToString());
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
