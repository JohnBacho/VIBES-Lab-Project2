using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


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
    public GameObject MiddleUI;
    public Slider mySlider;
    private float previousSliderValue = 0f;

    private bool sliderInitialized = false;
    public TogglePressInteractable TogglePressInteractable1;
    public TogglePressInteractable TogglePressInteractable2;
    public TogglePressInteractable TogglePressInteractable3;
    public CardManager CardManager;

    public Leaderboard leaderboard;
    static float seconds = 5;



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
        float Payout = 0f;
        int counter = 0;
        foreach (float decimalOdds in decimalOddsList)
        {
            float probability = (1 / decimalOdds);
            float roll = UnityEngine.Random.value;
            if (roll <= probability)
            {
                counter++;
            }
            else
            {
                break;
            }
        }

        if (counter == decimalOddsList.Count)
        {
            Payout = CalculateParlayPayout();
            wallet += Payout;
            TurnOffUI();
            WinText.text = $"You Win! \n Payout: ${Payout:0.00},\nyour total wallet\nis now ${wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            WinText.text = "";
        }
        else
        {
            TurnOffUI();
            LossText.text = $"You Lose! \nyour total wallet\nis now ${wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            LossText.text = "";
        }
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
        UpdateUI();
        UpdateOddsText();
    }
    
    void UpdateOddsText()
        {
            TogglePressInteractable1.ResetToggle();
            TogglePressInteractable2.ResetToggle();
            TogglePressInteractable3.ResetToggle();
            TogglePressInteractable1.UpdateUI();
            TogglePressInteractable2.UpdateUI();
            TogglePressInteractable3.UpdateUI();
        }
}
