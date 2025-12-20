using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCard : MonoBehaviour
{
    private TogglePressInteractable associatedToggle;
    private CardManager cardManager;
    
    // Call this when spawning the card to set up the reference
    public void Initialize(TogglePressInteractable toggle, CardManager manager)
    {
        associatedToggle = toggle;
        cardManager = manager;
    }
    
    public void OnClickRemoveCard()
    {
        // Tell the CardManager to remove this card properly
        if (cardManager != null && associatedToggle != null)
        {
            cardManager.RemoveCard(associatedToggle);
        }
        else
        {
            // Fallback if references aren't set
            Destroy(gameObject);
        }
    }
}