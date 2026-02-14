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
    [SerializeField] private TextMeshPro intermisionText;

    [SerializeField] private TextMeshPro WinningsText;
    [SerializeField] private float startTime = 45f;
    [SerializeField] private List<Bucket> EffortTaskBucket;
    [SerializeField] private DetectBallMovement detectGreenBallMovement;
    [SerializeField] private DetectBallMovement detectRedBallMovement;
    [SerializeField] private DetectBallMovement detectBlueBallMovement;

    public bool TrialCompleted => trialCompleted;
    [SerializeField] private bool trialCompleted = false;
    [SerializeField] private TextMeshPro TutorialTextGreenBasket;
    [SerializeField] private TextMeshPro TutorialTextRedBasket;
    [SerializeField] private TextMeshPro TutorialTextBlueBasket;

    private ManageWallet CurrentWalletScript;
    private float PrevWalletValue;
    [SerializeField] private Vector3 ballStartPosition;
    [SerializeField] private GameObject GreenBall;
    [SerializeField] private GameObject RedBall;
    [SerializeField] private GameObject BlueBall;
    private const float IntermissionTime = 3f;

    private float currentTime;
    private enum TutorialState { WaitingForMovement, GreenBasket, RedBasket, BlueBasket, Complete }
    private TutorialState currentTutorialState = TutorialState.WaitingForMovement;

    public void StartTutorial()
    {
        StartNewTrial();
        PrevWalletValue = 0;
        CurrentWalletScript = null;
        EffortTask.SetActive(true);
        EffortTaskTutorial.SetActive(true);
        currentTutorialState = TutorialState.WaitingForMovement;
        startInstructions();
    }

    private void startInstructions()
    {
        StartCoroutine(WaitforBallMovement());
    }

    private IEnumerator WaitforBallMovement()
    {        
        switch (currentTutorialState)
        {
            case TutorialState.WaitingForMovement:
                yield return new WaitUntil(() => detectGreenBallMovement.HasMoved);
                TutorialTextGreenBasket.text = "Throw the <color=green>green</color> ball into\nthe <color=green>green</color> basket";
                currentTutorialState = TutorialState.GreenBasket;
                break;
            case TutorialState.GreenBasket:
                yield return new WaitUntil(() => detectGreenBallMovement.HasMoved);
                TutorialTextGreenBasket.text = "Throw the <color=green>green</color> ball into\nthe <color=green>green</color> basket";
                break;
            case TutorialState.RedBasket:
                yield return new WaitUntil(() => detectRedBallMovement.HasMoved);
                TutorialTextRedBasket.text = "Throw the <color=red>red</color> ball into\nthe <color=red>red</color> basket";
                break;
            case TutorialState.BlueBasket:
                yield return new WaitUntil(() => detectBlueBallMovement.HasMoved);
                TutorialTextBlueBasket.text = "Throw the <color=blue>blue</color> ball into\nthe <color=blue>blue</color> basket";
                break;
        }
    }

    public void WaitforBasketScore()
    {
        ClearAllTutorialText();
        
        switch (currentTutorialState)
        {
            case TutorialState.GreenBasket:
                currentTutorialState = TutorialState.RedBasket;
                SwapBall(GreenBall, RedBall);
                StartCoroutine(WaitforBallMovement());
                break;
            case TutorialState.RedBasket:
                currentTutorialState = TutorialState.BlueBasket;
                SwapBall(RedBall, BlueBall);
                StartCoroutine(WaitforBallMovement());
                break;
            case TutorialState.BlueBasket:
                currentTutorialState = TutorialState.Complete;
                BlueBall.SetActive(false);
                StartCountdown();
                break;
        }
    }

    private void wrongBasket()
    {
        ClearAllTutorialText();
        
        switch (currentTutorialState)
        {
            case TutorialState.GreenBasket:
                TutorialTextGreenBasket.text = "Try again!";
                break;
            case TutorialState.RedBasket:
                TutorialTextRedBasket.text = "Try again!";
                break;
            case TutorialState.BlueBasket:
                TutorialTextBlueBasket.text = "Try again!";
                break;
        }
        
        StartCoroutine(WaitforBallMovement());
    }

    private void ClearAllTutorialText()
    {
        TutorialTextGreenBasket.text = "";
        TutorialTextRedBasket.text = "";
        TutorialTextBlueBasket.text = "";
    }

    private void SwapBall(GameObject currentBall, GameObject nextBall)
    {
        if (currentBall != null)
        {
            currentBall.SetActive(false);
        }
        
        if (nextBall != null)
        {
            nextBall.SetActive(true);
            ResetBallPositionOnly(nextBall);
        }
    }

    private void ResetBallPositionOnly(GameObject ball)
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
    }
    

    private void StartCountdown()
    {
        currentTime = IntermissionTime;
        EffortTaskTutorial.SetActive(false);
        StartCoroutine(CountdownToEffort());
    }
    private IEnumerator CountdownToEffort()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            intermisionText.text = $"Task starts\nin {Mathf.CeilToInt(currentTime).ToString()}";
            yield return null;
        }

        intermisionText.text = "GO!";
        yield return new WaitForSeconds(0.8f);
        intermisionText.text = "";
        Wrapper.SetActive(true);
        ballSpawner.StartSpawning();
        currentTime = startTime;
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