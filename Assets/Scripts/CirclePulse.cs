using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePulse : MonoBehaviour
{
    [SerializeField] private GameObject gripButtonHighlight;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private Color idleColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color TargetColor = new Color(1f, 1f, 1f, 0.2f);
    private Coroutine pulseCoroutine;
    private Material materialInstance;
    private Renderer targetRenderer;
    
    void Awake()
    {
        if (gripButtonHighlight != null)
        {
            targetRenderer = gripButtonHighlight.GetComponent<Renderer>();
            
            if (targetRenderer != null)
            {
                materialInstance = targetRenderer.material;
                materialInstance.color = idleColor;
            }
        }
    }

     void OnEnable() {
        pulseCoroutine = StartCoroutine(PulseColor());
    }
     private IEnumerator PulseColor()
    {
        if (materialInstance == null)
        {
            yield break;
        }

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;
            float lerp = Mathf.PingPong(t, 1f);
            materialInstance.color = Color.Lerp(idleColor, TargetColor, lerp);
            Debug.Log("Pulsing Color");
            yield return null;
        }   
    }

    void OnDisable()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        if (materialInstance != null)
        {
            materialInstance.color = idleColor;
        }
    }
}