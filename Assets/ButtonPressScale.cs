using UnityEngine;
using UnityEngine.EventSystems;

public class VRButtonSmoothScale : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Scales")]
    public float hoverScale = 1.05f;
    public float pressedScale = 0.95f;

    [Header("Smoothing")]
    public float smoothSpeed = 12f; // higher = snappier

    private Vector3 startScale;
    private Vector3 targetScale;

    void Awake()
    {
        startScale = transform.localScale;
        targetScale = startScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = startScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = startScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = startScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = startScale * hoverScale;
    }
}
