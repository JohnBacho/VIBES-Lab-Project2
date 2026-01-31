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

    private Renderer beaconRenderer;
    private Material beaconMaterial;
    private bool isHandInside = false;
    private bool hasHiddenStatTutorial = false;
    private Coroutine pulseCoroutine;
    public Material StatsMaterial;
    private Color originalStatsColor;


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
public void OnControllerEnter(Collider other)
{
    if (!IsVRController(other))
    {
        return;
    }

    isHandInside = true;
    if (BeaconBox.activeSelf)
    {
            ShowGrabHandleTutorialpt2();
    }

    if (pulseCoroutine != null)
        StopCoroutine(pulseCoroutine);

    pulseCoroutine = StartCoroutine(PulseColor(grabColor, beaconMaterial, 1f));
}

public void OnControllerExit(Collider other)
{
    if (!IsVRController(other))
        return;

    isHandInside = false;
    if (BeaconBox.activeSelf)
    {
        ShowGrabHandleTutorialpt1();
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

    private void ShowSelectParlayTutorial()
    {
        if (hasHiddenStatTutorial)
            return;

        if (SelectParlayTutorial != null)
            SelectParlayTutorial.SetActive(true);
        
        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].TeachParlay();
        }

        hasHiddenStatTutorial = true;
    }

    public void HideSelectParlayTutorial()
    {
        if (SelectParlayTutorial != null)
            SelectParlayTutorial.SetActive(false);

        for (int i = 0; i < togglePressInteractables.Count; i++)
        {
            togglePressInteractables[i].StopTeachingParlay();
        }
    }

    private void ShowGrabHandleTutorialpt2()
    {
        if (pt1GrabHandleTutorial != null)
            pt1GrabHandleTutorial.SetActive(false);
        if (pt2GrabHandleTutorial != null)
            pt2GrabHandleTutorial.SetActive(true);
    }

    public void HideGrabHandleTutorial()
    {
        if (pt2GrabHandleTutorial != null)
            pt2GrabHandleTutorial.SetActive(false);

        BeaconBox.SetActive(false);
        ShowStatTutorial();
    }

    private void ShowStatTutorial()
    {
        if (StatTutorial != null)
            StatTutorial.SetActive(true);
        
        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        pulseCoroutine = StartCoroutine(PulseColor(grabColor, StatsMaterial, 2f));
    }

    public void HideStatTutorial()
    {
        if (StatTutorial != null)
            StatTutorial.SetActive(false);

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);
        StatsMaterial.color = originalStatsColor;
        ShowSelectParlayTutorial();
    }
    private void ShowGrabHandleTutorialpt1()
    {
        if (pt1GrabHandleTutorial != null)
            pt1GrabHandleTutorial.SetActive(true);
        if (pt2GrabHandleTutorial != null)  
            pt2GrabHandleTutorial.SetActive(false);
    }
}