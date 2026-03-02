using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ParlayInstructions : MonoBehaviour
{
    [SerializeField] private GameObject Pt1InstructionsPanel;
    [SerializeField] private GameObject Pt2InstructionsPanel;
    [SerializeField] private GameObject Pt3InstructionsPanel;


    public IEnumerator ShowParlayInstructions()
    {
        Pt1InstructionsPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        Pt1InstructionsPanel.SetActive(false);
        yield return Pt2(); 
    }

    private IEnumerator Pt2()
    {
        Pt2InstructionsPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        Pt2InstructionsPanel.SetActive(false);
        yield return Pt3(); 
    }
    private IEnumerator Pt3()
    {
        Pt3InstructionsPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil(() => sxr.GetTrigger());
        Pt3InstructionsPanel.SetActive(false);
    }
}
