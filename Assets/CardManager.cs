using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public GameObject CardPrefab; // assign your card prefab
    public Transform CardParent; // usually a Vertical Layout Group
    private List<GameObject> activeCards = new List<GameObject>();
    private Dictionary<TogglePressInteractable, GameObject> cardMap = new Dictionary<TogglePressInteractable, GameObject>();

public void SpawnCard(string team1, string team2, int odds, TogglePressInteractable toggle)
{
    GameObject newCard = Instantiate(CardPrefab, CardParent);
    newCard.transform.SetAsLastSibling();
    
    TextMeshPro oddsText = newCard.transform.Find("Odds").GetComponent<TextMeshPro>();
    TextMeshPro team1Text = newCard.transform.Find("Team1").GetComponent<TextMeshPro>();
    TextMeshPro team2Text = newCard.transform.Find("Team2").GetComponent<TextMeshPro>();
    
    oddsText.text = odds.ToString();
    team1Text.text = team1;
    team2Text.text = team2;
    
    RemoveCard removeCardScript = newCard.GetComponent<RemoveCard>();
    if (removeCardScript != null)
    {
        removeCardScript.Initialize(toggle, this);
    }
    
    cardMap[toggle] = newCard;
}

    public void RemoveCard(TogglePressInteractable toggle)
    {
        if (cardMap.TryGetValue(toggle, out GameObject card))
        {
            activeCards.Remove(card);
            cardMap.Remove(toggle);

            if (toggle != null)
            {
                int selectedOdds = toggle.ListOfOdds[sxr.GetTrial()];
                toggle.betManager.RemoveFromCalculateOdds(selectedOdds);
                toggle.SetPressed(false);
            }

            Destroy(card);
        }
    }
    public void RemoveAllCards()
    {
        var toggles = new List<TogglePressInteractable>(cardMap.Keys);

        foreach (var toggle in toggles)
        {
            if (toggle == null)
                continue;

            if (cardMap.TryGetValue(toggle, out GameObject card))
            {
                activeCards.Remove(card);

                int selectedOdds = toggle.ListOfOdds[sxr.GetTrial()];
                toggle.betManager.RemoveFromCalculateOdds(selectedOdds);
                toggle.SetPressed(true);
                Destroy(card);
                cardMap.Remove(toggle);
            }
        }
        activeCards.Clear();
        cardMap.Clear();
    }

}
