using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTutorial : MonoBehaviour
{
    [SerializeField] private GameObject IncreaseBetTutorial;
    [SerializeField] private GameObject GrabHandleTutorial;

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
