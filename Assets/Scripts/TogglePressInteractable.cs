using UnityEngine;
using TMPro;

public class TogglePressInteractable : MonoBehaviour
{

    public int[] ListOfOdds;
    private bool isPressed = false;
    public string TeamName1;
    public string TeamName2;
    public BetManager betManager;
    public CardManager cardManager;    
     public TextMeshPro OddsText;

  public bool isSpawnedCard = false; // Add this flag
    
    void Start()
    {
        if (!isSpawnedCard) // Only update if it's not a spawned card
        {
            UpdateUI();
        }
    }

    public void OnClick()
    {
        isPressed = !isPressed;

        int selectedOdds = ListOfOdds[sxr.GetTrial()];

        if (isPressed)
        {
            betManager.AddToCalculateOdds(selectedOdds);
            cardManager.SpawnCard(TeamName1, TeamName2, selectedOdds, this);
        }
        else
        {
            betManager.RemoveFromCalculateOdds(selectedOdds);
            cardManager.RemoveCard(this);
        }

        
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (OddsText != null)
        {
            OddsText.text = ListOfOdds[sxr.GetTrial()].ToString();
        }
    }
}
