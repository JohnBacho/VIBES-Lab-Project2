using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class SlotHandler : MonoBehaviour, ManageWallet
{
    public TextMeshPro walletText;
    public TextMeshPro betText;
    public TextMeshPro EstimatedPayout;
    public TextMeshPro WinText;
    public TextMeshPro LossText;
    public TextMeshPro ErrorMessage;
    public AudioSource WinAudioSource;
    public AudioSource LoseAudioSource;
    public PokeButton IncreaseBetButton;
    public PokeButton DecreaseBetButton;

    private float currentBet = 0f;
    private float multiplier;
    private float wallet = 100f;
    private const float FirstTrial= 0f;
    private const float ReelSpinDuration = 1f;

    [SerializeField] private Reel[] reels;
    [SerializeField] private Handle handle;
    [SerializeField] private Driver driver;
    [SerializeField] private SlotTutorial SlotTutorial;


    private int[] storedOutcome;
    public bool TrialCompleted => trialCompleted;
    private bool trialCompleted = false;
    private bool betPlaced = false;
    private void Awake()
    {
        UpdateUI();
    }

    public void SetOutcome(int[] row)
    {
        storedOutcome = row;
    }

    public void SetMultiplier(float SlotMultiplier)
    {
        multiplier = SlotMultiplier;
    }

    public void SpinReceived()
    {
        if (storedOutcome == null)
        {
            Debug.LogError("No outcome set from Driver!");
            return;
        }
        if(sxr.GetTrial() == FirstTrial)
        {
            SlotTutorial.HideGrabHandleTutorial();
        }
        IncreaseBetButton.DisableButton();
        DecreaseBetButton.DisableButton();

        foreach (Reel r in reels)
            r.Spin();

        StartCoroutine(StopReelsRoutine());
    }

    private IEnumerator StopReelsRoutine()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            yield return new WaitForSeconds(ReelSpinDuration);
            reels[i].StopSpin(storedOutcome[i]);
        }

        yield return new WaitForSeconds(ReelSpinDuration);

        StartCoroutine(ResolveOutcome());
    }

    private IEnumerator ResolveOutcome()
    {
        bool isWin = storedOutcome[0] == storedOutcome[1] && storedOutcome[1] == storedOutcome[2];
        EstimatedPayout.text = "";
        betText.text = "";
        if (isWin)
        {
            float winnings = currentBet * multiplier;
            AddWallet(winnings + currentBet);
            WinAudioSource.Play();
            walletText.text = $"Wallet: ${wallet:0.00}";
            const float TextOnTime = 0.5f;
            const float TextOffTime = 0.35f;
            for(int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(TextOnTime);
                WinText.text = $"YOU WIN ${winnings}";
                yield return new WaitForSeconds(TextOffTime);
                WinText.text = "";
            }
            WinText.text = $"YOU WIN ${winnings}";
            walletText.text = $"Wallet: ${wallet:0.00}";
        }
        else
        {
            LossText.text = $"YOU LOST ${currentBet}";
            WinText.text = "";
            walletText.text = $"Wallet: ${wallet:0.00}";
            LoseAudioSource.Play();
            const float TextOnTime = 4f;
            yield return new WaitForSeconds(TextOnTime);
        }
        WinAudioSource.Stop();
        LoseAudioSource.Stop();
        MarkTrialComplete();
    }

    public void rest()
    {
        handle.ResetHandle();
        WinText.text = "";
        LossText.text = "";
        walletText.text = "";
        currentBet = 0f;
        IncreaseBetButton.EnableButton();
        DecreaseBetButton.EnableButton();
        sxr.SetBetAmount(currentBet);   
        sxr.SetPayout(currentBet * multiplier);
        UpdateUI();
        betPlaced = false;
        StartCoroutine(PlaceBetText());
    }


    // === UI & Betting ===

    public void UpdateUI()
    {
        if (betText != null)
        {
            betText.text = $"Wager: ${currentBet:0.00}";
            Debug.Log("BetUpdated");
            sxr.SetBetAmount(currentBet);  
        }
        if (EstimatedPayout != null)
        {
            sxr.SetPayout(currentBet * multiplier);   
            EstimatedPayout.text = $"To Win: ${currentBet * multiplier:0.00}";
        }

        if (sxr.GetTrial() == 0 & !betPlaced)
        {
            StartCoroutine(PlaceBetText());
        } 
    }

    IEnumerator PlaceBetText()
    {
        yield return null;
        handle.DisableGrab();
        EstimatedPayout.text = "";
        const float TextOnTime = 0.8f;
        const float TextOffTime = 0.5f;
        while (!betPlaced)
        {
            if (!betPlaced)
            {
                betText.text = "Place a bet\nto continue";                
            }
            yield return new WaitForSeconds(TextOnTime);
            if (!betPlaced)
            {
                betText.text = "";                
            }
            yield return new WaitForSeconds(TextOffTime);
        }
    }

    public void IncreaseBet()
    {
        const float increaseAmount = 1f;
        if (wallet < increaseAmount) return;
        if(sxr.GetTrial() == FirstTrial)
        {
            SlotTutorial.HideIncreaseBetTutorial();
            SlotTutorial.ShowGrabHandleTutorial();
        }
        handle.EnableGrab();
        currentBet += increaseAmount;
        RemoveWallet(increaseAmount);
        betPlaced = true;
        UpdateUI();
    }

    public void DecreaseBet()
    {
    const float decreaseAmount = 1f;
    if (currentBet <= 0f)
        {
            handle.DisableGrab();
            return;
        }
        currentBet -= decreaseAmount;
        AddWallet(decreaseAmount);
        UpdateUI();
    }

    public void MarkTrialComplete()
    {
        trialCompleted = true;
    }

    public void StartNewTrial()
    {
        trialCompleted = false;
    }
    public void AddWallet(float amount)
    {
        wallet += amount;
        sxr.SetWallet(wallet);
    }

    public void RemoveWallet(float amount)
    {
        wallet -= amount;
        wallet = Mathf.Max(0, wallet);
        sxr.SetWallet(wallet);
    }

    public void AtStartDisableButtons()
    {
        StartCoroutine(TemporarilyDisableButtons());
    }
    
    IEnumerator TemporarilyDisableButtons()
    {
        IncreaseBetButton.DisableButton();
        DecreaseBetButton.DisableButton();
        const float buttonDisableTime = 2f;
        yield return new WaitForSeconds(buttonDisableTime);
        IncreaseBetButton.EnableButton();
        DecreaseBetButton.EnableButton();
    }
}
