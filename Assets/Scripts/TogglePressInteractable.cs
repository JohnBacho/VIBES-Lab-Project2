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
    public XRPokeToggleButton ToggleTeam1;
    public XRPokeToggleButton ToggleTeam2;
    public XRPokeToggleButton ToggleStats;


    [Header("UI Text")]
    public TextMeshPro OddsTextTeam1;
    public TextMeshPro OddsTextTeam2;
    public TextMeshPro Team1Text;
    public TextMeshPro Team2Text;
    [Header("Stats UI Text")]
    public TextMeshPro WinLossTeam1;
    public TextMeshPro WinLossTeam2;
    public TextMeshPro Team1Abbr;
    public TextMeshPro Team2Abbr;
    public GameObject Stats;
    public GameObject[] Team1Health = new GameObject[3];
    public GameObject[] Team2Health = new GameObject[3];


    [Header("Managers")]
    public ParlayHandler parlayHandler;
    public CardManager cardManager;

    public AudioSource audioSource;

    private bool team1Selected = false;
    private bool team2Selected = false;
    private int teamIndex;
    private Color TextColorNormal = new Color32(0x1F, 0x37, 0x5B, 0xFF);
    private bool isResetting = false;



    void Start()
    {
        // Wire XR poke toggle events
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
        audioSource.Play();

        if (team == 1)
        {
            team1Selected = true;
            team2Selected = false;

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

            // Disable other toggle
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
            // Both toggles off
            if (ToggleTeam1 != null) ToggleTeam1.SetNormalColor();
            if (ToggleTeam2 != null) ToggleTeam2.SetNormalColor();
        }

        UpdateUI();
    }

    private void OnStatsSelected()
    {
        if (isResetting) return;
        if(Stats.activeSelf) return;
        audioSource.Play();
        Stats.SetActive(true);
        parlayHandler.TurnOffSelectStats(this);
        UpdateStatsUI();
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
            SetTeamsWinLoss(currentParlay.Team1WinLoss.ToString(), true);
        if (WinLossTeam2 != null)
            SetTeamsWinLoss(currentParlay.Team2WinLoss.ToString(), false);

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
        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < winLoss.Length; i++)
        {
            char c = winLoss[i];

            if (c == 'W')
                sb.Append("<color=green>W</color>");
            else if (c == 'L')
                sb.Append("<color=red>L</color>");
            else
                sb.Append(c);

            if (i < winLoss.Length - 1)
                sb.Append("-");
        }

        if (isTeam1)
            WinLossTeam1.text = sb.ToString();
        else
            WinLossTeam2.text = sb.ToString();
    }


    private static readonly Dictionary<string, string> TeamAbbreviations =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // AFC North
        { "Browns", "CLE" },
        { "Ravens", "BAL" },
        { "Steelers", "PIT" },
        { "Bengals", "CIN" },

        // AFC East
        { "Jets", "NYJ" },
        { "Patriots", "NE" },
        { "Dolphins", "MIA" },
        { "Bills", "BUF" },

        // AFC West
        { "Chiefs", "KC" },
        { "Raiders", "LV" },
        { "Broncos", "DEN" },
        { "Chargers", "LAC" },
        
        // AFC South
        { "Texans", "HOU" },
        { "Titans", "TEN" },
        { "Colts", "IND" },
        { "Jaguars", "JAX" },

        // NFC North
        { "Packers", "GB" },
        { "Bears", "CHI" },
        { "Vikings", "MIN" },
        { "Lions", "DET" },

        // NFC East
        { "Cowboys", "DAL" },
        { "Giants", "NYG" },
        { "Eagles", "PHI" },
        { "Commanders", "WAS" },

        // NFC South
        { "Saints", "NO" },
        { "Falcons", "ATL" },
        { "Buccaneers", "TB" },
        { "Panthers", "CAR" },

        // NFC West
        { "49ers", "SF" },
        { "Seahawks", "SEA" },
        { "Rams", "LAR" },
        { "Cardinals", "ARI" },
    };


    public static string GetTeamAbbreviation(string teamName)
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


    public void AtStartDisableButtons()
    {
        StartCoroutine(TemporarilyDisableButtons());
    }

    private IEnumerator TemporarilyDisableButtons()
    {
        yield return null;
        ToggleTeam1.DisableInteraction();
        ToggleTeam2.DisableInteraction();
        const float buttonDisableTime = 2f;
        yield return new WaitForSeconds(buttonDisableTime);
        ToggleTeam1.EnableInteraction();
        ToggleTeam2.EnableInteraction();
    }



}
