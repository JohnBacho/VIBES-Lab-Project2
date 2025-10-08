using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshPro leaderboardText;

    // List of players with name, score, and money
    private List<PlayerData> players = new List<PlayerData>
    {
        new PlayerData("Thomas", 500.25f),
        new PlayerData("Lauren", 620.10f),
        new PlayerData("Alexa", 110.00f),
        new PlayerData("You", 100.00f)
    };

    void Start()
    {
        UpdateLeaderboard();
    }

    public void UpdateLeaderboard()
    {
        // Sort by score (or by money if you prefer)
        players.Sort((a, b) => b.money.CompareTo(a.money));

        string display = "Leaderboard\n\n";
        foreach (var player in players)
        {
            display += $"{player.name}  |   ${player.money:F2}\n";
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
