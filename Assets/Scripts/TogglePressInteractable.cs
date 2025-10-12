using UnityEngine;
using TMPro;

public class TogglePressInteractable : MonoBehaviour
{

    public int[] ListOfOdds;
    private bool isPressed = false;
    public string[] ListOfTeamName1;
    public string[] ListOfTeamName2;
    public BetManager betManager;
    public CardManager cardManager;
    public TextMeshPro OddsText;
    public TextMeshPro Team1Text;

    public TextMeshPro Team2Text;

    void Start()
    {
        UpdateUI();
    }

    public void OnClick()
    {
        isPressed = !isPressed;

        int selectedOdds = ListOfOdds[sxr.GetTrial()];
        string TeamName1 = ListOfTeamName1[sxr.GetTrial()];
        string TeamName2 = ListOfTeamName2[sxr.GetTrial()];

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

        SetPressed(isPressed);
        UpdateUI();
    }
    
    public void SetPressed(bool pressed)
    {
        isPressed = pressed;
        
        ToggleButton toggleButton = GetComponent<ToggleButton>();
        if (toggleButton != null)
        {
            toggleButton.SetToggled(pressed);
        }
        
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (OddsText != null)
        {
            OddsText.text = ListOfOdds[sxr.GetTrial()].ToString();
        }
        if (Team1Text != null)
        {
            Team1Text.text = ListOfTeamName1[sxr.GetTrial()];
        }
        if (Team2Text != null)
        {
            Team2Text.text = ListOfTeamName2[sxr.GetTrial()];
        }
    }

    public void ResetToggle()
    {
        isPressed = false;
        SetPressed(false);
        UpdateUI();
    }
}
