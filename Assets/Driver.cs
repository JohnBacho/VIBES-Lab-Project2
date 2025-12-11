using System.Collections;
using UnityEngine;

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

    private int[][] outcomeRows = new int[][]
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
        SlotMachine.SetActive(true);
        Parlay.SetActive(false);
        EffortTask.SetActive(false);

        StartNextTrial();
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


        StartCoroutine(RunSlotTrial(outcomes[sxr.GetTrial()], outcomeRows[sxr.GetTrial()]));
        sxr.NextTrial();
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
}
