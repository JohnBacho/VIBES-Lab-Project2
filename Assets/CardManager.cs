using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject CardPrefab;
    public Transform CardParent;
    private List<GameObject> activeCards = new List<GameObject>();
    private Dictionary<TogglePressInteractable, GameObject> cardMap = new Dictionary<TogglePressInteractable, GameObject>();
    
    private GridLayoutGroup gridLayout;
    private ContentSizeFitter contentSizeFitter;

    void Awake()
    {
        // Get or add GridLayoutGroup
        gridLayout = CardParent.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = CardParent.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        // Get or add ContentSizeFitter
        contentSizeFitter = CardParent.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter == null)
        {
            contentSizeFitter = CardParent.gameObject.AddComponent<ContentSizeFitter>();
        }
        
        // Configure ContentSizeFitter
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Initial grid setup
        ConfigureGridLayout();
    }

    private void ConfigureGridLayout()
    {
        int cardCount = activeCards.Count;
        
        gridLayout.spacing = new Vector2(-50, -5);
        gridLayout.padding = new RectOffset(-2, -2, -2, -2);
        
        gridLayout.cellSize = new Vector2(150, 30);
        
        if (cardCount <= 3)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 1;
            gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
        }
        else
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 2;
            gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
        }
        
        gridLayout.childAlignment = TextAnchor.UpperCenter;
    }

    public void SpawnCard(string team1, int odds, TogglePressInteractable toggle)
    {
        GameObject newCard = Instantiate(CardPrefab, CardParent);
        newCard.transform.SetAsLastSibling();

        activeCards.Add(newCard);

        TextMeshPro oddsText = newCard.transform.Find("Odds").GetComponent<TextMeshPro>();
        TextMeshPro team1Text = newCard.transform.Find("Team1").GetComponent<TextMeshPro>();
        
        if (odds > 0)
            oddsText.text = "+" + odds.ToString();
        else
        oddsText.text = odds.ToString();

        team1Text.text = team1;

        RemoveCard removeCardScript = newCard.GetComponent<RemoveCard>();
        if (removeCardScript != null)
        {
            removeCardScript.Initialize(toggle, this);
        }

        cardMap[toggle] = newCard;
        
        ConfigureGridLayout();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(CardParent as RectTransform);
    }

    public void RemoveCard(TogglePressInteractable toggle)
    {
        if (cardMap.TryGetValue(toggle, out GameObject card))
        {
            activeCards.Remove(card);
            cardMap.Remove(toggle);

            if (toggle != null)
            {
                int selectedOdds = toggle.GetSelectedOdds();
                toggle.betManager.RemoveFromCalculateOdds(selectedOdds);
                toggle.SetPressed(false);
            }

            Destroy(card);
            
            ConfigureGridLayout();
            LayoutRebuilder.ForceRebuildLayoutImmediate(CardParent as RectTransform);
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

                int selectedOdds = toggle.GetSelectedOdds();
                toggle.betManager.RemoveFromCalculateOdds(selectedOdds);
                toggle.SetPressed(true);
                Destroy(card);
                cardMap.Remove(toggle);
            }
        }
        activeCards.Clear();
        cardMap.Clear();
        
        // Reset grid layout
        ConfigureGridLayout();
    }

public void AnimateCardsColor(List<int> colorValues)
{
    StartCoroutine(AnimateCardsSequentially(colorValues));
}

    private IEnumerator AnimateCardsSequentially(List<int> colorValues)
    {
        if (colorValues.Count != activeCards.Count)
        {
            Debug.LogWarning($"Number of colors doesn't match number of cards {colorValues.Count} vs {activeCards.Count}");
            yield break;
        }

        for (int i = 0; i < activeCards.Count; i++)
        {
            GameObject card = activeCards[i];
            if (card != null)
            {
                bool isGreen = colorValues[i] == 1;
                StartCoroutine(AnimateSingleCardColorWithDelay(card, isGreen, i * 0.3f)); // 0.2s delay between cards
            }
        }

    }

    private IEnumerator AnimateSingleCardColorWithDelay(GameObject card, bool isGreen, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return AnimateSingleCardColor(card, isGreen);
    }

    public IEnumerator AnimateSingleCardColor(GameObject card, bool isGreen)
    {
        Color targetColor = isGreen ? Color.green : Color.red;
        Renderer cardRenderer = card.GetComponent<Renderer>();
        Image cardImage = card.GetComponent<Image>();
        if (cardImage == null) yield break;


        Transform cardTransform = card.transform;
        Vector3 originalScale = cardTransform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cardTransform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cardTransform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t);
            yield return null;
        }

        int flashes = 20;
        float flashSpeed = 0.05f;
        for (int i = 0; i < flashes; i++)
        {
            cardImage.color = (i % 2 == 0) ? Color.red : Color.green;
            yield return new WaitForSeconds(flashSpeed);
        }

        cardImage.color = targetColor;

    }

    public void AnimateCardsExceptUnrevealed(List<int> legWins)
    {
        StartCoroutine(FlashAllHits(legWins));
    }

    private IEnumerator FlashAllHits(List<int> legWins)
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            if (legWins[i] == 1)
            {
                StartCoroutine(AnimateSingleCardColor(activeCards[i], true));
            }
            else
            {
                // unrevealed leg (0 so far) stays neutral color
                Image cardImage = activeCards[i].GetComponent<Image>();
                if (cardImage != null) cardImage.color = Color.white;
            }
        }
        yield break;
    }

    // Called after player decides to Cash Out or Stay In
    public void RevealUnrevealedCards(List<int> legWins)
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            if (legWins[i] == 0)
            {
                StartCoroutine(AnimateSingleCardColor(activeCards[i], false));
            }
        }
    }


}