using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBallMovement : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 startPosition;
    private Rigidbody rb;
    public bool HasMoved { get; private set; } = false;
    [SerializeField] private bool enableWrapper = false;
    [SerializeField] private float movementThreshold = 0.2f;
    // [SerializeField] private GameObject Wrapper;
    
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
            rb.constraints = RigidbodyConstraints.None;
            HasMoved = true;
            // Wrapper.SetActive(false);
        }
    }
    
    public void ResetMovement()
    {
        HasMoved = false;
        lastPosition = transform.position;
    }

    public void WrongBucketResetMovement()
    {
        HasMoved = false;
        lastPosition = transform.position;
    }
}