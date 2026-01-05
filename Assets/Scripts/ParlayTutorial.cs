using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParlayTutorial : MonoBehaviour
{
    [SerializeField] private GameObject SelectParlayTutorial;
    [SerializeField] private GameObject GrabHandleTutorial;
    [SerializeField] private GameObject StatTutorial;
    private bool hasHiddenStatTutorial = false;


    private void ShowSelectParlayTutorial()
    {
        if (hasHiddenStatTutorial)
        {
            return;
        } 
        SelectParlayTutorial.SetActive(true);
        hasHiddenStatTutorial = true;
    }
    public void HideSelectParlayTutorial()
    {
        Destroy(SelectParlayTutorial,0.15f);
    }
    private void ShowGrabHandleTutorial()
    {
        GrabHandleTutorial.SetActive(true);
    }
    public void HideGrabHandleTutorial()
    {
        Destroy(GrabHandleTutorial,0.15f);
        ShowStatTutorial();
    }

    private void ShowStatTutorial()
    {
        StatTutorial.SetActive(true);
    }
    public void HideStatTutorial()
    {
        Destroy(StatTutorial,0.05f);
        ShowSelectParlayTutorial();
    }
}
