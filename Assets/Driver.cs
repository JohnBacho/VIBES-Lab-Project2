using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum OutcomeType
{
    Win,
    Lose,
    NearMiss,
}

public class Driver : MonoBehaviour
{
    public GameObject SlotMachine; // used for switching GamblingType
    public GameObject Parlay; // used for switching GamblingType
    public GameObject EffortTask;
    public BetManager betManager;
    public SlotHandler slotHandler;

    private int[][] SlotOutcomeRows = new int[][]
    {
        new int[] {3, 3, 1}, // NearMiss
        new int[] {5, 5, 5}, // Win
        new int[] {5, 3, 8}, // Lose
        new int[] {7, 7, 7},  // Win
        new int[] {2, 5, 9}, // Lose
        new int[] {5, 7, 6}, // Lose
        new int[] {0, 0, 0}, // Dummy for Effort Task
        new int[] {6, 7, 6}

    };
    private int[][] Parlay3Leg = new int[][]
    {
        new int[] {1, 1, 1}, // Win
        new int[] {1, 1, 0}, // NearMiss
        new int[] {0, 0, 1}, // Lose
    };

    private int[][] Parlay4Leg = new int[][]
    {
        new int[] {1, 1, 1, 1}, // Win
        new int[] {1, 1, 1, 0}, // NearMiss
        new int[] {0, 1, 0, 0}, // Lose
    };

    private int[][] Parlay5Leg = new int[][]
    {
        new int[] {1, 1, 1, 1, 1}, // Win
        new int[] {1, 1, 1, 1, 0}, // NearMiss
        new int[] {1, 0, 1, 0, 0}, // Lose
    };


    private OutcomeType[] outcomes = new OutcomeType[]
    {
        OutcomeType.NearMiss,
        OutcomeType.Lose,
        OutcomeType.Win,
        OutcomeType.Lose,
        OutcomeType.Win,
        OutcomeType.NearMiss,
        OutcomeType.Lose,
        OutcomeType.Win,
        OutcomeType.Lose,
        OutcomeType.Win
    };

    void Start()
    {
        SlotMachine.SetActive(false);
        Parlay.SetActive(true);
        EffortTask.SetActive(false);
        StartNextParlayTrial();
        // StartNextTrial();
    }

    public void StartNextTrial()
    {
        if (sxr.GetTrial() >= outcomes.Length)
        {
            Debug.Log("Slot trials complete!");
            return;
        }

        if(sxr.GetTrial() == 6)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial());
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

    private IEnumerator RunEffortTaskTrial()
    {
        sxr.NextTrial();
        Debug.Log("Starting Effort Task Trial");
        SlotMachine.SetActive(false);
        Parlay.SetActive(false);
        EffortTask.SetActive(true);
        FindObjectOfType<BallSpawner>().StartSpawning();
        FindObjectOfType<CountdownTimer>().StartCountdown();
        yield return new WaitForSeconds(60f);
        Debug.Log("Effort Task Trial complete");
        FindObjectOfType<BallSpawner>().StopSpawning();
        FindObjectOfType<BallSpawner>().DestroyAllBalls();

        EffortTask.SetActive(false);
        SlotMachine.SetActive(true);
        StartNextTrial();
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

        if(sxr.GetTrial() == 6)
        {
            Debug.Log("Running Effort Task Trial");
            StartCoroutine(RunEffortTaskTrial());
            return;
        }
        Debug.Log($"Starting Parlay Trial {sxr.GetTrial()}");
        StartCoroutine(RunParlayTrial(outcomes[sxr.GetTrial()]));

    }

    private IEnumerator RunParlayTrial(OutcomeType outcome)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        betManager.StartNewTrial();


        while (!betManager.TrialCompleted)
        {
            yield return null;
        }

        Debug.Log($"Trial {sxr.GetTrial()} complete");

        yield return new WaitForSeconds(0.5f);

        betManager.StartNewTrial();
        
        sxr.NextTrial();
        StartNextTrial();
    }


}
