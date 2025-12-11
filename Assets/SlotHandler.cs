using System.Collections;
using System.Collections.Generic;
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
    private int multiplier = 2;
    public float currentBet = 0f;

    public void UpdateUI()
    {
        if (betText != null)
            betText.text = $"Wager: ${currentBet:0.00}";

        if (EstimatedPayout != null)
            CalculatePayout();
    }


    float CalculatePayout()
    {
        float payout = currentBet * multiplier;

        if (EstimatedPayout != null)
            EstimatedPayout.text = $"To Win: ${payout:0.00}";
        
        return payout;
    }


    public void IncreaseBet()
    {
        if(GameManager.Instance.wallet <= 1f)
            // Place error text here - John 
            return;
        currentBet += 1f;        
        GameManager.Instance.RemoveWallet(1f);        
        Debug.Log("Bet Increased");
        UpdateUI();
    }

    public void DecreaseBet()
    {
        if (currentBet <= 0f)
            // Place error text here - John 
            return;
        currentBet -= 1f;
        GameManager.Instance.AddWallet(1f);
        Debug.Log("Bet Decreased");
        UpdateUI();
    }

}
