using System.Collections;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    [SerializeField] private Material statsMaterial;
    [SerializeField] private Color idleColor = new Color32(74, 144, 226, 255);
    [SerializeField] private Color pulseColor = new Color32(140, 180, 245, 255);
    [SerializeField] private float pulseSpeed = 1.5f;

    private Coroutine pulseRoutine;
    private bool activate = false;

    private void OnEnable()
    {
        if (pulseRoutine == null)
            pulseRoutine = StartCoroutine(PulseColor());
    }

    private void OnDisable()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        if (statsMaterial != null)
            statsMaterial.color = idleColor;
    }

    private IEnumerator PulseColor()
    {
        if (statsMaterial == null)
            yield break;
        if(!activate)
            yield break;

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;
            float lerp = Mathf.PingPong(t, 1f);
            statsMaterial.color = Color.Lerp(idleColor, pulseColor, lerp);
            yield return null;
        }
    }

    public void Setactive(bool active)
    {
        if (active)
        {
            activate = true;
            if (pulseRoutine == null)
                pulseRoutine = StartCoroutine(PulseColor());
        }
    }
}