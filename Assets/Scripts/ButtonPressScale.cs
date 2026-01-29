using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPokeSmoothScale : MonoBehaviour
{
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float smoothSpeed = 12f;

    private Vector3 startScale;
    private Vector3 targetScale;
    private XRSimpleInteractable interactable;

    void Awake()
    {
        startScale = transform.localScale;
        targetScale = startScale;

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(_ => Press());
        interactable.selectExited.AddListener(_ => Release());
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    private void Press()
    {
        targetScale = startScale * pressedScale;
    }

    private void Release()
    {
        targetScale = startScale;
    }
}
