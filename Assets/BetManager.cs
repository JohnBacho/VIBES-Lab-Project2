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


    private List<int> oddsArray = new List<int>();
    private List<double> PercentagesArray = new List<double>();

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (walletText != null)
        {
            walletText.text = "Wallet $" + wallet;
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
        }
        float payout = currentBet * totalMultiplier;
        if (EstimatedPayout != null)
        {
            EstimatedPayout.text = $"Current Payout: ${payout:0.00}";
        }
        UpdateUI();
        return payout;
    }

    public void CalculatePercentage()
    {
        PercentagesArray.Clear();

        for (int i = 0; i < oddsArray.Count; i++)
        {
            int odds = oddsArray[i];
            double probability;

            if (odds < 0)
            {
                probability = (double)Math.Abs(odds) / (Math.Abs(odds) + 100);
            }
            else
            {
                probability = 100.0 / (odds + 100);
            }

            PercentagesArray.Add(probability * 100);
        }
    }

    public void submit()
    {
        CalculatePercentage();
        int counter = 0;
        foreach (double chance in PercentagesArray)
        {
            int result = RandomWithChance((int)chance);
            if (result == 1)
            {
                counter++;
            }
            else
            {
                break;
            }
        }

        if (counter == PercentagesArray.Count)
        {
            Debug.Log("You Win!");
            wallet += currentBet * 2;
            currentBet = 0;
            oddsArray.Clear();
            PercentagesArray.Clear();
            sxr.NextTrial();
            UpdateUI();
        }
        else
        {
            Debug.Log("You Lose!");
            currentBet = 0;
            oddsArray.Clear();
            PercentagesArray.Clear();
            sxr.NextTrial();
            UpdateUI();
        }
    }


    int RandomWithChance(int percent)
    {
        int roll = UnityEngine.Random.Range(0, 100);
        return (roll < percent) ? 1 : 0;
    }


}
