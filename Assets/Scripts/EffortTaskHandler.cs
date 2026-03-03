using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using sxr_internal;

public class EffortTaskHandler : MonoBehaviour
{
    [SerializeField] private GameObject effortTaskObject;

    [SerializeField] private GameObject practice;
    [SerializeField] private GameObject pt1InstructionsPanel;

    [SerializeField] private GameObject wrapper;

    [SerializeField] private TextMeshPro countdownText;
    [SerializeField] private TextMeshPro intermissionText;
    [SerializeField] private TextMeshPro winningsText;
    [SerializeField] private GameObject pokeButtonTutorial;
    [SerializeField] private GameObject practiceTutorialBlock;
    [SerializeField] private GazeHandler gazeHandler;
    [SerializeField] private PokeButton PracticeButton;

    [SerializeField] private PokeButton EasyButton;
    [SerializeField] private PokeButton HardButton;


    private const float IntermissionTime = 3f;

    public bool TrialCompleted => trialCompleted;
    [SerializeField] private bool trialCompleted = false;

    private ManageWallet currentWalletScript;
    private float prevWalletValue;
    private float currentTime;
    private float practiceTime = 10f;
    private bool practiceComplete = false;
    private int counter = 0;
    private int PracticeCounter =0;
    private int goal = 1;
    private const int practiceGoal = 10;

    [SerializeField] private TextMeshPro goalText;
    private int trialIndexCounter = 0;
    [SerializeField] private GameObject choicePanel;
    private bool isHardMode = false;
    [SerializeField] private TextMeshPro hardChoiceText;
    [SerializeField] private TextMeshPro easyChoiceText;

    private static readonly float buttonVolume = 0.5f;
    private static readonly float increasePitch = 3f;
    private bool trialEnded = false;


    private const int EasyGoal = 20;
    private const float EasyTime = 7f;
    private  const float EasyReward = 1f;
    private const int HardGoal = 90;
    private const float HardTime = 21f;
    private const float HardReward = 3f;


    public void StartTutorial()
    {
        StartNewTrial();
        prevWalletValue = 0;
        currentWalletScript = null;
        effortTaskObject.SetActive(true);
        StartCoroutine(StartInstructions());            
    }

    private IEnumerator StartInstructions()
    {
        if (!practiceComplete)
        {
            pt1InstructionsPanel.SetActive(true);
            yield return new WaitForSeconds(3f);
            yield return new WaitUntil(() => sxr.GetTrigger());
            pt1InstructionsPanel.SetActive(false);
            practice.SetActive(true);
            DisableButtons(true);            
        }
        else
        {
            StartCountdown();
        }

    }

    private IEnumerator PracticeTimer()
    {
        while (practiceTime > 0)
        {
            practiceTime -= 1;
            intermissionText.text = $"Time: {Mathf.CeilToInt(practiceTime)}";
            if (practiceComplete)
            {
                intermissionText.text = "";
                goalText.text = "";
                practiceTime = 0;
            }
            yield return new WaitForSeconds(1f);
        }

        intermissionText.text = "";

        if (!practiceComplete)
        {
            practiceTime = 10f;
            intermissionText.text = "Try again";
            practiceTutorialBlock.SetActive(true);
            pokeButtonTutorial.SetActive(true);
            PracticeCounter = 0;
            goalText.text = $"Button Goal:\n{practiceGoal - PracticeCounter}";
        }
    }

    private void StartCountdown()
    {
        practice.SetActive(false);
        choicePanel.SetActive(true);
        DisableButtons(false);
        SetChoiceText();
    }

    private IEnumerator CountdownToEffort()
    {
        float countdown = 3f;
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            intermissionText.text = $"Get Ready\n{Mathf.CeilToInt(countdown)}";
            yield return null;
        }

        intermissionText.text = "GO!";
        yield return new WaitForSeconds(0.6f);
        intermissionText.text = "";

