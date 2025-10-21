using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;


public class BetManager : MonoBehaviour
{
    public float wallet = 100;
    public float currentBet = 0;
    public TextMeshPro walletText;
    public TextMeshPro betText;
    public TextMeshPro EstimatedPayout;
    public TextMeshPro WinText;
    public TextMeshPro LossText;
    public TextMeshPro ErrorMessage;
    public TextMeshPro CashOutPayoutText;
    public GameObject MiddleUI;
    public GameObject CashOutWrapper;
    public GameObject MiddleParlayUI;
    public Slider mySlider;
    private float previousSliderValue = 0f;

    private bool sliderInitialized = false;
    public List<TogglePressInteractable> togglePressInteractables;
    public CardManager CardManager;

    public Leaderboard leaderboard;
    static float seconds = 4;
    private bool canCashOut = false;

    private List<int> lastLegWins;




    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();


    void Start()
    {
        UpdateUI();

        if (mySlider != null)
        {
            mySlider.maxValue = wallet;
            mySlider.minValue = 0;
            mySlider.SetValueWithoutNotify(0f); // start at 0 so no phantom delta
            previousSliderValue = 0f;

            mySlider.onValueChanged.AddListener(OnSliderChanged);
            sliderInitialized = true;
        }
    }


    void UpdateUI()
    {
        if (walletText != null)
        {
            walletText.text = $"Wallet ${wallet:0.00}";
        }
        if (betText != null)
        {
            betText.text = "$" + currentBet;
        }
        if (EstimatedPayout != null)
        {
            CalculateParlayPayout();
        }
    }

    void TurnOffUI()
    {
        MiddleUI.SetActive(false);
        MiddleParlayUI.SetActive(false);
    }


