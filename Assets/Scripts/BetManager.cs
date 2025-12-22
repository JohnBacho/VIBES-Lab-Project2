using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


[System.Serializable]
public class ParlaySelection
{
    public string teamName;
    public int odds;
    public TogglePressInteractable toggle; 
    
    public ParlaySelection(string name, int oddsValue, TogglePressInteractable associatedToggle)
    {
        teamName = name;
        odds = oddsValue;
        toggle = associatedToggle;
    }
}

public class BetManager : MonoBehaviour, ManageWallet
{
    private float currentBet = 0f;
    public TextMeshPro WagerText;
    public TextMeshPro ToWinText;
    public TextMeshProUGUI PlaceBetText;
    public TextMeshPro WinText;
    public TextMeshPro LossText;
    public TextMeshPro ErrorMessage;
    public TextMeshPro WalletText;
    public TextMeshPro CashOutPayoutText;
    public Image PlaceBetButtonImage;
    public PokeButton PlaceBetButton;
    public Image BetSlipButtonImage;
    public PokeButton BetSlipButton;
    public TextMeshProUGUI BetSlipText;


    public GameObject MiddleUI;
    public GameObject CashOutWrapper;
    public GameObject MiddleParlayUI;

    public GameObject ParlayUI;
    public GameObject BetslipUI;

    public List<TogglePressInteractable> togglePressInteractables;
    public List<XRPokeToggleButton> toggleButton;

    public CardManager CardManager;
    public Leaderboard leaderboard;
    public Driver driver;
    public AudioSource WinningAudioSource;


    public bool TrialCompleted => trialCompleted;
    private bool trialCompleted = false;
    public bool AlreaydSubmitting = false;
    private float wallet = 100f;


    private static float seconds = 4.5f;
    private List<int> lastLegWins;

    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();
    private Dictionary<TogglePressInteractable, int> activeToggles = new Dictionary<TogglePressInteractable, int>();
    private List<ParlaySelection> currentParlaySelections = new List<ParlaySelection>();

    public void UpdateUI()
    {
        if (WagerText != null)
            WagerText.text = $"${currentBet:0.00}";
            sxr.SetBetAmount(currentBet);

        if (ToWinText != null)
            CalculateParlayPayout();
        if (PlaceBetText != null)
        {
            if (currentBet <= 0f)
            {
                PlaceBetButton.DisableButton();
                PlaceBetButtonImage.color = new Color(0.5471698f, 0.5471698f, 0.5471698f, 1f); // Gray color
                PlaceBetText.text = $"Place a bet\nto continue";
            }
            else
            {
                PlaceBetButton.EnableButton();   
                PlaceBetButtonImage.color = new Color(0.1058824f, 0.6235294f, 0.2745098f, 1f); // Green color
                PlaceBetText.text = $"Place ${currentBet:0} Bet";
            }
        }
        if (oddsArray.Count < 3)
        {
            BetSlipButton.DisableButton();
            BetSlipButtonImage.color = new Color(0.5471698f, 0.5471698f, 0.5471698f, 1f); // Gray color
            BetSlipText.text = $"Select 3 or more parlays";
        }
        else
        {
            BetSlipButtonImage.color = new Color(0.1058824f, 0.6235294f, 0.2745098f, 1f); // Green color
            BetSlipButton.EnableButton();
            BetSlipText.text = $"View Betslip";
        }
}

    public void AddToCalculateOdds(int odds, TogglePressInteractable toggle, string teamName)
    {
        if (!activeToggles.ContainsKey(toggle))
        {
            oddsArray.Add(odds);
            activeToggles[toggle] = odds;
            currentParlaySelections.Add(new ParlaySelection(teamName, odds, toggle));
            sxr.SetParlaySelection(GetParlayDataString());
        }
        UpdateUI();
    }


    public void RemoveFromCalculateOdds(int odds, TogglePressInteractable toggle)
    {
        if (activeToggles.ContainsKey(toggle))
        {
            int storedOdds = activeToggles[toggle];
            oddsArray.Remove(storedOdds);
            
            currentParlaySelections.RemoveAll(p => p.toggle == toggle);
            sxr.SetParlaySelection(GetParlayDataString());
            activeToggles.Remove(toggle);
        }
        UpdateUI();
    }

