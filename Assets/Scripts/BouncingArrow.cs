using UnityEngine;

public class BouncingArrow : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceHeight = 0.02f;
    [SerializeField] private float bounceSpeed = 2f;
    [SerializeField] private bool bounceHorizontally = false;

    private Vector3 baseLocalPosition;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        if (bounceHorizontally)
        {
            transform.localPosition = baseLocalPosition + new Vector3(offset, 0, 0);
        }
        else
        {
            transform.localPosition = baseLocalPosition + new Vector3(0, offset, 0);
        }
    }
}
