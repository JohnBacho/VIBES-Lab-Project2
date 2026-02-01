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
        IncreaseBetTutorial.SetActive(false);
    }
    public void ShowGrabHandleTutorial()
    {
        GrabHandleTutorial.SetActive(true);
    }
    public void HideGrabHandleTutorial()
    {
        GrabHandleTutorial.SetActive(false);
    }
}
