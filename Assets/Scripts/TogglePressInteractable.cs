using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TogglePressInteractable : MonoBehaviour
{
    [Header("Odds & Team Names")]
    public int[] Team1Odds;
    public int[] Team2Odds;
    public string[] Team1Names;
    public string[] Team2Names;

    [Header("Buttons")]
    public Button ButtonTeam1;
    public Button ButtonTeam2;

    [Header("UI Text")]
    public TextMeshPro OddsTextTeam1;
    public TextMeshPro OddsTextTeam2;
    public TextMeshPro Team1Text;
    public TextMeshPro Team2Text;

    [Header("Managers")]
    public BetManager betManager;
    public CardManager cardManager;

    private bool team1Selected = false;
    private bool team2Selected = false;
    public int teamIndex;


    void Start()
    {
        ButtonTeam1.onClick.AddListener(() => OnTeamSelected(1));
        ButtonTeam2.onClick.AddListener(() => OnTeamSelected(2));
        UpdateUI();
    }

    private void OnTeamSelected(int team)
    {
        int trial = sxr.GetTrial();

        if (team == 1)
        {
            team1Selected = !team1Selected;

            if (team1Selected)
            {
                DisableButton(ButtonTeam2);
                EnableButton(ButtonTeam1);
            Debug.Log("Team 1 Selected: " + team1Selected);

                int odds = Team1Odds[trial];
                betManager.AddToCalculateOdds(odds);
                cardManager.SpawnCard(Team1Names[trial], Team2Names[trial], odds, this);
            }
            else
            {
                EnableButton(ButtonTeam2);
                int odds = Team1Odds[trial];
                betManager.RemoveFromCalculateOdds(odds);
                cardManager.RemoveCard(this);
            }

            team2Selected = false;
        }

        else
        {
            team2Selected = !team2Selected;

            if (team2Selected)
            {
                DisableButton(ButtonTeam1);
                EnableButton(ButtonTeam2);
            Debug.Log("Team 2 Selected: " + team2Selected);

                int odds = Team2Odds[trial];
                betManager.AddToCalculateOdds(odds);
                cardManager.SpawnCard(Team1Names[trial], Team2Names[trial], odds, this);
            }
            else
            {
                EnableButton(ButtonTeam1);
                int odds = Team2Odds[trial];
                betManager.RemoveFromCalculateOdds(odds);
                cardManager.RemoveCard(this);
            }

            team1Selected = false;
        }

        UpdateUI();
    }

    private void DisableButton(Button btn)
    {
        btn.interactable = false;
    }

    private void EnableButton(Button btn)
    {
        btn.interactable = true;
    }

    public void UpdateUI()
    {
        int trial = sxr.GetTrial();

        if (OddsTextTeam1 != null)
            if(Team1Odds[trial] > 0)
            {
                OddsTextTeam1.text = "+" + Team1Odds[trial].ToString();
            }
            else
            {
            OddsTextTeam1.text = Team1Odds[trial].ToString();
            }
        if (OddsTextTeam2 != null)
            if(Team2Odds[trial] > 0)
            {
                OddsTextTeam2.text = "+" + Team2Odds[trial].ToString();
            }
            else
            {
            OddsTextTeam2.text = Team2Odds[trial].ToString();
            }
        if (Team1Text != null)
            Team1Text.text = Team1Names[trial];

        if (Team2Text != null)
            Team2Text.text = Team2Names[trial];
    }

    public void ResetToggle()
    {
        team1Selected = false;
        team2Selected = false;
        EnableButton(ButtonTeam1);
        EnableButton(ButtonTeam2);
        UpdateUI();
    }

    public int GetSelectedOdds()
    {
        int trial = sxr.GetTrial();
        return teamIndex == 1 ? Team1Odds[trial] : Team2Odds[trial];
    }

    public void SetPressed(bool pressed)
    {
        if (teamIndex == 1)
            team1Selected = pressed;
        else
            team2Selected = pressed;

        if (pressed)
        {
            if (teamIndex == 1)
            {
                ButtonTeam2.interactable = false;
            }
            else
            {
                ButtonTeam1.interactable = false;
            }
        }
        else
        {
            ButtonTeam1.interactable = true;
            ButtonTeam2.interactable = true;
        }

        UpdateUI();
    }


}
