using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class VRBikePump : MonoBehaviour
{

    public Transform pumpHandle;
    

    public Transform pumpBody;

    public GameObject Balloon;

    public TextMeshPro CounterText;

    public float maxPumpDistance = 0.4f;
    public float minPumpDistance = 0.2f;
    public float pumpThreshold = 0.25f;
    public float resetSpeed = 2f;
    private int pumpCount = 0;

    public AudioSource pumpSound;
    
    [Tooltip("Drag the ParticleSystem component here (on HoseConnection GameObject)")]
    public ParticleSystem airParticles;
    
    private XRGrabInteractable grabInteractable;
    private Vector3 lastHandlePosition;
    private bool isGrabbed = false;
    private bool hasPumped = false;
    
    void Start()
    {
        if (pumpHandle == null || pumpBody == null)
        {
            Debug.LogError("VRBikePump: Missing pump components! Check Inspector assignments.");
            enabled = false;
            return;
        }
        
        lastHandlePosition = pumpHandle.position;
        
        grabInteractable = pumpHandle.GetComponent<XRGrabInteractable>();
        
        if (grabInteractable == null)
        {
            Debug.LogError("VRBikePump: PumpHandle is missing XRGrabInteractable component!");
            enabled = false;
            return;
        }
        
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        
        Debug.Log("VRBikePump: Initialized successfully!");
    }
    
    void Update()
    {
        if (isGrabbed)
        {
            ConstrainHandleMovement();
            CheckPumpAction();
        }
        else
        {
            ReturnHandleToRest();
        }
    }
    
    void ConstrainHandleMovement()
    {
        Vector3 currentPos = pumpHandle.position;
        Vector3 pumpDirection = pumpBody.up;
        
        Vector3 bodyToHandle = currentPos - pumpBody.position;
        float distanceAlongAxis = Vector3.Dot(bodyToHandle, pumpDirection);
        
        distanceAlongAxis = Mathf.Clamp(distanceAlongAxis, minPumpDistance, maxPumpDistance);
        
        Vector3 constrainedPos = pumpBody.position + pumpDirection * distanceAlongAxis;
        pumpHandle.position = constrainedPos;
        
        pumpHandle.rotation = pumpBody.rotation;
    }
    
    void CheckPumpAction()
    {
        Vector3 pumpDirection = pumpBody.up;
        Vector3 movement = pumpHandle.position - lastHandlePosition;
        float movementAlongAxis = Vector3.Dot(movement, pumpDirection);
        
        Vector3 bodyToHandle = pumpHandle.position - pumpBody.position;
        float currentDistance = Vector3.Dot(bodyToHandle, pumpDirection);
        
        if (movementAlongAxis < -0.001f && !hasPumped)
        {
            if (currentDistance <= pumpThreshold)
            {
                ExecutePump();
                hasPumped = true;
            }
        }
        
        if (movementAlongAxis > 0.001f && currentDistance > pumpThreshold * 1.2f)
        {
            hasPumped = false;
        }
        
        lastHandlePosition = pumpHandle.position;
    }
    
    void ExecutePump()
    {
        if (pumpSound != null && pumpSound.clip != null)
        {
            pumpSound.pitch = Random.Range(0.9f, 1.1f);
            pumpSound.Play();
        }
        
        if (airParticles != null)
        {
            airParticles.Play();
        }
        
        TriggerHaptic();
        InflateBalloon();
        
        Debug.Log("Pump action executed!");
        UpdateCounterText();
    }
    
    void ReturnHandleToRest()
    {
        Vector3 targetPos = pumpBody.position + pumpBody.up * maxPumpDistance;
        pumpHandle.position = Vector3.Lerp(pumpHandle.position, targetPos, resetSpeed * Time.deltaTime);
        pumpHandle.rotation = pumpBody.rotation;
    }
    
    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        lastHandlePosition = pumpHandle.position;
    }
    
    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        hasPumped = false;
    }
    
    void TriggerHaptic()
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            var controller = grabInteractable.firstInteractorSelecting as XRBaseControllerInteractor;
            if (controller != null)
            {
                controller.SendHapticImpulse(0.5f, 0.2f);
            }
        }
    }
    
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    void InflateBalloon()
    {
        if (Balloon != null)
        {
            Balloon.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            Debug.Log("Balloon inflated!");
        }
    }

    void UpdateCounterText()
    {
        if (CounterText != null)
        {
            pumpCount++;
            CounterText.text = $"Pumps:\n{pumpCount}";
        }
    }
}