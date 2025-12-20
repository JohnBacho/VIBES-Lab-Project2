using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshPro leaderboardText;

    // List of players with name, score, and money
    private List<PlayerData> players = new List<PlayerData>
    {
        new PlayerData("Thomas", 115.25f),
        new PlayerData("Lauren", 620.10f),
        new PlayerData("Alexa", 1100.00f),
        new PlayerData("Olivia", 250.20f),
        new PlayerData("Corey", 185.20f),
        new PlayerData("Josh", -550.20f),
        new PlayerData("You", 100.00f)
    };

    void Start()
    {
        UpdateLeaderboard();
    }

    public void UpdateLeaderboard()
    {
        players.Sort((a, b) => b.money.CompareTo(a.money));

        string display = "Leaderboard\n\n";
        foreach (var player in players)
        {
            if (player.name == "You")
            {
                display += $"<color=green><b>{player.name}</b>  |   ${player.money:F2}</color>\n";
            }
            else
            {
                display += $"{player.name}  |   ${player.money:F2}\n";
            }
        }

        leaderboardText.text = display;
    }


    public void SetMoney(string playerName, float amount)
    {
        foreach (var player in players)
        {
            if (player.name == playerName)
            {
                player.money = amount;
                break;
            }
        }

        UpdateLeaderboard();
    }
}

[System.Serializable]
public class PlayerData
{
    public string name;
    public float money;

    public PlayerData(string name, float money)
    {
        this.name = name;
        this.money = money;
    }
}
