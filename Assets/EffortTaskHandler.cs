using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffortTaskHandler : MonoBehaviour
{
    [SerializeField] private GameObject EffortTask;
    [SerializeField] private BallSpawner ballSpawner;

    [SerializeField] private GameObject EffortTaskTutorial;
    [SerializeField] private GameObject Wrapper;

    [SerializeField] private TextMeshPro countdownText;
    [SerializeField] private TextMeshPro WinningsText;
    [SerializeField] private float startTime = 45f;
    [SerializeField] private List<Bucket> EffortTaskBucket;
    public bool TrialCompleted => trialCompleted;
    [SerializeField] private bool trialCompleted = false;
    private ManageWallet CurrentWalletScript;
    private float PrevWalletValue;


    private float currentTime;

    public void StartTutorial()
    {
        StartNewTrial();
        PrevWalletValue = 0;
        CurrentWalletScript = null;
        EffortTask.SetActive(true);
        EffortTaskTutorial.SetActive(true);
        StartCoroutine(InstructionSteps());
    }

    private IEnumerator InstructionSteps()
    {
        yield return null;
        yield return new WaitUntil(() => sxr.GetTrigger());
        EffortTaskTutorial.SetActive(false);  
        yield return null;
        StartCountdown();
    }

    private void StartCountdown()
    {
        currentTime = startTime;
        Wrapper.SetActive(true);
        ballSpawner.StartSpawning();
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            countdownText.text = Mathf.CeilToInt(currentTime).ToString();

            yield return null;
        }

        countdownText.text = "0";
        StartCoroutine(endTrial());
    }

    private IEnumerator endTrial()
    {
        ballSpawner.StopSpawning();
        ballSpawner.DestroyAllBalls();
        Wrapper.SetActive(false);
        WinningsText.text = $"${CurrentWalletScript.GetWallet() - PrevWalletValue:0.00}\nAdded to\nwallet";
        yield return new WaitForSeconds(5f);
        WinningsText.text = "";
        EffortTask.SetActive(false);
        EffortTaskTutorial.SetActive(false);
        MarkTrialComplete();
    }

    public void setBuckets(bool isSlot, SlotHandler slotHandler, ParlayHandler parlayHandler)
    {
        if(isSlot)
        {
            for(int i = 0; i < EffortTaskBucket.Count; i++)
            {
                EffortTaskBucket[i].SetWallet(slotHandler);
            }
            CurrentWalletScript = slotHandler;
            PrevWalletValue = CurrentWalletScript.GetWallet();
        }
        else
        {
            for(int i = 0; i < EffortTaskBucket.Count; i++)
            {
                EffortTaskBucket[i].SetWallet(parlayHandler);
            }
            CurrentWalletScript = parlayHandler;
            PrevWalletValue = CurrentWalletScript.GetWallet();
        }
    }

    private void MarkTrialComplete()
    {
        trialCompleted = true;
    }

    private void StartNewTrial()
    {
        trialCompleted = false;
    }

    public void SetActiveEffortTask(bool isActive)
    {
        EffortTask.SetActive(isActive);
    }

    public bool GetActive()
    {
        return EffortTask.activeSelf;
    }
    
}
