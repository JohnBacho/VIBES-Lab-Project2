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
    public GameObject SlotMachine; // used for switching GamblingType
    public GameObject Parlay; // used for switching GamblingType
    public GameObject EffortTask;
    public BetManager betManager;
    public SlotHandler slotHandler;
    private string headers = "Parlay1, Parlay2, Parlay3, Parlay4, Parlay5";
    private int counter = 0;
[SerializeField] XRInteractorLineVisual rayLineVisual;
    private int[][] SlotOutcomeRows = new int[][]
    {
        new int[] {3, 3, 1}, // NearMiss
        // new int[] {5, 5, 5}, // Win
        // new int[] {5, 3, 8}, // Lose
        // new int[] {7, 7, 7},  // Win
        // new int[] {2, 5, 9}, // Lose
        // new int[] {5, 7, 6}, // Lose
        // new int[] {0, 0, 0}, // Dummy for Effort Task
        // new int[] {6, 7, 6}, // Lose
        // new int[] {2, 7, 5}, // Lose
        // new int[] {1, 1, 1}, // Win
        // new int[] {4, 4, 4},  // Win
        // new int[] {4, 5, 4}, // NearMiss
        // new int[] {0, 8, 3}, // Lose
        // new int[] {6, 6, 6},  // Win
        // new int[] {6, 3, 1}, // Lose
        // new int[] {0,1,1} // NearMiss
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
        new int[] {0, 0, 0} // NearMiss
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


    private OutcomeType[] outcomes = new OutcomeType[]
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
        SlotMachine.SetActive(true);
        Parlay.SetActive(false);
        EffortTask.SetActive(false);
        StartDataTrackers();
        StartNextTrial();
    }

    void StartDataTrackers()
    {
        sxr.StartRecordingCameraPos();
        sxr.StartRecordingEyeTrackerInfo();
        sxr.WriteHeaderToTaggedFile("SummaryGambling", headers);
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
        if (sxr.GetTrial() >= SlotOutcomeRows.Length)
        {
            Debug.Log("Slot trials complete! Starting Parlay trials...");
            SwitchGamblingtype();
            return;
        }
        HideRayLine();
        SetTypeOutcome();
        if(sxr.GetTrial() == 6)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial(true));
            return;
        }

        Debug.Log($"Starting Slot Trial {sxr.GetTrial()}");
        StartCoroutine(RunSlotTrial(outcomes[sxr.GetTrial()], SlotOutcomeRows[sxr.GetTrial()]));
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

        yield return new WaitForSeconds(0.5f);

        slotHandler.StartNewTrial();
        sxr.NextTrial();
        StartNextTrial();
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
            HideRayLine();
            StartNextTrial();
        }
        else
        {
            ShowRayLine();
            StartNextParlayTrial();   
        }
    }

    private IEnumerator RunEffortTaskTrial(bool isSlot)
    {
        sxr.NextTrial();
        HideRayLine();
        Debug.Log("Starting Effort Task Trial");
        SlotMachine.SetActive(false);
        Parlay.SetActive(false);
        EffortTask.SetActive(true);
        FindObjectOfType<BallSpawner>().StartSpawning();
        FindObjectOfType<CountdownTimer>().StartCountdown();
        yield return new WaitForSeconds(45f);
        FindObjectOfType<BallSpawner>().StopSpawning();
        FindObjectOfType<BallSpawner>().DestroyAllBalls();

        EffortTask.SetActive(false);
        if(isSlot)
        {
            SlotMachine.SetActive(true);
            StartNextTrial();
            SetGamblingType();            
        }
        else
        {
            Parlay.SetActive(true);
            ShowRayLine();
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
            Debug.Log("Parlay trials complete!");
            return;
        }
        SetTypeOutcome();
        if(sxr.GetTrial() == 6)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial(false));
            return;
        }
        Debug.Log($"Starting Parlay Trial {sxr.GetTrial()}");
        StartCoroutine(RunParlayTrial(outcomes[sxr.GetTrial()]));

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

        betManager.StartNewTrial();
        
        sxr.NextTrial();
        StartNextParlayTrial();
    }

    void HideRayLine()
    {
        if (rayLineVisual != null)
            rayLineVisual.enabled = false;
    }

    void ShowRayLine()
    {
        if (rayLineVisual != null)
            rayLineVisual.enabled = true;
    }

    void SetTypeOutcome()
    {
        sxr.SetOutcome(outcomes[counter].ToString());
        counter++;
    }

}
