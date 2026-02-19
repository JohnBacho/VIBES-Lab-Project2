using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[System.Serializable]
public class Tutorial
{
    public DetectBallMovement detectMovement;
    public GameObject Ball;

}
[System.Serializable]
public class EffortTask
{
    public int EasyThrowGoal;
    public float EasyTime;
    public int HardThrowGoal;
    public float HardTime;
}

public class EffortTaskHandler : MonoBehaviour
{
    [SerializeField] private Tutorial[] tutorialTrials = new Tutorial[1];
        [SerializeField] private EffortTask[] EffortTaskTrials = new EffortTask[1];

    [SerializeField] private GameObject EffortTask;

    [SerializeField] private GameObject Practice;
    [SerializeField] private GameObject pt1InstructionsPanel;

    [SerializeField] private GameObject Wrapper;

    [SerializeField] private TextMeshPro countdownText;
    [SerializeField] private TextMeshPro intermisionTextpt2;
    [SerializeField] private TextMeshPro WinningsText;
    [SerializeField] private GameObject GrabTutorial;

    private const float startTime = 45f;
    private const float IntermissionTime = 3f;

    [SerializeField] private Bucket EffortTaskBucket;
    [SerializeField] private Vector3 ballStartPosition;

    public bool TrialCompleted => trialCompleted;
    [SerializeField] private bool trialCompleted = false;

    private ManageWallet CurrentWalletScript;
    private float PrevWalletValue;
    private float currentTime;
    private int currentTutorialIndex = 0;
    private float PracticeTime = 10f;
    private bool PracticeComplete = false;
    private int counter = 0;
    private int throwGoal = 1;
    [SerializeField] private TextMeshPro ThrowGoalText;
    private int TrialIndexCounter = 0;
    [SerializeField] private GameObject ChoicePanel;
    [SerializeField] private GameObject Ball;
    private bool isHardMode = false;


    public void StartTutorial()
    {
        StartNewTrial();
        PrevWalletValue = 0;
        CurrentWalletScript = null;
        EffortTask.SetActive(true);
        StartCoroutine(StartInstructions());
    }

    private IEnumerator StartInstructions()
    {
        pt1InstructionsPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        pt1InstructionsPanel.SetActive(false);


        Practice.SetActive(true);

        tutorialTrials[currentTutorialIndex].Ball.SetActive(true);
        StartCoroutine(WaitForBallMovement());
    }

    private IEnumerator WaitForBallMovement()
    {
        Tutorial current = tutorialTrials[0];
        if (currentTutorialIndex == 0)
        {
            GrabTutorial.SetActive(true);
        }
        yield return new WaitUntil(() => current.detectMovement.HasMoved);
        if(currentTutorialIndex == 0)
        {
            GrabTutorial.SetActive(false);
            StartCoroutine(PracticeTimer());
        }
    }

    private IEnumerator PracticeTimer()
    {
        while (PracticeTime > 0)
        {
            PracticeTime -= 1;
            intermisionTextpt2.text = $"{Mathf.CeilToInt(PracticeTime)}";
            if (PracticeComplete)
            {
                intermisionTextpt2.text = "";
                PracticeTime = 0;
            }
            yield return new WaitForSeconds(1f);
        }

        intermisionTextpt2.text = "";

        if (!PracticeComplete)
        {
            tutorialTrials[0].Ball.SetActive(false);
            currentTutorialIndex = 0;
            PracticeTime = 10f;
            tutorialTrials[0].Ball.SetActive(true);
            ResetBallPositionOnly(tutorialTrials[0].Ball);
            intermisionTextpt2.text = "Try again";
            StartCoroutine(WaitForBallMovement());
        }
    }

    public void WaitforBasketScore()
    {

        tutorialTrials[0].Ball.SetActive(false);
        currentTutorialIndex++;

        if (currentTutorialIndex < 5)
        {
            NextBall();
        }
        else
        {
            PracticeComplete = true;
            StartCountdown();
        }
    }

    private void WrongBasket()
    {
        ResetBallPosition(tutorialTrials[0].Ball);
    }

    private void NextBall()
    {
        Tutorial current = tutorialTrials[0];
        current.Ball.SetActive(true);
        ResetBallPositionOnly(current.Ball);
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
            detectMovement.ResetMovement();
    }
    

    private void StartCountdown()
    {
        Practice.SetActive(false);
        ChoicePanel.SetActive(true);
    }

    private IEnumerator CountdownToEffort()
    {

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            intermisionTextpt2.text = $"Starting in\n{Mathf.CeilToInt(currentTime)}";
            yield return null;
        }

        intermisionTextpt2.text = "GO!";
        yield return new WaitForSeconds(0.6f);
        intermisionTextpt2.text = "";

        Wrapper.SetActive(true);
        currentTime = startTime;
        sxr.RestartTimer();
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(currentTime).ToString();
            yield return null;
        }

        countdownText.text = "0";
        StartCoroutine(EndTrial());
    }

    private IEnumerator EndTrial()
    {

        Wrapper.SetActive(false);
        WinningsText.text = $"${CurrentWalletScript.GetWallet() - PrevWalletValue:0.00}\nAdded to\nwallet";
        sxr.CalculateEffortScore(sxr.GetBallsThrown());

        yield return new WaitForSeconds(5f);

        WinningsText.text = "";
        Practice.SetActive(false);
        MarkTrialComplete();
    }

    public void setWallet(bool isSlot, SlotHandler slotHandler, ParlayHandler parlayHandler)
    {
        ManageWallet wallet = isSlot ? (ManageWallet)slotHandler : parlayHandler;
        CurrentWalletScript = wallet;
        PrevWalletValue = wallet.GetWallet();
    }

    private void addToWallet(float Money)
    {
        if (CurrentWalletScript != null)
        {
            CurrentWalletScript.AddWallet(Money);
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
            detectMovement.WrongBucketResetMovement();
    }

    public void AddScore()
    {
        counter++;
        ThrowGoalText.text = $"Throw Goal: {throwGoal- counter}";
        ResetBallPositionOnly(Ball);
        Ball.SetActive(true);
        if(counter >= throwGoal)
        {
            EndTrial();
            addToWallet(10f);
        }
        
    }

    public void Hard()
    {
        isHardMode = true;
        if (TrialIndexCounter < EffortTaskTrials.Length)
        {
            throwGoal = EffortTaskTrials[TrialIndexCounter].HardThrowGoal;
            currentTime = EffortTaskTrials[TrialIndexCounter].HardTime;
        }
        ChoicePanel.SetActive(false);
        Wrapper.SetActive(true);
        ThrowGoalText.text = $"Throw Goal: {throwGoal- counter}";
        sxr.RestartTimer();
        StartCoroutine(TimerRoutine());
    }

    public void Easy()
    {
        isHardMode = false;
        if (TrialIndexCounter < EffortTaskTrials.Length)
        {
            throwGoal = EffortTaskTrials[TrialIndexCounter].EasyThrowGoal;
            currentTime = EffortTaskTrials[TrialIndexCounter].EasyTime;
        }
        ChoicePanel.SetActive(false);
        Wrapper.SetActive(true);
        ThrowGoalText.text = $"Throw Goal: {throwGoal- counter}";
        sxr.RestartTimer();
        StartCoroutine(TimerRoutine());
    }

}