        wrapper.SetActive(true);
        gazeHandler.SetCaptureEventBaseline();
        sxr.RestartTimer();
        goalText.text = $"Button press Goal:\n{goal - counter}";
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(currentTime).ToString();
            if(trialEnded || counter == goal)
            {
                break;
            }
            yield return null;
        }

        countdownText.text = "0";
        if (!trialEnded)
        {
            trialEnded = true;
            goalText.text = "";
            StartCoroutine(EndTrial());
        }
    }

    private IEnumerator EndTrial()
    {
        wrapper.SetActive(false);
        winningsText.text = $"${currentWalletScript.GetWallet() - prevWalletValue:0.00}\nAdded to\nwallet";

        yield return new WaitForSeconds(5f);

        winningsText.text = "";
        MarkTrialComplete();
        yield return null;
        ResetTrialState();
    }

    private void ResetTrialState()
    {
        counter = 0;
        PracticeCounter = 0;
        goal = 1;
        currentTime = 0f;
        isHardMode = false;
        practiceTime = 10f;
        goalText.text = "";
        countdownText.text = "";
        intermissionText.text = "";
        trialEnded = false;
        trialIndexCounter++;
    }

    public void setWallet(bool isSlot, SlotHandler slotHandler, ParlayHandler parlayHandler)
    {
        ManageWallet wallet = isSlot ? (ManageWallet)slotHandler : parlayHandler;
        currentWalletScript = wallet;
        prevWalletValue = wallet.GetWallet();
    }

    private void AddToWallet(float money)
    {
        if (currentWalletScript != null)
        {
            currentWalletScript.AddWallet(money);
            sxr.SetPayout(money);
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
        effortTaskObject.SetActive(isActive);
    }

    public bool GetActive()
    {
        return effortTaskObject.activeSelf;
    }

public void AddScore()
{
    SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
    counter++;
    sxr.SetButtonPresses(counter);
    goalText.text = $"Button press Goal:\n{goal - counter}";

    if (counter == goal && !trialEnded)
    {
        trialEnded = true;
        AddToWallet(isHardMode ? HardReward : EasyReward);
        goalText.text = "";
        StartCoroutine(EndTrial());
    }
}

    private void SetChoiceText()
    {

    easyChoiceText.text = $"<b>EASY TASK</b>\n<size=120%><color=green>${EasyReward}</color></size>\n<size=80%>{EasyGoal} presses • 7s</size>";

    hardChoiceText.text = $"<b>HARD TASK</b>\n<size=120%><color=green>${HardReward}</color></size>\n<size=80%>{HardGoal} presses • 21s</size>";

    }

    public void Hard()
    {
        SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
        isHardMode = true;
        sxr.SetHardEffortTask(isHardMode);
        goal = HardGoal;
        currentTime = HardTime;
        choicePanel.SetActive(false);
        StartCoroutine(CountdownToEffort());
    }

    public void Easy()
    {
        SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
        isHardMode = false;
        sxr.SetHardEffortTask(isHardMode);
        goal = EasyGoal;
        currentTime = EasyTime;
        choicePanel.SetActive(false);
        StartCoroutine(CountdownToEffort());
    }

    public void AddPracticeScore()
    {
        if (PracticeCounter == 0)
        {
            practiceTime = 10f;
            practiceTutorialBlock.SetActive(false);
            pokeButtonTutorial.SetActive(false);
            StartCoroutine(PracticeTimer());
        }

        SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
        PracticeCounter++;
        goalText.text = $"Button Goal:\n{practiceGoal - PracticeCounter}";

        if (PracticeCounter == practiceGoal)
        {
            Debug.Log("Practice Goal Reached");
            goalText.text = "";
            practiceComplete = true;
            StartCountdown();
        }
    }

    private void DisableButtons(bool practice)
    {
        StartCoroutine(TemporarilyDisableButtons(practice));
    }
    private IEnumerator TemporarilyDisableButtons(bool practice)
    {
        if (practice)
        {
            PracticeButton.DisableButton();
        }
        EasyButton.DisableButton();
        HardButton.DisableButton();
        yield return new WaitForSeconds(1.5f);
        if (practice)
        {
            PracticeButton.EnableButton();
        }
        EasyButton.EnableButton();
        HardButton.EnableButton();
    }
}