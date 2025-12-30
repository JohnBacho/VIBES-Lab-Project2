using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParlayTutorial : MonoBehaviour
{
    public GameObject SelectParlayTutorial;
    public GameObject GrabHandleTutorial;

    public void ShowSelectParlayTutorial()
    {
        SelectParlayTutorial.SetActive(true);
    }
    public void HideSelectParlayTutorial()
    {
        Destroy(SelectParlayTutorial,0.25f);
    }
    public void ShowGrabHandleTutorial()
    {
        GrabHandleTutorial.SetActive(true);
    }
    public void HideGrabHandleTutorial()
    {
        Destroy(GrabHandleTutorial,0.25f);
    }
}
