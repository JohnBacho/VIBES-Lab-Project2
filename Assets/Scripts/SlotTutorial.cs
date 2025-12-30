using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTutorial : MonoBehaviour
{
    public GameObject IncreaseBetTutorial;
    public GameObject GrabHandleTutorial;

    public void ShowIncreaseBetTutorial()
    {
        IncreaseBetTutorial.SetActive(true);
    }
    public void HideIncreaseBetTutorial()
    {
        Destroy(IncreaseBetTutorial, 0.25f);
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
