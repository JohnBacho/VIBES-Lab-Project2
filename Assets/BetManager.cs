using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class BetManager : MonoBehaviour
{
    public float currentBet = 0f;
    public TextMeshPro WagerText;
    public TextMeshPro ToWinText;
    public TextMeshProUGUI PlaceBetText;
    public TextMeshPro WinText;
    public TextMeshPro LossText;
    public TextMeshPro ErrorMessage;
    public TextMeshPro CashOutPayoutText;
    public Image PlaceBetButtonImage;
    public Button PlaceBetButton;

    public GameObject MiddleUI;
    public GameObject CashOutWrapper;
    public GameObject MiddleParlayUI;

    public GameObject ParlayUI;
    public GameObject BetslipUI;

    public List<TogglePressInteractable> togglePressInteractables;
    public List<ToggleButton> toggleButton;

    public CardManager CardManager;
    public Leaderboard leaderboard;
    public Driver driver;
    public AudioSource WinningAudioSource;


    public bool TrialCompleted => trialCompleted;
    private bool trialCompleted = false;


    private static float seconds = 4f;
    private bool canCashOut = false;
    private List<int> lastLegWins;

    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();

    public void UpdateUI()
    {
        if (WagerText != null)
            WagerText.text = $"${currentBet:0.00}";

        if (ToWinText != null)
            CalculateParlayPayout();
        if (PlaceBetText != null)
        {
            if (currentBet <= 0f)
            {
                PlaceBetButton.interactable = false;
                PlaceBetButtonImage.color = new Color(0.5471698f, 0.5471698f, 0.5471698f, 1f); // Gray color
                PlaceBetText.text = $"Enter a wager to place a bet";
            }
            else
            {
                PlaceBetButton.interactable = true;   
                PlaceBetButtonImage.color = new Color(0.1058824f, 0.6235294f, 0.2745098f, 1f); // Green color
                PlaceBetText.text = $"Place ${currentBet:0} Bet";
            }


        }
        
            PlaceBetText.text = $"Place ${currentBet:0} Bet";
    }

    void TurnOffUI()
    {
        MiddleUI.SetActive(false);
        MiddleParlayUI.SetActive(false);
    }

    void TurnOnUI()
    {
        MiddleUI.SetActive(true);
        MiddleParlayUI.SetActive(true);
    }

    public void AddToCalculateOdds(int odds)
    {
        oddsArray.Add(odds);
    }

    public void RemoveFromCalculateOdds(int odds)
    {
        oddsArray.Remove(odds);
        UpdateUI();
    }

    public float CalculateParlayPayout()
    {
        float totalMultiplier = 1f;
        decimalOddsList.Clear();

        foreach (int odds in oddsArray)
        {
            float decimalOdds = 1f;

            if (odds > 0)
                decimalOdds = 1f + (odds / 100f);
            else if (odds < 0)
                decimalOdds = 1f + (100f / Mathf.Abs(odds));

            totalMultiplier *= decimalOdds;
            decimalOddsList.Add(decimalOdds);
        }

        float payout = currentBet * totalMultiplier;

        if (ToWinText != null)
            ToWinText.text = $"${payout:0.00}";

        return payout;
    }

    public void StartSubmit()
    {
        if(currentBet == 0f)
            return;
        StartCoroutine(Submit());
    }

    public void SetOutcome(List<int> legWins)
    {
        lastLegWins = legWins;
    }

    public IEnumerator Submit()
    {
        if (oddsArray.Count < 3)
        {
            TurnOffUI();
            ErrorMessage.text = "Please select at\nleast 3 bets\nfor a parlay.";
            yield return new WaitForSeconds(seconds);
            ErrorMessage.text = "";
            MiddleUI.SetActive(true);
            yield break;
        }
        driver.ParlayOutcome(oddsArray.Count);
        yield return ResolveBet(lastLegWins);
    }

    void UpdateOddsText()
    {
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].ResetToggle();
            togglePressInteractables[i].UpdateUI();
        }
    }

    private IEnumerator ResolveBet(List<int> LegWins)
    {
        float Payout = 0f;

        CardManager.AnimateCardsColor(LegWins);

        yield return new WaitForSeconds(seconds);
        MarkTrialComplete();

        if (LegWins.Sum() == decimalOddsList.Count)
        {
            Payout = CalculateParlayPayout();
            GameManager.Instance.AddWallet(Payout);

            TurnOffUI();
            WinningAudioSource.Play();
            WinText.text = $"You Win!\nPayout: ${Payout:0.00}\nWallet: ${GameManager.Instance.wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            WinningAudioSource.Stop();
            WinText.text = "";
        }
        else
        {
            TurnOffUI();
            LossText.text = $"You Lose!\nWallet: ${GameManager.Instance.wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            LossText.text = "";
        }
        ResetRound();
    }

    private void ResetRound()
    {
        currentBet = 0;
        CardManager.RemoveAllCards();
        oddsArray.Clear();
        decimalOddsList.Clear();

        leaderboard.SetMoney("You", GameManager.Instance.wallet);

        BetslipUI.SetActive(false);
        ParlayUI.SetActive(true);
        UpdateUI();
        UpdateOddsText();
        for (int i = 0; i < toggleButton.Count; i++)
        {
            toggleButton[i].SetToggled(false);
        }
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].UpdateUI();
        }

    }

    public void ViewBetslip()
    {
        BetslipUI.SetActive(true);
        TurnOnUI();
        ParlayUI.SetActive(false);
        UpdateUI();
    }

    public void ViewParlays()
    {
        BetslipUI.SetActive(false);
        ParlayUI.SetActive(true);
    }

    public void IncreaseParlayBet()
    {
        if (GameManager.Instance.wallet < 1f) return;

        currentBet += 1f;
        GameManager.Instance.RemoveWallet(1f);
        UpdateUI();
    }

    public void DecreaseParlayBet()
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