    public float CalculateParlayPayout()
    {
        float totalMultiplier = 1f;
        float TotalOdds = 0f;
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
        TotalOdds = (totalMultiplier - 1f) * 100f;
        sxr.SetTotalOdds(TotalOdds);
        Debug.Log($"Total Odds: {TotalOdds}");
        float payout = currentBet * totalMultiplier;
        sxr.SetPayout(payout);
        if (ToWinText != null)
            ToWinText.text = $"${payout:0.00}";

        return payout;
    }

    public void StartSubmit()
    {
        if(currentBet == 0f || AlreaydSubmitting)
            return;
        AlreaydSubmitting = true;    
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
            MiddleUI.SetActive(false);
            ErrorMessage.text = "Please select at\nleast 3 bets\nfor a parlay.";
            yield return new WaitForSeconds(seconds);
            ErrorMessage.text = "";
            MiddleUI.SetActive(true);
            yield break;
        }
        driver.ParlayOutcome(oddsArray.Count);
        yield return ResolveBet(lastLegWins);
    }

    public void UpdateOddsText()
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

        if (LegWins.Sum() == decimalOddsList.Count)
        {
            Payout = CalculateParlayPayout();
            AddWallet(Payout);

            MiddleUI.SetActive(false);
            WinningAudioSource.Play();
            WalletText.text = $"Wallet: ${wallet:0.00}";
            for(int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(0.5f);
                WinText.text = $"YOU WIN\n${Payout:0.00}";
                yield return new WaitForSeconds(0.35f);
                WinText.text = "";
            }
            WinText.text = $"YOU WIN\n${Payout:0.00}";
            yield return new WaitForSeconds(.5f);
            WinningAudioSource.Stop();
            WinText.text = "";
            WalletText.text = "";
        }
        else
        {
            MiddleUI.SetActive(false);
            LossText.text = $"You Lose!";
            WalletText.text = $"Wallet: ${wallet:0.00}";
            yield return new WaitForSeconds(4f);
            LossText.text = "";
            WalletText.text = "";
        }
        MarkTrialComplete();
    }

    public void ResetRound()
    {
        currentBet = 0;
        sxr.SetBetAmount(0);
        sxr.SetPayout(0);
        CardManager.RemoveAllCards();
        oddsArray.Clear();
        decimalOddsList.Clear();
        activeToggles.Clear();
        currentParlaySelections.Clear();
        UpdateLeaderboard();

        BetslipUI.SetActive(false);
        ParlayUI.SetActive(true);
        UpdateUI();
        UpdateOddsText();
        foreach (var t in toggleButton)
        {
            t.ForceReset();
            t.SetNormalColor();
        }
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].UpdateUI();
            togglePressInteractables[i].ResetToggle();
        }
        AlreaydSubmitting = false;        

    }

    public void ViewBetslip()
    {
        ParlayUI.SetActive(false);
        BetslipUI.SetActive(true);
        MiddleUI.SetActive(true);
        UpdateUI();
        foreach (var t in toggleButton)
        {
            t.ForceReset();
        }

    }
    public void ViewParlays()
    {
        BetslipUI.SetActive(false);
        ParlayUI.SetActive(true);
    }

    public void IncreaseParlayBet()
    {
        if (wallet < 1f || AlreaydSubmitting) return;

        currentBet += 1f;
        RemoveWallet(1f);
        UpdateUI();
    }

    public void DecreaseParlayBet()
    {
        if (currentBet <= 0f || AlreaydSubmitting) return;

        currentBet -= 1f;
        AddWallet(1f);
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

    public string GetParlayDataString()
    {
        string parlayData = "";
        
        for (int i = 0; i < 5; i++)
        {
            if (i < currentParlaySelections.Count)
            {
                parlayData += currentParlaySelections[i].teamName + "," + 
                            currentParlaySelections[i].odds + ",";
            }
            else
            {
                parlayData += ",,";
            }
        }
        
        return parlayData;
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
    public void UpdateLeaderboard()
    {
        leaderboard.SetMoney("You", wallet);
    }
}