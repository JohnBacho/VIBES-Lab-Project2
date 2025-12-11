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
        new int[] {3, 3, 1}, // Win
        new int[] {2, 4, 1}, // Lose
        new int[] {5, 3, 8}, // Lose
        new int[] {7, 7, 7},  // Win
        new int[] {1, 1, 1} // Win
    };

    private OutcomeType[] outcomes = new OutcomeType[]
    {
        OutcomeType.Win,
        OutcomeType.Lose,
        OutcomeType.Win,
        OutcomeType.Lose,
        OutcomeType.Win
    };

    void Start()
    {
        // Start with slot machine active
        SlotMachine.SetActive(true);
        Parlay.SetActive(false);
        EffortTask.SetActive(false);

        StartNextTrial();
    }

    // Call this from GameManager when a trial is done
    public void StartNextTrial()
    {
        if (sxr.GetTrial() >= outcomes.Length)
        {
            Debug.Log("All trials complete!");
            return;
        }

        StartCoroutine(RunSlotTrial(outcomes[sxr.GetTrial()], outcomeRows[sxr.GetTrial()]));
        sxr.NextTrial();
    }

    private IEnumerator RunSlotTrial(OutcomeType outcome, int[] outcomeRow)
    {
        Debug.Log($"Starting trial {sxr.GetTrial()} with outcome: {outcome}");

        GameManager.Instance.SetOutcome(outcomeRow);

        while (!GameManager.Instance.TrialCompleted)
        {
            yield return null;
        }

        Debug.Log($"Trial {sxr.GetTrial()} complete");

        yield return new WaitForSeconds(1f);

        GameManager.Instance.TrialCompleted = false;

        StartNextTrial();
    }
}
