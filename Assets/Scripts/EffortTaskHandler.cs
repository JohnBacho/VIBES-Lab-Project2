using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class EffortTask
{
    public int EasyGoal;
    public float EasyTime;
    public float EasyReward;
    public int HardGoal;
    public float HardTime;
    public float HardReward;
}

public class EffortTaskHandler : MonoBehaviour
{
    [SerializeField] private EffortTask[] effortTaskTrials = new EffortTask[1];

    [SerializeField] private GameObject effortTaskObject;

    [SerializeField] private GameObject practice;
    [SerializeField] private GameObject pt1InstructionsPanel;
    [SerializeField] private GameObject pt2InstructionsPanel;

    [SerializeField] private GameObject wrapper;

    [SerializeField] private TextMeshPro countdownText;
    [SerializeField] private TextMeshPro intermissionText;
    [SerializeField] private TextMeshPro winningsText;
    [SerializeField] private GameObject pokeButtonTutorial;
    [SerializeField] private GameObject practiceTutorialBlock;

    private const float IntermissionTime = 3f;

    public bool TrialCompleted => trialCompleted;
    [SerializeField] private bool trialCompleted = false;

    private ManageWallet currentWalletScript;
    private float prevWalletValue;
    private float currentTime;
    private float practiceTime = 10f;
    private bool practiceComplete = false;
    private int counter = 0;
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
        pt1InstructionsPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        pt1InstructionsPanel.SetActive(false);

        practice.SetActive(true);
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
            counter = 0;
            goalText.text = $"Button Goal: {practiceGoal - counter}";
        }
    }

    private void StartCountdown()
    {
        practice.SetActive(false);
        choicePanel.SetActive(true);
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
        sxr.RestartTimer();
        goalText.text = $"Button press Goal: {goal - counter}";
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
        wrapper.SetActive(false);
        winningsText.text = $"${currentWalletScript.GetWallet() - prevWalletValue:0.00}\nAdded to\nwallet";
        sxr.CalculateEffortScore(sxr.GetBallsThrown());

        yield return new WaitForSeconds(5f);

        winningsText.text = "";
        ResetTrialState();
        MarkTrialComplete();
    }

    private void ResetTrialState()
    {
        counter = 0;
        goal = 1;
        currentTime = 0f;
        isHardMode = false;
        practiceComplete = false;
        practiceTime = 10f;
        goalText.text = "";
        countdownText.text = "";
        intermissionText.text = "";
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
        goalText.text = $"Button press Goal: {goal - counter}";

        if (counter == goal)
        {
            Debug.Log("Goal Reached");
            if (isHardMode)
                AddToWallet(effortTaskTrials[trialIndexCounter].HardReward);
            else
                AddToWallet(effortTaskTrials[trialIndexCounter].EasyReward);

            goalText.text = "";
            StartCoroutine(EndTrial());
        }
    }

    private void SetChoiceText()
    {
        if (trialIndexCounter < effortTaskTrials.Length)
        {
            int easyGoal = effortTaskTrials[trialIndexCounter].EasyGoal;
            float easyReward = effortTaskTrials[trialIndexCounter].EasyReward;
            easyChoiceText.text = $"<b>EASY TASK</b>\n<size=120%><color=green>${easyReward}</color></size>\n<size=80%>{easyGoal} presses • 7s</size>";

            int hardGoal = effortTaskTrials[trialIndexCounter].HardGoal;
            float hardReward = effortTaskTrials[trialIndexCounter].HardReward;
            hardChoiceText.text = $"<b>HARD TASK</b>\n<size=120%><color=green>${hardReward}</color></size>\n<size=80%>{hardGoal} presses • 21s</size>";
        }
    }

    public void Hard()
    {
        SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
        isHardMode = true;
        if (trialIndexCounter < effortTaskTrials.Length)
        {
            goal = effortTaskTrials[trialIndexCounter].HardGoal;
            currentTime = effortTaskTrials[trialIndexCounter].HardTime;
        }
        choicePanel.SetActive(false);
        StartCoroutine(CountdownToEffort());
    }

    public void Easy()
    {
        SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
        isHardMode = false;
        if (trialIndexCounter < effortTaskTrials.Length)
        {
            goal = effortTaskTrials[trialIndexCounter].EasyGoal;
            currentTime = effortTaskTrials[trialIndexCounter].EasyTime;
        }
        choicePanel.SetActive(false);
        StartCoroutine(CountdownToEffort());
    }

    public void AddPracticeScore()
    {
        if (counter == 0)
        {
            practiceTime = 10f;
            practiceTutorialBlock.SetActive(false);
            pokeButtonTutorial.SetActive(false);
            StartCoroutine(PracticeTimer());
        }

        SoundManager.SoundManager.PlaySound3D(SoundType.increaseButtonSound, practiceTutorialBlock.transform.position, buttonVolume, increasePitch);
        counter++;
        goalText.text = $"Button Goal: {practiceGoal - counter}";

        if (counter == practiceGoal)
        {
            Debug.Log("Practice Goal Reached");
            goalText.text = "";
            practiceComplete = true;
            StartCountdown();
        }
    }
}