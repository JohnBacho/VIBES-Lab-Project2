using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParlayTutorial : MonoBehaviour
{
    [SerializeField] private GameObject SelectParlayTutorial;
    [SerializeField] private GameObject pt1GrabHandleTutorial;
    [SerializeField] private GameObject pt2GrabHandleTutorial;
    [SerializeField] private GameObject StatTutorial;

    [Header("Beacon Settings")]
    [SerializeField] private GameObject BeaconBox;
    [SerializeField] private Color idleColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color grabColor = Color.green;
    [SerializeField] private List<TogglePressInteractable> togglePressInteractables;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;

    private Renderer beaconRenderer;
    private Material beaconMaterial;
    private bool isHandInside = false;
    private bool hasHiddenStatTutorial = false;
    private Coroutine pulseCoroutine;
    [SerializeField] private Material StatsMaterial;
    private Color originalStatsColor;

    private enum TutorialState
    {
        Disabled,
        GrabHandlePt1,
        GrabHandlePt2,
        Stats,
        SelectParlay,
        Completed
    }
    private TutorialState currentState = TutorialState.Disabled;


    private void Start()
    {
        if (BeaconBox != null)
        {
            beaconRenderer = BeaconBox.GetComponent<Renderer>();
            
            if (beaconRenderer != null)
            {
                beaconMaterial = beaconRenderer.material;
                beaconMaterial.color = idleColor;
            }
            if(StatsMaterial != null)
            {
                originalStatsColor = StatsMaterial.color;
            }
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
            case TutorialState.GrabHandlePt1:
                pt1GrabHandleTutorial?.SetActive(false);
                break;
            case TutorialState.GrabHandlePt2:
                pt2GrabHandleTutorial?.SetActive(false);
                break;
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
            case TutorialState.GrabHandlePt1:
                pt1GrabHandleTutorial?.SetActive(true);
                ToggleControllerColliders(true);
                BeaconBox.SetActive(true);
                break;
                
            case TutorialState.GrabHandlePt2:
                pt2GrabHandleTutorial?.SetActive(true);
                StartPulsing(beaconMaterial, 1f);
                break;
                
            case TutorialState.Stats:
                for(int i = 0; i < togglePressInteractables.Count; i++)
                {
                    togglePressInteractables[i].enableStatsButton();
                }
                StatTutorial?.SetActive(true);
                ToggleControllerColliders(false);
                BeaconBox.SetActive(false);
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

public void OnControllerEnter(Collider other)
{
    if (!IsVRController(other) || TutorialState.Completed == currentState)
    {
        return;
    }

    isHandInside = true;
    if (BeaconBox.activeSelf)
    {
            SetTutorialState(TutorialState.GrabHandlePt2);
    }

    if (pulseCoroutine != null)
        StopCoroutine(pulseCoroutine);

    pulseCoroutine = StartCoroutine(PulseColor(grabColor, beaconMaterial, 1f));
}

    public void OnControllerExit(Collider other)
    {
        if (!IsVRController(other) || TutorialState.Completed == currentState)
        {
            return;
        }
            
        isHandInside = false;
        if (BeaconBox.activeSelf)
        {
            SetTutorialState(TutorialState.GrabHandlePt1);
        }

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        if (beaconMaterial != null)
            beaconMaterial.color = idleColor;
    }

    private bool IsVRController(Collider other)
    {
        string objName = other.gameObject.name.ToLower();
        if (objName.Contains("controller"))
        {
            return true;
        }

        return false;
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

    public void HideGrabHandleTutorial()
    {
        SetTutorialState(TutorialState.Stats);
    }

    public void HideStatTutorial()
    {
        SetTutorialState(TutorialState.SelectParlay);
    }

    public void StartTutorial()
    {
        SetTutorialState(TutorialState.GrabHandlePt1);
    }

    private void ToggleControllerColliders(bool enable)
    {
        leftController.GetComponent<Collider>().enabled = enable;
        rightController.GetComponent<Collider>().enabled = enable;
    }

    public void turnOffTutorials()
    {
        SetTutorialState(TutorialState.Completed);
    }
}