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
    [SerializeField] private TextMeshPro WagerText;
    [SerializeField] private TextMeshPro ToWinText;
    [SerializeField] private TextMeshProUGUI PlaceBetText;
    [SerializeField] private TextMeshPro WinText;
    [SerializeField] private TextMeshPro LossText;
    [SerializeField] private TextMeshPro ErrorMessage;
    [SerializeField] private TextMeshPro WalletText;
    [SerializeField] private TextMeshPro CashOutPayoutText;
    [SerializeField] private Image PlaceBetButtonImage;
    [SerializeField] private PokeButton PlaceBetButton;
    [SerializeField] private Image BetSlipButtonImage;
    [SerializeField] private PokeButton BetSlipButton;
    [SerializeField] private TextMeshProUGUI BetSlipText;


    [SerializeField] private GameObject MiddleUI;
    [SerializeField] private GameObject CashOutWrapper;
    [SerializeField] private GameObject MiddleParlayUI;

    [SerializeField] private GameObject ParlayUI;
    [SerializeField] private GameObject BetslipUI;

    [SerializeField] private List<TogglePressInteractable> togglePressInteractables;

    [SerializeField] private CardManager CardManager;
    [SerializeField] private ParlayTutorial parlayTutorial;
    [SerializeField] private Driver driver;


    public bool TrialSubmitting => trialSubmitting;
    private bool trialSubmitting = false;
    public bool TrialCompleted => trialCompleted;
    private bool trialCompleted = false;
    [SerializeField] private bool AlreadySubmitting = false;
    private float wallet = 25f;


    private static float seconds = 4.5f;
    private List<bool> lastLegWins;

    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();
    private Dictionary<TogglePressInteractable, int> activeToggles = new Dictionary<TogglePressInteractable, int>();
    private List<ParlaySelection> currentParlaySelections = new List<ParlaySelection>();
    private static readonly Color DisabledColor = new Color(0.547f, 0.547f, 0.547f, 1f); // Gray color
    private static readonly Color EnabledColor = new Color(0.106f, 0.624f, 0.275f, 1f); // Green color
    private bool hasCompletedStatsTutorial = false;
    private static readonly float winLossVolume = 0.8f;
    private static readonly float winPitch = 1.2f;
    private static readonly float lossPitch = 0.8f;

    private void UpdateUI()
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
            BetSlipText.text = $"Select 3 or\nmore parlays";

            if(!hasCompletedStatsTutorial && sxr.GetTrial() == 0)
            {
                BetSlipText.text = $"Press stats button";
            }
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
                parlayTutorial.HideSelectParlayTutorial();
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

    private float CalculateParlayPayout()
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
        if(sxr.GetTotalOdds() != TotalOdds)
        {
            sxr.SetTotalLegs(oddsArray.Count);
            sxr.SetTotalOdds(TotalOdds);   
        }
        float payout = currentBet * totalMultiplier;
        sxr.SetPayout(payout);
        if (ToWinText != null)
            ToWinText.text = $"${payout:0.00}";

        return payout;
    }

    public void StartSubmit()
    {
        if(currentBet <= 0f || AlreadySubmitting)
            return;
        AlreadySubmitting = true;
        trialSubmitting = true;
        StartCoroutine(Submit());
    }

    public void SetOutcome(List<bool> legWins)
    {
        lastLegWins = legWins;
    }

    private IEnumerator Submit()
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

    private IEnumerator ResolveBet(List<bool> LegWins)
    {
        float Payout = 0f;
        CardManager.AnimateCardsColor(LegWins);

        yield return new WaitForSeconds(seconds);

        if (LegWins.Count(b => b) == decimalOddsList.Count)
        {
            Payout = CalculateParlayPayout();
            AddWallet(Payout);

            MiddleUI.SetActive(false);
            SoundManager.SoundManager.PlaySound3DOnce(SoundType.winAudio, WinText.transform.position, winLossVolume, winPitch);
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
            SoundManager.SoundManager.StopSound3D(SoundType.winAudio);
            yield return new WaitForSeconds(0.6f);
            WinText.text = "";
            WalletText.text = "";
        }
        else
        {
            sxr.SetPayout(0f);
            MiddleUI.SetActive(false);
            SoundManager.SoundManager.PlaySound3DOnce(SoundType.lossAudio, LossText.transform.position, winLossVolume, lossPitch);
            LossText.text = $"YOU LOST\n${currentBet:0.00}";
            WalletText.text = $"Wallet: ${wallet:0.00}";
            const float TextOffTime = 4f;
            yield return new WaitForSeconds(TextOffTime);
            SoundManager.SoundManager.StopSound3D(SoundType.lossAudio);
            LossText.text = "";
            WalletText.text = "";
        }
        MarkTrialComplete();
    }

    public void Reset()
    {
        StartNewTrial();
        trialSubmitting = false;
        const int restvalue = 0;
        currentBet = restvalue;
        sxr.SetBetAmount(restvalue);
        sxr.SetPayout(restvalue);
        CardManager.RemoveAllCards();
        oddsArray.Clear();
        decimalOddsList.Clear();
        activeToggles.Clear();
        currentParlaySelections.Clear();

        BetslipUI.SetActive(false);
        ParlayUI.SetActive(true);
        UpdateUI();
        UpdateOddsText();
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
        if ((wallet > 0f && wallet < increaseAmount) && !AlreadySubmitting)
        {
            currentBet += wallet;
            RemoveWallet(wallet);
            UpdateUI();
            return;
        }

        if (wallet < increaseAmount || AlreadySubmitting) return;

        currentBet += increaseAmount;
        RemoveWallet(increaseAmount);
        UpdateUI();
    }

    public void DecreaseParlayBet()
    {
        const float decreaseAmount = 1f;
        if ((currentBet - decreaseAmount) <= 0f || AlreadySubmitting) return;

        currentBet -= decreaseAmount;
        AddWallet(decreaseAmount);
        UpdateUI();
    }

    private void MarkTrialComplete()
    {
        trialCompleted = true;
    }

    private void StartNewTrial()
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

    private void TempDisableParlaySubmit()
    {
        StartCoroutine(TemporarilyDisableSubmit());
    }
    
    private IEnumerator TemporarilyDisableSubmit()
    {
        PlaceBetButton.DisableButton();
        const float buttonDisableTime = 1.5f;
        yield return new WaitForSeconds(buttonDisableTime);
        UpdateUI();
    }

    public void DisableButtons(float disableTime)
    {
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].AtStartDisableButtons(disableTime);
        }
    }

    public void TurnOffSelectStats(TogglePressInteractable selected)
    {
        if(sxr.GetTrial() == 0 && !hasCompletedStatsTutorial)
        {
            hasCompletedStatsTutorial = true;
            parlayTutorial.HideStatTutorial();
        }
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            if (togglePressInteractables[i] != selected)
            {
                togglePressInteractables[i].OnStatsDeselected();
            }
        }
    }

    public float GetWallet()
    {
        return wallet;
    }

    public void RunTutorial()
    {
        parlayTutorial.StartTutorial();
    }

}