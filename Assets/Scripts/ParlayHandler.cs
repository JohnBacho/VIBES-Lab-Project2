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

public class ParlayHandler : MonoBehaviour, ManageWallet
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
    public List<XRPokeToggleButton> StatButtons;



    public CardManager CardManager;
    public ParlayTutorial parlayTutorial;
    public Leaderboard leaderboard;
    public Driver driver;
    public AudioSource WinningAudioSource;


    public bool TrialCompleted => trialCompleted;
    private bool trialCompleted = false;
    public bool AlreadySubmitting = false;
    private float wallet = 100f;


    private static float seconds = 4.5f;
    private List<int> lastLegWins;

    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();
    private Dictionary<TogglePressInteractable, int> activeToggles = new Dictionary<TogglePressInteractable, int>();
    private List<ParlaySelection> currentParlaySelections = new List<ParlaySelection>();
    private static readonly Color DisabledColor = new Color(0.547f, 0.547f, 0.547f, 1f); // Gray color
    private static readonly Color EnabledColor = new Color(0.106f, 0.624f, 0.275f, 1f); // Green color

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
                PlaceBetButtonImage.color = DisabledColor;
                PlaceBetText.text = $"Place a bet\nto continue";
            }
            else
            {
                PlaceBetButton.EnableButton();   
                PlaceBetButtonImage.color = EnabledColor;
                PlaceBetText.text = $"Place ${currentBet:0} Bet";
            }
        }
        if (oddsArray.Count < 3)
        {
            BetSlipButton.DisableButton();
            BetSlipButtonImage.color = DisabledColor;
            BetSlipText.text = $"Select 3 or more parlays";
        }
        else
        {
            BetSlipButtonImage.color = EnabledColor;
            BetSlipButton.EnableButton();
            BetSlipText.text = $"View Betslip";
        }
}

    public void AddToCalculateOdds(int odds, TogglePressInteractable toggle, string teamName)
    {
        if (!activeToggles.ContainsKey(toggle))
        {
            if(sxr.GetTrial() == 0)
            {
                parlayTutorial.HideGrabHandleTutorial();
            }
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
        sxr.SetTotalLegs(oddsArray.Count);
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
        if(currentBet == 0f || AlreadySubmitting)
            return;
        AlreadySubmitting = true;    
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
            const float TextOnTime = 0.5f;
            const float TextOffTime = 0.35f;
            for(int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(TextOnTime);
                WinText.text = $"YOU WIN\n${Payout:0.00}";
                yield return new WaitForSeconds(TextOffTime);
                WinText.text = "";
            }
            WinText.text = $"YOU WIN\n${Payout:0.00}";
            yield return new WaitForSeconds(TextOnTime);
            WinningAudioSource.Stop();
            WinText.text = "";
            WalletText.text = "";
        }
        else
        {
            MiddleUI.SetActive(false);
            LossText.text = $"YOU LOST\n${currentBet:0.00}";
            WalletText.text = $"Wallet: ${wallet:0.00}";
            const float TextOffTime = 4f;
            yield return new WaitForSeconds(TextOffTime);
            LossText.text = "";
            WalletText.text = "";
        }
        MarkTrialComplete();
    }

    public void ResetRound()
    {
        const int restvalue = 0;
        currentBet = restvalue;
        sxr.SetBetAmount(restvalue);
        sxr.SetPayout(restvalue);
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

        foreach (var s in StatButtons)
        {
            s.ForceReset();
            s.SetNormalColor();
        }

        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].UpdateUI();
            togglePressInteractables[i].ResetToggle();
            togglePressInteractables[i].OnStatsDeselected();
        }
        AlreadySubmitting = false;        

    }

    public void ViewBetslip()
    {
        ParlayUI.SetActive(false);
        parlayTutorial.HideSelectParlayTutorial();
        BetslipUI.SetActive(true);
        MiddleUI.SetActive(true);
        UpdateUI();
        TempDisableParlaySubmit();
    }
    public void ViewParlays()
    {
        if(AlreadySubmitting) return;
        BetslipUI.SetActive(false);
        ParlayUI.SetActive(true);
    }

    public void IncreaseParlayBet()
    {
        const float increaseAmount = 1f;
        if (wallet < increaseAmount || AlreadySubmitting) return;

        currentBet += increaseAmount;
        RemoveWallet(increaseAmount);
        UpdateUI();
    }

    public void DecreaseParlayBet()
    {
        const float decreaseAmount = 1f;
        if (currentBet <= 0f || AlreadySubmitting) return;

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

    public void TempDisableParlaySubmit()
    {
        StartCoroutine(TemporarilyDisableSubmit());
    }
    
    IEnumerator TemporarilyDisableSubmit()
    {
        PlaceBetButton.DisableButton();
        const float buttonDisableTime = 1.5f;
        yield return new WaitForSeconds(buttonDisableTime);
        UpdateUI();
    }

    public void AtStartDisableButtons()
    {
        StartCoroutine(TemporarilyDisableButtons());
    }
    
    IEnumerator TemporarilyDisableButtons()
    {
        yield return null;
        foreach (var t in toggleButton)
        {
            t.DisableInteraction();
        }
        const float buttonDisableTime = 2f;
        yield return new WaitForSeconds(buttonDisableTime);
        foreach (var t in toggleButton)
        {
            t.EnableInteraction();
        }
    }

    public void TurnOffSelectStats(TogglePressInteractable selected)
    {
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            if (togglePressInteractables[i] != selected)
            {
                togglePressInteractables[i].OnStatsDeselected();
                StatButtons[i].ForceReset();
            }
        }
    }

}