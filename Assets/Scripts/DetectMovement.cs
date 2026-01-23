using UnityEngine;

public class DetectMovement : MonoBehaviour
{
    [SerializeField] private ParlayTutorial parlayTutorial;
    private Vector3 lastPosition;
    [SerializeField] private float movementThreshold = 0.01f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, lastPosition) > movementThreshold)
        {
            Debug.Log("Object moved");
            parlayTutorial.HideGrabHandleTutorial();
            enabled = false;
        }
    }
}
