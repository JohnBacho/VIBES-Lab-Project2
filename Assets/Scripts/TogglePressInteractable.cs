using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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

}

public class TogglePressInteractable : MonoBehaviour
{
    public ParlayManager parlayManager;

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

    public AudioSource audioSource;


    private bool team1Selected = false;
    private bool team2Selected = false;
    public int teamIndex;
    private Color TextColorNormal = new Color32(0x1F, 0x37, 0x5B, 0xFF);


    void Start()
    {
        ButtonTeam1.onClick.AddListener(() => OnTeamSelected(1));
        ButtonTeam2.onClick.AddListener(() => OnTeamSelected(2));
        UpdateUI();
    }

    private void OnTeamSelected(int team)
    {
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];
        audioSource.Play();
        
        if (team == 1)
        {
            team1Selected = !team1Selected;

            if (team1Selected)
            {
                DisableButton(ButtonTeam2);
                EnableButton(ButtonTeam1);
                Debug.Log("Team 1 Selected: " + team1Selected);
                OddsTextTeam1.color = Color.white;
                OddsTextTeam2.color = Color.white;

                teamIndex = 1;
                betManager.AddToCalculateOdds(currentParlay.Team1Odds, this, currentParlay.Team1Name);
                cardManager.SpawnCard(currentParlay.Team1Name, currentParlay.Team1Odds, this);
            }
            else
            {
                EnableButton(ButtonTeam2);
                OddsTextTeam1.color = TextColorNormal;
                OddsTextTeam2.color = TextColorNormal;
                Debug.Log("Team 1 Deselected: " + team1Selected);
                betManager.RemoveFromCalculateOdds(currentParlay.Team1Odds, this);
                cardManager.RemoveCard(this);
                teamIndex = 0;
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
                OddsTextTeam1.color = Color.white;
                OddsTextTeam2.color = Color.white;

                teamIndex = 2;
                betManager.AddToCalculateOdds(currentParlay.Team2Odds, this, currentParlay.Team2Name);
                cardManager.SpawnCard(currentParlay.Team2Name, currentParlay.Team2Odds, this);
            }
            else
            {
                EnableButton(ButtonTeam1);
                OddsTextTeam1.color = TextColorNormal;
                OddsTextTeam2.color = TextColorNormal;
                Debug.Log("Team 2 Deselected: " + team2Selected);
                betManager.RemoveFromCalculateOdds(currentParlay.Team2Odds, this);
                cardManager.RemoveCard(this);
                teamIndex = 0;
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
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];


        if (OddsTextTeam1 != null)
            if(currentParlay.Team1Odds > 0)
            {
                OddsTextTeam1.text = "+" + currentParlay.Team1Odds.ToString();
            }
            else
            {
            OddsTextTeam1.text = currentParlay.Team1Odds.ToString();
            }
        if (OddsTextTeam2 != null)
            if(currentParlay.Team2Odds > 0)
            {
                OddsTextTeam2.text = "+" + currentParlay.Team2Odds.ToString();
            }
            else
            {
            OddsTextTeam2.text = currentParlay.Team2Odds.ToString();
            }
        if (Team1Text != null)
            Team1Text.text = currentParlay.Team1Name;

        if (Team2Text != null)
            Team2Text.text = currentParlay.Team2Name;
    }

    public void ResetToggle()
    {
        team1Selected = false;
        team2Selected = false;
        teamIndex = 0;
        EnableButton(ButtonTeam1);
        EnableButton(ButtonTeam2);
        OddsTextTeam1.color = TextColorNormal;
        OddsTextTeam2.color = TextColorNormal;
        UpdateUI();
    }

    public int GetSelectedOdds()
    {
        Parlay currentParlay = parlayManager.Parlays[sxr.GetTrial()];

        return teamIndex == 1 ? currentParlay.Team1Odds : currentParlay.Team1Odds;
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
            OddsTextTeam1.color = Color.white;
            OddsTextTeam2.color = Color.white;

        }
        else
        {
            ButtonTeam1.interactable = true;
            ButtonTeam2.interactable = true;
        }

        UpdateUI();
    }
}