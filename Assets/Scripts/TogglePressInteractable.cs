using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;


[Serializable]
public class ParlayManager
{
    public List<Parlay> Parlays;
}

[Serializable]
public class Parlay
{
    public string Team1Name;
    public int Team1Odds;
    public string Team2Name;
    public int Team2Odds;
    public string Team1WinLoss;
    public TeamHealth Team1Health;
    public string Team2WinLoss;
    public TeamHealth Team2Health;
}

public enum TeamHealth
{
    Good,
    Medium,
    Bad
}

public class TogglePressInteractable : MonoBehaviour
{
    public ParlayManager parlayManager;

    [Header("XR Poke Toggles")]
    [SerializeField] private XRPokeToggleButton ToggleTeam1;
    [SerializeField] private XRPokeToggleButton ToggleTeam2;
    [SerializeField] private XRPokeToggleButton ToggleStats;
    [SerializeField] private Pulse pulse;



    [Header("UI Text")]
    [SerializeField] private TextMeshPro OddsTextTeam1;
    [SerializeField] private TextMeshPro OddsTextTeam2;
    [SerializeField] private TextMeshPro Team1Text;
    [SerializeField] private TextMeshPro Team2Text;
    [Header("Stats UI Text")]
    [SerializeField] private TextMeshPro WinLossTeam1;
    [SerializeField] private TextMeshPro WinLossTeam2;
    [SerializeField] private TextMeshPro Team1Abbr;
    [SerializeField] private TextMeshPro Team2Abbr;
    [SerializeField] private GameObject Stats;
    [SerializeField] private GameObject[] Team1Health = new GameObject[3];
    [SerializeField] private GameObject[] Team2Health = new GameObject[3];


    [Header("Managers")]
    public ParlayHandler parlayHandler;
    public CardManager cardManager;

    private bool team1Selected = false;
    private bool team2Selected = false;
    private int teamIndex;
    private Color TextColorNormal = new Color32(0x1F, 0x37, 0x5B, 0xFF);
    private bool isResetting = false;
    private bool tutorialFlag = false;



    void Start()
    {
        if (ToggleTeam1 != null)
        {
            ToggleTeam1.onToggledOn.AddListener(() => OnTeamSelected(1));
            ToggleTeam1.onToggledOff.AddListener(() => OnTeamDeselected(1));
        }

        if (ToggleTeam2 != null)
        {
            ToggleTeam2.onToggledOn.AddListener(() => OnTeamSelected(2));
            ToggleTeam2.onToggledOff.AddListener(() => OnTeamDeselected(2));
        }

        if (ToggleStats != null)
        {
            ToggleStats.onToggledOn.AddListener(OnStatsSelected);
            ToggleStats.onToggledOff.AddListener(OnStatsDeselected);
        }

        Stats.SetActive(false);

    }

    private void OnTeamSelected(int team)
    {
        if (isResetting) return;
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];

        if (team == 1)
        {
            team1Selected = true;
            team2Selected = false;

            ToggleTeam1.PlayUISound();

            ToggleTeam1.SetToggledColor();
            ToggleTeam2.SetDisableColor();

            OddsTextTeam1.color = Color.white;
            OddsTextTeam2.color = Color.white;

            teamIndex = 1;
            parlayHandler.AddToCalculateOdds(currentParlay.Team1Odds, this, currentParlay.Team1Name);
            cardManager.SpawnCard(currentParlay.Team1Name, currentParlay.Team1Odds, this);

            Debug.Log("Team 1 Selected: " + team1Selected);
        }
        else
        {
            team2Selected = true;
            team1Selected = false;

            ToggleTeam2.PlayUISound();

            ToggleTeam1.SetDisableColor();
            ToggleTeam2.SetToggledColor();

            OddsTextTeam1.color = Color.white;
            OddsTextTeam2.color = Color.white;

            teamIndex = 2;
            parlayHandler.AddToCalculateOdds(currentParlay.Team2Odds, this, currentParlay.Team2Name);
            cardManager.SpawnCard(currentParlay.Team2Name, currentParlay.Team2Odds, this);

            Debug.Log("Team 2 Selected: " + team2Selected);
        }

