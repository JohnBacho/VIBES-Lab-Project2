using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ParlayInstructions : MonoBehaviour
{
    [SerializeField] private GameObject instructionSceneParlay;
    [SerializeField] private TextMeshPro textParlayInstructions;

    public IEnumerator ShowParlayInstructions()
    {
        instructionSceneParlay.SetActive(true);
        textParlayInstructions.text = "PlaceHolder";
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        yield return Pt2(); 
    }

    private IEnumerator Pt2()
    {
        textParlayInstructions.text = "pt2 PlaceHolder";
        yield return new WaitForSeconds(2f);
        instructionSceneParlay.SetActive(false);
    }
}
