using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Handle : MonoBehaviour
{
    [SerializeField] XRGrabInteractable handle;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform handleTransform;
    [SerializeField] SlotHandler slotHandler;
    [SerializeField] AudioSource audioSource;


    bool handleDown = false;
    bool AudioPlayed = false;
    float returnSpeed = 150f;

    private bool isGrabbed = false;
    public bool IsGrabbed => isGrabbed;

    private void Awake()
    {
        handle.enabled = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = false;

        Debug.Log("[Handle] Awake: Grab disabled, Rigidbody unconstrained, gravity off.");
    }

    private void Start()
    {
        EnableGrab();
        handle.onSelectEntered.AddListener(OnGrabbed);
        handle.onSelectExited.AddListener(OnReleased);

        Debug.Log("[Handle] Start: Grab enabled, listeners attached.");
    }

    private void Update()
    {
        float angle = NormalizeAngle(handleTransform.localEulerAngles.x);

    if (angle <= -40f && !AudioPlayed)
    {
        audioSource.Play();
        AudioPlayed = true;
    }


    if (angle <= -60f && !handleDown)
    {
        DisableGrab();
        handleDown = true;
        slotHandler.SpinReceived();

    }


        if (handleDown && !isGrabbed)
        {
            rb.angularVelocity = Vector3.up * returnSpeed * Time.deltaTime;
            Debug.Log($"[Handle] Returning up. Angular velocity: {rb.angularVelocity}");
        }

        // Handle back upright
        if (angle >= -1f && handleDown)
        {
            rb.angularVelocity = Vector3.zero;
            handleDown = false;
            AudioPlayed = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationY |
                             RigidbodyConstraints.FreezeRotationZ;

            EnableGrab();
            Debug.Log("[Handle] Handle returned upright. Grab re-enabled, rotations frozen.");
        }
    }

    private void OnGrabbed(XRBaseInteractor interactor)
    {
        isGrabbed = true;
        
        Debug.Log("[Handle] Handle grabbed");
    }

    private void OnReleased(XRBaseInteractor interactor)
    {
        isGrabbed = false;
        Debug.Log("[Handle] Handle released");
    }

    public void EnableGrab()
    {
        handle.enabled = true;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;

        Debug.Log("[Handle] EnableGrab called: Grab enabled, angular velocity reset, Rigidbody unconstrained");
    }

    private void DisableGrab()
    {
        handle.enabled = false;
        Debug.Log("[Handle] DisableGrab called: Grab disabled");
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        // Debug only occasional calls if needed
        // Debug.Log($"[Handle] NormalizeAngle: Input {angle + (angle > 180f ? 360f : 0)}, Output {angle}");
        return angle;
    }

    public void ResetHandle()
    {
        handleDown = false;
        AudioPlayed = false;
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = true;
        handleTransform.eulerAngles = Vector3.zero;
        EnableGrab();

        Debug.Log("[Handle] ResetHandle called: Handle state reset, rotations frozen, grab re-enabled");
    }

}
