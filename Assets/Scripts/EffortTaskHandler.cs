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
    [SerializeField] private DetectBallMovement detectBallMovement;
    public bool TrialCompleted => trialCompleted;
    [SerializeField] private bool trialCompleted = false;
    [SerializeField] private TextMeshPro TutorialTextGreenBasket;
    private ManageWallet CurrentWalletScript;
    private float PrevWalletValue;
    [SerializeField] private Vector3 ballStartPosition;


    private float currentTime;

    public void StartTutorial()
    {
        StartNewTrial();
        PrevWalletValue = 0;
        CurrentWalletScript = null;
        EffortTask.SetActive(true);
        EffortTaskTutorial.SetActive(true);
        startInstructions();
    }

    private void startInstructions()
    {
        StartCoroutine(WaitforBallMovement());
    }

    private IEnumerator WaitforBallMovement()
    {
        
        yield return new WaitUntil(() => detectBallMovement.HasMoved);
        TutorialTextGreenBasket.text = "Throw the ball into\nthe green basket";
    }

    public void WaitforBasketScore()
    {
        TutorialTextGreenBasket.text = "";
    }

    private void wrongBasket()
    {
        TutorialTextGreenBasket.text = "Try again!";        
        StartCoroutine(WaitforBallMovement());
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
        foreach(Bucket bucket in EffortTaskBucket)
        {
            bucket.forceTextClear();
        }
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

    public void ResetBallPosition(GameObject ball)
    {        
        ball.transform.position = ballStartPosition;
        
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        DetectBallMovement detectMovement = ball.GetComponent<DetectBallMovement>();
        if (detectMovement != null)
        {
            detectMovement.ResetMovement();
        }

        wrongBasket();
    }
    
}
