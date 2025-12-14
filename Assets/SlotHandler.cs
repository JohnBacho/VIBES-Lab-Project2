using System.Collections;
using UnityEngine;
using TMPro;

public class SlotHandler : MonoBehaviour
{
    public TextMeshPro walletText;
    public TextMeshPro betText;
    public TextMeshPro EstimatedPayout;
    public TextMeshPro WinText;
    public TextMeshPro LossText;
    public TextMeshPro ErrorMessage;
    public AudioSource audioSource;


    public float currentBet = 0f;
    private int multiplier = 2;

    [SerializeField] private Reel[] reels;
    [SerializeField] private Handle handle;
    [SerializeField] private Driver driver;

    private int[] storedOutcome;
    public bool TrialCompleted => trialCompleted;
    private bool trialCompleted = false;

    private void Awake()
    {
        UpdateUI();
    }

    public void SetOutcome(int[] row)
    {
        storedOutcome = row;
    }

    public void SpinReceived()
    {
        if (storedOutcome == null)
        {
            Debug.LogError("No outcome set from Driver!");
            return;
        }

        foreach (Reel r in reels)
            r.Spin();

        StartCoroutine(StopReelsRoutine());
    }

    private IEnumerator StopReelsRoutine()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            yield return new WaitForSeconds(1f);
            reels[i].StopSpin(storedOutcome[i]);
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ResolveOutcome());
        trialCompleted = true;

        handle.ResetHandle();
    }

    private IEnumerator ResolveOutcome()
    {
        bool isWin = storedOutcome[0] == storedOutcome[1] && storedOutcome[1] == storedOutcome[2];
        EstimatedPayout.text = "";
        betText.text = "";
        if (isWin)
        {
            float winnings = currentBet * multiplier;
            GameManager.Instance.AddWallet(winnings + currentBet);
            audioSource.Play();
            walletText.text = $"Wallet: ${GameManager.Instance.wallet:0.00}";
            for(int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(0.5f);
                WinText.text = $"YOU WIN ${winnings}";
                yield return new WaitForSeconds(0.5f);
                WinText.text = "";
            }
            WinText.text = $"YOU WIN ${winnings}";
            walletText.text = $"Wallet: ${GameManager.Instance.wallet:0.00}";

        }
        else
        {
            LossText.text = $"YOU LOST ${currentBet}";
            WinText.text = "";
        }
        audioSource.Stop();
        walletText.text = $"Wallet: ${GameManager.Instance.wallet:0.00}";
        yield return new WaitForSeconds(3f);
        WinText.text = "";
        LossText.text = "";
        walletText.text = "";
        currentBet = 0f;
        UpdateUI();
    }


    // === UI & Betting ===

    public void UpdateUI()
    {
        if (betText != null)
            betText.text = $"Wager: ${currentBet:0.00}";

        if (EstimatedPayout != null)
            EstimatedPayout.text = $"To Win: ${currentBet * multiplier:0.00}";
    }

    public void IncreaseBet()
    {
        if (GameManager.Instance.wallet < 1f) return;

        currentBet += 1f;
        GameManager.Instance.RemoveWallet(1f);
        UpdateUI();
    }

    public void DecreaseBet()
    {
        if (currentBet <= 0f) return;

        currentBet -= 1f;
        GameManager.Instance.AddWallet(1f);
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
}
