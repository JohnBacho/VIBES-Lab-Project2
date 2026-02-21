using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class SwitchContextText : MonoBehaviour
{
    [SerializeField] private TextMeshPro ContextSwitchText;
    [SerializeField] private GameObject ContextSwitchScene;
    [SerializeField] private GameObject PressTriggerGraphic;

    const float TextTime = 0.30f;

    public IEnumerator StartContextSwitch(bool switchingToParlay)
    {
        ContextSwitchScene.SetActive(true);
        PressTriggerGraphic.SetActive(true);
        ContextSwitchText.text =
            "<b>Gambling Task #1 Completed</b>\n\n"+ 
            "The next task is <b>separate and independent</b>.\n\n" +
            "Any money from the previous task <b>will not carry over</b>.\n\n" +
            "Wallet reset to <color=green><b>$25</b></color>";
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        PressTriggerGraphic.SetActive(false);
        if (switchingToParlay)
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
        for (int i = 0; i < 2; i++)
        {
            ContextSwitchText.text = "Switching to Parlay!\nLoading";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = "Switching to Parlay!\nLoading.";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = "Switching to Parlay!\nLoading..";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = "Switching to Parlay!\nLoading...";
            yield return new WaitForSeconds(TextTime);
        }
        ContextSwitchScene.SetActive(false);
    }

    private IEnumerator SwitchingToSlotCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            ContextSwitchText.text = "Switching to Slot!\nLoading";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = "Switching to Slot!\nLoading.";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = "Switching to Slot!\nLoading..";
            yield return new WaitForSeconds(TextTime);
            ContextSwitchText.text = "Switching to Slot!\nLoading...";
            yield return new WaitForSeconds(TextTime);
        }
        ContextSwitchScene.SetActive(false);
    }
}
