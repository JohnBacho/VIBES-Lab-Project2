using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParlayTutorial : MonoBehaviour
{
    [SerializeField] private GameObject SelectParlayTutorial;
    [SerializeField] private GameObject StatTutorial;

    [Header("Beacon Settings")]
    [SerializeField] private Color idleColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color grabColor = Color.green;
    [SerializeField] private List<TogglePressInteractable> togglePressInteractables;

    private Renderer beaconRenderer;
    private Coroutine pulseCoroutine;
    [SerializeField] private Material StatsMaterial;
    private Color originalStatsColor;

    private enum TutorialState
    {
        Disabled,
        Stats,
        SelectParlay,
        Completed
    }
    private TutorialState currentState = TutorialState.Disabled;


    private void Start()
    {
        if(StatsMaterial != null)
        {
            originalStatsColor = StatsMaterial.color;
        }
    }

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
            case TutorialState.Stats:
                StatTutorial?.SetActive(false);
                StopPulsing();
                StatsMaterial.color = originalStatsColor;
                break;
            case TutorialState.SelectParlay:
                SelectParlayTutorial?.SetActive(false);
                foreach (var interactable in togglePressInteractables)
                    interactable.StopTeachingParlay();
                break;
        }
    }

    private void EnterState(TutorialState state)
    {
        switch (state)
        {       
            case TutorialState.Stats:
                StatTutorial?.SetActive(true);
                StartPulsing(StatsMaterial, 2f);
                break;
                
            case TutorialState.SelectParlay:
                for(int i = 0; i < togglePressInteractables.Count; i++)
                {
                    togglePressInteractables[i].ParlayTutorialEnableButtons();
                }
                SelectParlayTutorial?.SetActive(true);
                foreach (var interactable in togglePressInteractables)
                    interactable.TeachParlay();
                break;
                
            case TutorialState.Completed:
                break;
        }
    }

    private void StartPulsing(Material material, float speed)
    {
        StopPulsing();
        pulseCoroutine = StartCoroutine(PulseColor(grabColor, material, speed));
    }

    private IEnumerator PulseColor(Color targetColor, Material Objectmaterial, float pulseSpeed)
    {
        if (Objectmaterial == null)
        {
            yield break;
        }

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;
            float lerp = Mathf.PingPong(t, 1f);
            Objectmaterial.color = Color.Lerp(idleColor, targetColor, lerp);
            yield return null;
        }
    }


    private void StopPulsing()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }

    public void HideSelectParlayTutorial()
    {
        if (SelectParlayTutorial != null)
            SelectParlayTutorial.SetActive(false);

        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].StopTeachingParlay();
        }
        SetTutorialState(TutorialState.Completed);
    }

    public void HideStatTutorial()
    {
        SetTutorialState(TutorialState.SelectParlay);
    }

    public void StartTutorial()
    {
        SetTutorialState(TutorialState.Stats);
    }

    public void turnOffTutorials()
    {
        SetTutorialState(TutorialState.Completed);
    }
}