        UpdateUI();
    }

    private void OnTeamDeselected(int team)
    {
        if (isResetting) return;
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];

        if (team == 1)
        {
            team1Selected = false;
            teamIndex = 0;

            OddsTextTeam1.color = TextColorNormal;
            OddsTextTeam2.color = TextColorNormal;

            parlayHandler.RemoveFromCalculateOdds(currentParlay.Team1Odds, this);
            cardManager.RemoveCard(this);
            ToggleTeam1.SetNormalColor();
            ToggleTeam2.SetNormalColor();
            Debug.Log("Team 1 Deselected");
        }
        else
        {
            team2Selected = false;
            teamIndex = 0;

            OddsTextTeam1.color = TextColorNormal;
            OddsTextTeam2.color = TextColorNormal;

            parlayHandler.RemoveFromCalculateOdds(currentParlay.Team2Odds, this);
            cardManager.RemoveCard(this);
            ToggleTeam1.SetNormalColor();
            ToggleTeam2.SetNormalColor();
            Debug.Log("Team 2 Deselected");
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];

        if (OddsTextTeam1 != null)
            OddsTextTeam1.text = currentParlay.Team1Odds > 0 ? "+" + currentParlay.Team1Odds : currentParlay.Team1Odds.ToString();

        if (OddsTextTeam2 != null)
            OddsTextTeam2.text = currentParlay.Team2Odds > 0 ? "+" + currentParlay.Team2Odds : currentParlay.Team2Odds.ToString();

        if (Team1Text != null)
            Team1Text.text = currentParlay.Team1Name;

        if (Team2Text != null)
            Team2Text.text = currentParlay.Team2Name;
    }

    public void ResetToggle()
    {
        isResetting = true;

        team1Selected = false;
        team2Selected = false;
        teamIndex = 0;

        OddsTextTeam1.color = TextColorNormal;
        OddsTextTeam2.color = TextColorNormal;

        if (ToggleTeam1 != null)
            ToggleTeam1.ForceReset();
        if (ToggleTeam2 != null)
            ToggleTeam2.ForceReset();

        ToggleTeam1.SetNormalColor();
        ToggleTeam2.SetNormalColor();

        isResetting = false;

        UpdateUI();

        if(!tutorialFlag)
        {
           ParlayTutorialDisableButtons();
        }
    }


    public int GetSelectedOdds()
    {
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];

        if (teamIndex == 1) return currentParlay.Team1Odds;
        if (teamIndex == 2) return currentParlay.Team2Odds;

        return 0;
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
                if (ToggleTeam2 != null) ToggleTeam2.SetNormalColor();
            }
            else
            {
                if (ToggleTeam1 != null) ToggleTeam1.SetNormalColor();
            }

            OddsTextTeam1.color = Color.white;
            OddsTextTeam2.color = Color.white;
        }
        else
        {
            if (ToggleTeam1 != null) ToggleTeam1.SetNormalColor();
            if (ToggleTeam2 != null) ToggleTeam2.SetNormalColor();
        }

        UpdateUI();
    }

    private void OnStatsSelected()
    {
        if (isResetting) return;
        if(Stats.activeSelf) return;
        ToggleStats.PlayUISound();
        Stats.SetActive(true);
        UpdateStatsUI();
        parlayHandler.TurnOffSelectStats(this);
    }

    public void OnStatsDeselected()
    {
        if (isResetting) return;
        if (!Stats.activeSelf) return;
        ToggleStats.ForceReset();
        Stats.SetActive(false);
    }

    private void UpdateStatsUI()
    {
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];

        if (WinLossTeam1 != null)
            SetTeamsWinLoss(currentParlay.Team1WinLoss, true);
        if (WinLossTeam2 != null)
            SetTeamsWinLoss(currentParlay.Team2WinLoss, false);

        if (Team1Health != null)
        {
            SetTeamHealth(Team1Health, currentParlay.Team1Health);
            SetTeamHealth(Team2Health, currentParlay.Team2Health);   
        }

        if(Team1Abbr != null)
        {
            string TempTeam1Abbrev = GetTeamAbbreviation(currentParlay.Team1Name);
            string TempTeam2Abbrev = GetTeamAbbreviation(currentParlay.Team2Name);

            Team1Abbr.text = TempTeam1Abbrev;
            Team2Abbr.text = TempTeam2Abbrev;
        }
    }

    private void SetTeamHealth(GameObject[] healthIcons, TeamHealth health)
    {
        if (healthIcons == null) return;

        for (int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i].SetActive(i == (int)health);
        }
    }

    private void SetTeamsWinLoss(string winLoss, bool isTeam1)
    {
        TextMeshPro tmp = isTeam1 ? WinLossTeam1 : WinLossTeam2;
        if (tmp == null) return;

        tmp.richText = true;

        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < winLoss.Length; i++)
        {
            char c = winLoss[i];

            if (c == 'W')
                sb.Append("<color=#00FF00>W</color>");
            else if (c == 'L')
                sb.Append("<color=#FF0000>L</color>");
            else
                sb.Append(c);

            if (i < winLoss.Length - 1)
                sb.Append("-");
        }

        tmp.text = sb.ToString();

        tmp.ForceMeshUpdate();
    }

    private static readonly Dictionary<string, string> TeamAbbreviations =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "Team A", "A" },
        { "Team B", "B" },
        { "Team C", "C" },
        { "Team D", "D" },
  
        { "Team E", "E" },
        { "Team F", "F" },
        { "Team G", "G" },
        { "Team H", "H" },

        { "Team I", "I" },
        { "Team J", "J" },
        { "Team K", "K" },
        { "Team L", "L" },
        
        { "Team M", "M" },
        { "Team N", "N" },
        { "Team O", "O" },
        { "Team P", "P" },

        { "Team Q", "Q" },
        { "Team R", "R" },
        { "Team S", "S" },
        { "Team T", "T" },

        { "Team U", "U" },
        { "Team V", "V" },
        { "Team W", "W" },
        { "Team X", "X" },

        { "Team Y", "Y" },
        { "Team Z", "Z" },
    };


    private static string GetTeamAbbreviation(string teamName)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return string.Empty;

        teamName = teamName.Trim();

        if (TeamAbbreviations.TryGetValue(teamName, out string abbreviation))
            return abbreviation;

        return teamName.Length >= 3
            ? teamName.Substring(0, 3).ToUpper()
            : teamName.ToUpper();
    }


    public void AtStartDisableButtons(float disableTime)
    {
        StartCoroutine(TemporarilyDisableButtons(disableTime));
    }

    private IEnumerator TemporarilyDisableButtons(float disableTime)
    {
        yield return null;
        ToggleTeam1.DisableInteraction();
        ToggleTeam2.DisableInteraction();
        yield return new WaitForSeconds(disableTime);
        ToggleTeam1.EnableInteraction();
        ToggleTeam2.EnableInteraction();
    }

    private void ParlayTutorialDisableButtons()
    {   
        ToggleTeam1.SetDisableColor();
        ToggleTeam2.SetDisableColor();
        ToggleStats.DisableInteraction();
        tutorialFlag = true;
    }

    public void ParlayTutorialEnableButtons() // called by tutorial manager
    {
        ToggleTeam1.EnableInteraction();
        ToggleTeam2.EnableInteraction();
        ToggleStats.EnableInteraction();
    }

    public void enableStatsButton()
    {
        ToggleStats.EnableInteraction();
    }

    public void TeachParlay()
    {
        ToggleTeam1.StartPulsing(Color.green);
        ToggleTeam2.StartPulsing(Color.green);
    }

    public void StopTeachingParlay()
    {
        if (team1Selected)
        {
            ToggleTeam1.StopPulsing();
            ToggleTeam1.SetToggledColor();
            ToggleTeam2.StopPulsing();
            ToggleTeam2.SetDisableColor();
        }
        else if (team2Selected)
        {
            ToggleTeam2.StopPulsing();
            ToggleTeam2.SetToggledColor();
            ToggleTeam1.StopPulsing();
            ToggleTeam1.SetDisableColor();
        }
        else
        {
            ToggleTeam1.StopPulsing();
            ToggleTeam1.SetNormalColor();
            ToggleTeam2.StopPulsing();
            ToggleTeam2.SetNormalColor();
        }
        pulse.Setactive(true);
    }



}