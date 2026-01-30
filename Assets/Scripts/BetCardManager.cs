using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BetCardManager : MonoBehaviour
{
    [System.Serializable]
    public class BetCard
    {
        public string matchup;
        public int odds;
        public GameObject uiObject;
    }

    [SerializeField] private Transform cardParent; // Assign your main Canvas panel here
    [SerializeField] private TMP_FontAsset font;   // Optional: assign a TMP font in the inspector

    private List<BetCard> activeBets = new List<BetCard>();

    public void AddBet(string matchup, int odds)
    {
        // Prevent duplicates
        if (activeBets.Exists(b => b.matchup == matchup)) return;

        // Create a new GameObject for the card
        GameObject newCard = new GameObject(matchup);
        newCard.transform.SetParent(cardParent, false);

        // Add TextMeshProUGUI component
        var text = newCard.AddComponent<TextMeshProUGUI>();
        text.text = $"{matchup}  ({odds})";
        text.fontSize = 32;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        if (font != null) text.font = font;

        // Add a LayoutElement so VerticalLayoutGroup can handle spacing
        var layout = newCard.AddComponent<UnityEngine.UI.LayoutElement>();
        layout.preferredHeight = 60;

        // Add to list (keeps order of selection)
        activeBets.Add(new BetCard { matchup = matchup, odds = odds, uiObject = newCard });
    }

    public void RemoveBet(string matchup)
    {
        BetCard found = activeBets.Find(b => b.matchup == matchup);
        if (found != null)
        {
            Destroy(found.uiObject);
            activeBets.Remove(found);
        }
    }
}
