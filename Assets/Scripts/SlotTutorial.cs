using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTutorial : MonoBehaviour
{
    [SerializeField] private GameObject IncreaseBetTutorial;
    [SerializeField] private GameObject GrabHandleTutorial;

    private enum TutorialState
    {
        Disabled,
        IncreaseBetTutorial,
        GrabHandleTutorial,
        Completed
    }
    private TutorialState currentState = TutorialState.IncreaseBetTutorial;




    private void SetTutorialState(TutorialState newState)
    {
        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

        private void ExitState(TutorialState state)
    {
        switch (state)
        {
            case TutorialState.IncreaseBetTutorial:
                IncreaseBetTutorial?.SetActive(false);
                break;
            case TutorialState.GrabHandleTutorial:
                GrabHandleTutorial?.SetActive(false);
                break;
            case TutorialState.Completed:
                break;
        }
    }

    private void EnterState(TutorialState state)
    {
        switch (state)
        {
            case TutorialState.IncreaseBetTutorial:
                IncreaseBetTutorial?.SetActive(true);
                break;
                
            case TutorialState.GrabHandleTutorial:
                GrabHandleTutorial?.SetActive(true);
                break;
            case TutorialState.Completed:
                break;
        }
    }

    public void ShowGrabHandleTutorial()
    {
        SetTutorialState(TutorialState.GrabHandleTutorial);
    }
    public void HideGrabHandleTutorial()
    {
        SetTutorialState(TutorialState.Completed);
    }
}
