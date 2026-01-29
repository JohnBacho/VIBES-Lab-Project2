using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Transform CardParent;
    private List<GameObject> activeCards = new List<GameObject>();
    private Dictionary<TogglePressInteractable, GameObject> cardMap = new Dictionary<TogglePressInteractable, GameObject>();
    
    private GridLayoutGroup gridLayout;
    private ContentSizeFitter contentSizeFitter;

    private static readonly float suspenseVolume = 0.3f;
    private static readonly float resultVolume = 0.6f;


    void Awake()
    {
        EnsureLayoutComponentsInitialized();
    }
    
    private void EnsureLayoutComponentsInitialized()
    {
        if (CardParent == null)
        {
            Debug.LogError("CardParent is not assigned in CardManager!");
            return;
        }
        
        if (!CardParent.gameObject.activeInHierarchy)
        {
            CardParent.gameObject.SetActive(true);
        }
        
        gridLayout = CardParent.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = CardParent.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        contentSizeFitter = CardParent.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter == null)
        {
            contentSizeFitter = CardParent.gameObject.AddComponent<ContentSizeFitter>();
        }
        
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        ConfigureGridLayout();
    }

    private void ConfigureGridLayout()
    {
        if (CardParent != null)
        {
            gridLayout = CardParent.GetComponent<GridLayoutGroup>();
        }
        
        if (gridLayout == null)
        {
            if (CardParent == null)
            {
                Debug.LogError("CardParent is null in ConfigureGridLayout!");
                return;
            }
            
            bool wasInactive = !CardParent.gameObject.activeSelf;
            if (wasInactive)
            {
                CardParent.gameObject.SetActive(true);
            }
            
            gridLayout = CardParent.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                gridLayout = CardParent.gameObject.AddComponent<GridLayoutGroup>();
                Debug.LogWarning("GridLayout was null, re-initializing...");
            }
        }
        
        int cardCount = activeCards.Count;
        
        gridLayout.spacing = new Vector2(-50, -5);
        gridLayout.padding = new RectOffset(-2, -2, -2, -2);
        
        // Adjusts the size of the cards based on the number of parlays selected by the player
        if (cardCount <= 4)
        {
            gridLayout.cellSize = new Vector2(150, 30);
        }
        else
        {
            // Does a slight scale down
            float scaleFactor = Mathf.Max(0.5f, 1f - ((cardCount - 4) * 0.1f));
            float widthMultiplier = 1f + (1f - scaleFactor) * 0.3f;
            gridLayout.cellSize = new Vector2(150 * scaleFactor * widthMultiplier, 30 * scaleFactor);
        }
        
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 1;
        gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
        
        gridLayout.childAlignment = TextAnchor.UpperCenter;
    }

    public void SpawnCard(string team1, int odds, TogglePressInteractable toggle)
    {
        if (CardPrefab == null || CardParent == null)
        {
            Debug.LogError("CardPrefab or CardParent is not assigned!");
            return;
        }
        
        EnsureLayoutComponentsInitialized();
        
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
        
        if (CardParent is RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
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
                toggle.parlayHandler.RemoveFromCalculateOdds(selectedOdds, toggle);
                toggle.SetPressed(false);
            }

            Destroy(card);
            
            ConfigureGridLayout();
            
            if (CardParent is RectTransform rectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
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
                toggle.parlayHandler.RemoveFromCalculateOdds(selectedOdds, toggle);
                toggle.SetPressed(true);
                Destroy(card);
                cardMap.Remove(toggle);
            }
        }
        activeCards.Clear();
        cardMap.Clear();
        
        ConfigureGridLayout();
    }

    public void AnimateCardsColor(List<bool> colorValues)
    {
        StartCoroutine(AnimateCardsSequentially(colorValues));
    }

    private IEnumerator AnimateCardsSequentially(List<bool> colorValues)
    {
        if (colorValues.Count != activeCards.Count)
        {
            Debug.LogWarning($"Number of colors doesn't match number of cards {colorValues.Count} vs {activeCards.Count}");
            yield break;
        }

        SoundManager.SoundManager.PlayLoop(SoundType.suspenseLoop, transform.position, suspenseVolume);

        for (int i = 0; i < activeCards.Count; i++)
        {
            GameObject card = activeCards[i];
            if (card != null)
            {
                bool isGreen = colorValues[i] == true;
                StartCoroutine(
                    AnimateSingleCardColorWithDelay(card, isGreen, i * 0.3f)
                );
            }
        }

        float totalTime =
            (activeCards.Count * 0.3f) + 
            (0.6f) + 
            (20 * 0.05f); 

        yield return new WaitForSeconds(totalTime);

        SoundManager.SoundManager.StopSound3D(SoundType.suspenseLoop);
    }


    private IEnumerator AnimateSingleCardColorWithDelay(GameObject card, bool isGreen, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return AnimateSingleCardColor(card, isGreen);
    }

    public IEnumerator AnimateSingleCardColor(GameObject card, bool isGreen)
    {
        Color targetColor = isGreen ? Color.green : Color.red;
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

        PlayResultSound(isGreen);
    }
    
    private void PlayResultSound(bool isGreen)
    {
        if (isGreen)
        {
            SoundManager.SoundManager.PlaySound3D(SoundType.successTone, transform.position, resultVolume);
        }
        else
        {
            SoundManager.SoundManager.PlaySound3D(SoundType.lossAudio, transform.position, resultVolume);
        }
    }
}