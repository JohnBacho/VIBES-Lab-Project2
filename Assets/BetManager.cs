using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class BetManager : MonoBehaviour
{
    public float currentBet = 0f;
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

    private static float seconds = 4f;
    private bool canCashOut = false;
    private List<int> lastLegWins;

    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();

    void Start()
    {
        UpdateUI();

        if (mySlider != null)
        {
            mySlider.maxValue = GameManager.Instance.wallet;
            mySlider.minValue = 0;
            mySlider.SetValueWithoutNotify(0f);
            previousSliderValue = 0f;

            mySlider.onValueChanged.AddListener(OnSliderChanged);
            sliderInitialized = true;
        }
    }

    public void UpdateUI()
    {
        if (walletText != null)
            walletText.text = $"Wallet ${GameManager.Instance.wallet:0.00}";

        if (betText != null)
            betText.text = $"${currentBet:0.00}";

        if (EstimatedPayout != null)
            CalculateParlayPayout();
    }

    void TurnOffUI()
    {
        MiddleUI.SetActive(false);
        MiddleParlayUI.SetActive(false);
    }

    void OnSliderChanged(float newValue)
    {
        if (!sliderInitialized || mySlider == null) return;

        float maxAllowed = GameManager.Instance.wallet + previousSliderValue;
        float clampedValue = Mathf.Clamp(newValue, 0f, maxAllowed);
        float delta = clampedValue - previousSliderValue;

        GameManager.Instance.RemoveWallet(delta);

        currentBet = clampedValue;
        previousSliderValue = clampedValue;

        if (Mathf.Abs(clampedValue - newValue) > 0.0001f)
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
            float decimalOdds = 1f;

            if (odds > 0)
                decimalOdds = 1f + (odds / 100f);
            else if (odds < 0)
                decimalOdds = 1f + (100f / Mathf.Abs(odds));

            totalMultiplier *= decimalOdds;
            decimalOddsList.Add(decimalOdds);
        }

        float payout = currentBet * totalMultiplier;

        if (EstimatedPayout != null)
            EstimatedPayout.text = $"${payout:0.00}";

        return payout;
    }

    public void StartSubmit()
    {
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

    void CashOutUIToggle()
    {
        canCashOut = !canCashOut;
        CashOutWrapper.SetActive(canCashOut);
        MiddleUI.SetActive(!canCashOut);

        if (canCashOut)
            CashOutPayoutText.text = $"Cash Out Now For: ${CalculateParlayPayout() * 0.50f:0.00}";
        else
            CashOutPayoutText.text = "";
    }

    private IEnumerator ResolveBet(List<int> LegWins)
    {
        float Payout = 0f;

        CardManager.AnimateCardsColor(LegWins);

        yield return new WaitForSeconds(seconds);

        if (LegWins.Sum() == decimalOddsList.Count)
        {
            Payout = CalculateParlayPayout();
            GameManager.Instance.AddWallet(Payout);

            TurnOffUI();
            WinText.text = $"You Win!\nPayout: ${Payout:0.00}\nWallet: ${GameManager.Instance.wallet:0.00}";
            yield return new WaitForSeconds(seconds);
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
        previousSliderValue = 0f;

        CardManager.RemoveAllCards();
        oddsArray.Clear();
        decimalOddsList.Clear();

        if (mySlider != null)
        {
            mySlider.onValueChanged.RemoveAllListeners();
            mySlider.maxValue = GameManager.Instance.wallet;
            mySlider.minValue = 0;
            mySlider.SetValueWithoutNotify(0f);
            mySlider.onValueChanged.AddListener(OnSliderChanged);
        }

        leaderboard.SetMoney("You", GameManager.Instance.wallet);

        MiddleUI.SetActive(true);
        MiddleParlayUI.SetActive(true);

        UpdateUI();
        UpdateOddsText();
    }
}
