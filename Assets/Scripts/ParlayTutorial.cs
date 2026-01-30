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
            return;

        if (SelectParlayTutorial != null)
            SelectParlayTutorial.SetActive(true);

        hasHiddenStatTutorial = true;
    }

    public void HideSelectParlayTutorial()
    {
        if (SelectParlayTutorial != null)
            SelectParlayTutorial.SetActive(false);
    }

    private void ShowGrabHandleTutorial()
    {
        if (GrabHandleTutorial != null)
            GrabHandleTutorial.SetActive(true);
    }

    public void HideGrabHandleTutorial()
    {
        if (GrabHandleTutorial != null)
            GrabHandleTutorial.SetActive(false);

        ShowStatTutorial();
    }

    private void ShowStatTutorial()
    {
        if (StatTutorial != null)
            StatTutorial.SetActive(true);
    }

    public void HideStatTutorial()
    {
        if (StatTutorial != null)
            StatTutorial.SetActive(false);

        ShowSelectParlayTutorial();
    }
}