    void OnSliderChanged(float newValue)
    {
        if (!sliderInitialized || mySlider == null) return;

        float maxAllowed = wallet + previousSliderValue;
        float clampedValue = Mathf.Clamp(newValue, 0f, maxAllowed);
        float delta = clampedValue - previousSliderValue;

        wallet -= delta;
        wallet = Mathf.Max(wallet, 0f);

        currentBet = clampedValue;
        previousSliderValue = clampedValue;

        if (Math.Abs(clampedValue - newValue) > 0.0001f)
            mySlider.SetValueWithoutNotify(clampedValue);

        UpdateUI();
    }
    public void AddToCalculateOdds(int odds)
    {
        oddsArray.Add(odds);
        UpdateUI();
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
            float decimalOdds = 0f;

            if (odds > 0)
            {
                decimalOdds = 1f + (odds / 100f);
            }
            else if (odds < 0)
            {
                decimalOdds = 1f + (100f / Mathf.Abs(odds));
            }
            else
            {
                decimalOdds = 1f;
            }

            totalMultiplier *= decimalOdds;
            decimalOddsList.Add(decimalOdds);
        }
        float payout = currentBet * totalMultiplier;
        if (EstimatedPayout != null)
        {
            EstimatedPayout.text = $"${payout:0.00}";
        }
        return payout;
    }

    public void StartSubmit()
    {
        StartCoroutine(Submit());
    }


    public IEnumerator Submit()
    {
        if (oddsArray.Count < 2)
        {
            TurnOffUI();
            ErrorMessage.text = "Please select at\nleast 2 bets\nfor a parlay.";
            yield return new WaitForSeconds(seconds);
            ErrorMessage.text = "";
            MiddleUI.SetActive(true);
            yield break;
        }
        List<int> LegWins = new List<int>();

        foreach (float decimalOdds in decimalOddsList)
        {
            float probability = (1 / decimalOdds);
            Debug.Log($"Probability: {probability}");

            float roll = UnityEngine.Random.value;
            Debug.Log($"Roll: {roll}");
            if (roll <= probability)
            {
                LegWins.Add(1);
            }
            else
            {
                LegWins.Add(0);
            }
        }

        if ((LegWins.Sum() == decimalOddsList.Count - 1 || LegWins.Sum() == decimalOddsList.Count) && decimalOddsList.Count >= 3)
        {
            Debug.Log("Near miss");
            lastLegWins = new List<int>(LegWins);
            StartCoroutine(HandleNearMiss(LegWins));
            yield break;
        }


        yield return ResolveBet(LegWins, false);
    }

    void UpdateOddsText()
    {

        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].ResetToggle();
            togglePressInteractables[i].UpdateUI();
        }
    }

    void CashOutUIToggle()
    {
        canCashOut = !canCashOut;
        CashOutWrapper.SetActive(canCashOut);
        MiddleUI.SetActive(false);
        if (canCashOut)
        {
            CashOutPayoutText.text = $"Cash Out Now For: ${CalculateParlayPayout() * 0.50f:0.00}";
        }
        else
        {
            CashOutPayoutText.text = $"";
        }
    }

    private IEnumerator HandleNearMiss(List<int> legWins)
    {
        if (legWins.All(x => x == 1))
        {
            Debug.Log("All wins - removing one for near miss");
            legWins[UnityEngine.Random.Range(0, legWins.Count)] = 0;
            CardManager.AnimateCardsExceptUnrevealed(legWins);
        }
        else
        {
            CardManager.AnimateCardsExceptUnrevealed(legWins);
        }
        CashOutUIToggle(); 
        yield return null;
    }

    public void YesToCashout()
    {
        CashOutUIToggle();
        StartCoroutine(FinishNearMissRound());
    }

    public void NoToCashout()
    {
        CashOutUIToggle();
        StartCoroutine(ResolveBet(lastLegWins, true));
    }


    private IEnumerator FinishNearMissRound()
    {
        Debug.Log($"[YES] lastLegWins = {(lastLegWins == null ? "null" : string.Join(",", lastLegWins))}");
        CardManager.RevealUnrevealedCards(lastLegWins);

        yield return new WaitForSeconds(seconds);
        TurnOffUI();
        float partialPayout = CalculateParlayPayout() * 0.50f; // cash-out 50%
        Debug.Log($"Cashed out for ${partialPayout:0.00}");
        wallet += partialPayout;
        WinText.text = $"You cashed out early!\nWon ${partialPayout:0.00}\nWallet: ${wallet:0.00}";
        yield return new WaitForSeconds(seconds);
        WinText.text = "";
        ResetRound();
        lastLegWins = null;
    }


    private IEnumerator ResolveBet(List<int> LegWins, bool isNearMiss)
    {
        float Payout = 0f;
        if (isNearMiss)
        {
            CardManager.RevealUnrevealedCards(LegWins);
        }
        else
            CardManager.AnimateCardsColor(LegWins);

        yield return new WaitForSeconds(seconds);

        if (LegWins.Sum() == decimalOddsList.Count)
        {
            Payout = CalculateParlayPayout();
            wallet += Payout;
            TurnOffUI();
            WinText.text = $"You Win! \n Payout: ${Payout:0.00}\nWallet: ${wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            WinText.text = "";
        }
        else
        {
            TurnOffUI();
            LossText.text = $"You Lose!\nWallet: ${wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            LossText.text = "";
        }

        ResetRound();
    }

    private void ResetRound()
    {
        currentBet = 0;
        previousSliderValue = 0f;

        CardManager.RemoveAllCards();
        oddsArray.Clear();
        decimalOddsList.Clear();

        mySlider.onValueChanged.RemoveListener(OnSliderChanged);
        mySlider.maxValue = wallet;
        mySlider.minValue = 0;
        mySlider.SetValueWithoutNotify(0f);
        sxr.NextTrial();

        leaderboard.SetMoney("You", wallet);
        mySlider.onValueChanged.AddListener(OnSliderChanged);
        MiddleUI.SetActive(true);
        MiddleParlayUI.SetActive(true);
        UpdateUI();
        UpdateOddsText();
    }

}
