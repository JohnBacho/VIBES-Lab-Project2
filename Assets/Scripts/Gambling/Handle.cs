using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Handle : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] XRGrabInteractable handle;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform handleTransform;

    bool handleDown = false;
    float returnSpeed = 150f;

    private bool isGrabbed = false;
    public bool IsGrabbed => isGrabbed;

    private void Awake()
    {
        handle.enabled = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = false;
    }

    private void Start()
    {
        EnableGrab();
        handle.onSelectEntered.AddListener(OnGrabbed);
        handle.onSelectExited.AddListener(OnReleased);
    }

    private void Update()
    {
        float angle = NormalizeAngle(handleTransform.localEulerAngles.x);

        // Detect handle pulled down
        if (angle <= -60f && !handleDown)
        {
            DisableGrab();
            handleDown = true;
            gameManager.SpinReceived();
        }

        // Handle returning up, only if not grabbed
        if (handleDown && !isGrabbed)
        {
            rb.angularVelocity = Vector3.up * returnSpeed * Time.deltaTime;
        }

        // Handle back upright
        if (angle >= -1f && handleDown)
        {
            rb.angularVelocity = Vector3.zero;
            handleDown = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationY |
                             RigidbodyConstraints.FreezeRotationZ;

            EnableGrab();
        }
    }

    private void OnGrabbed(XRBaseInteractor interactor)
    {
        isGrabbed = true;
        Debug.Log("Handle grabbed");
    }

    private void OnReleased(XRBaseInteractor interactor)
    {
        isGrabbed = false;
        Debug.Log("Handle released");
    }

    public void EnableGrab()
    {
        handle.enabled = true;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
    }

    private void DisableGrab()
    {
        handle.enabled = false;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
