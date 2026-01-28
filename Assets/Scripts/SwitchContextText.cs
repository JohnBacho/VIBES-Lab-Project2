using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class SwitchContextText : MonoBehaviour
{
    [SerializeField] private TextMeshPro ContextSwitchText;
    [SerializeField] private TextMeshPro WalletText;
    [SerializeField] private GameObject ContextSwitchScene;
    const float TextTime = 0.30f;
    
    public IEnumerator StartContextSwitch(bool switchingToParlay)
    {
        ContextSwitchScene.SetActive(true);
        if(switchingToParlay)
        {
            yield return StartCoroutine(SwitchingToParlayCoroutine());
        }
        else
        {
            yield return StartCoroutine(SwitchingToSlotCoroutine());
        }
    }
    
    private IEnumerator SwitchingToParlayCoroutine()
    {
        WalletText.text = $"Wallet reset to $100";
        for(int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Parlay!\nLoading";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Parlay!\nLoading.";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Parlay!\nLoading..";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Parlay!\nLoading...";
        }
        ContextSwitchScene.SetActive(false);
    }
    
    private IEnumerator SwitchingToSlotCoroutine()
    {
        WalletText.text = $"Wallet reset to $100";
        for(int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Slot!\nLoading";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Slot!\nLoading.";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Slot!\nLoading..";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = $"Switching to Slot!\nLoading...";
        }
        ContextSwitchScene.SetActive(false);
    }
}