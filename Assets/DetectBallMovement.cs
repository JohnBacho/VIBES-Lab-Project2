using UnityEngine;

public class DetectBallMovement : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 startPosition;
    private Rigidbody rb;
    public bool HasMoved { get; private set; } = false;
    [SerializeField] private float movementThreshold = 0.2f;
    [SerializeField] private GameObject Controller;
    
    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (Vector3.Distance(transform.position, lastPosition) > movementThreshold && !HasMoved)
        {
            Debug.Log("Object moved");
            rb.constraints = RigidbodyConstraints.None;
            HasMoved = true;
            Controller.SetActive(false);
        }
    }
    
    public void ResetMovement()
    {
        HasMoved = false;
        lastPosition = transform.position;
        Controller.SetActive(true);
    }
}