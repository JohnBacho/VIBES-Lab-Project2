using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class BetManager : MonoBehaviour
{
    public float wallet = 100;
    public int currentBet = 0;
    public int betIncrement = 10;
    public TextMeshPro walletText;
    public TextMeshPro betText;
    public TextMeshPro ToggledOdds;
    public TextMeshPro EstimatedPayout;
    public TextMeshPro WinText;
    public TextMeshPro LossText;
    public TextMeshPro ErrorMessage;

    public TogglePressInteractable TogglePressInteractable1;
    public TogglePressInteractable TogglePressInteractable2;
    public TogglePressInteractable TogglePressInteractable3;
    public Leaderboard leaderboard;
    static float seconds = 5;



    private List<int> oddsArray = new List<int>();
    private List<float> decimalOddsList = new List<float>();


    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (walletText != null)
        {
            walletText.text = $"Wallet ${wallet:0.00}";
        }
        if (betText != null)
        {
            betText.text = "Bet $" + currentBet;
        }
        if (ToggledOdds != null)
        {
            ToggledOdds.text = "Selected Odds: " + string.Join(", ", oddsArray);
        }
        if (EstimatedPayout != null)
        {
            CalculateParlayPayout();
        }
    }

    void TurnOffUI()
    {
        if (walletText != null)
        {
            walletText.text = "";
        }
        if (betText != null)
        {
            betText.text = "";
        }
        if (ToggledOdds != null)
        {
            ToggledOdds.text = "";
        }
        if (EstimatedPayout != null)
        {
            EstimatedPayout.text = "";
        }
    }



    public void IncreaseBet()
    {
        if ((wallet - betIncrement) >= 0)
        {
            currentBet += betIncrement;
            wallet -= betIncrement;
            UpdateUI();
        }
        else
        {
            Debug.Log("invaild funds");
        }

    }

    public void DecreaseBet()
    {
        if (!(currentBet <= 0))
        {
            currentBet -= betIncrement;
            wallet += betIncrement;
            UpdateUI();
        }
        else
        {
            Debug.Log("no more");
        }

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
            EstimatedPayout.text = $"Current Payout: ${payout:0.00}";
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
            ErrorMessage.text = "Please select at least 2 bets\n for a parlay.";
            yield return new WaitForSeconds(seconds);
            ErrorMessage.text = "";
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
            WinText.text = $"You Win! Payout: ${Payout:0.00},\n your total wallet is now ${wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            WinText.text = "";
            currentBet = 0;
            oddsArray.Clear();
            decimalOddsList.Clear();
            sxr.NextTrial();
            UpdateUI();
            UpdateOddsText();
        }
        else
        {
            Debug.Log("You Lose!");
            currentBet = 0;
            TurnOffUI();
            LossText.text = $"You Lose! your total wallet\nis now ${wallet:0.00}";
            yield return new WaitForSeconds(seconds);
            LossText.text = "";
            oddsArray.Clear();
            decimalOddsList.Clear();
            sxr.NextTrial();
            UpdateUI();
            UpdateOddsText();

        }

        leaderboard.SetMoney("You", wallet);

    }
    
    void UpdateOddsText()
        {
            TogglePressInteractable1.UpdateUI();
            TogglePressInteractable2.UpdateUI();
            TogglePressInteractable3.UpdateUI();
        }





}
