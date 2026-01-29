using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BART : MonoBehaviour
{

    [SerializeField] private  Transform pumpHandle;
    

    [SerializeField] private  Transform pumpBody;

    [SerializeField] private GameObject[] BalloonArray;

    [SerializeField] private TextMeshPro CounterText;

    [SerializeField] private float maxPumpDistance = 0.4f;
    [SerializeField] private float minPumpDistance = 0.2f;
    [SerializeField] private float pumpThreshold = 0.25f;
    [SerializeField] private float resetSpeed = 2f;
    private int pumpCount = 0;

    private int BlueBalloonPop = 0;
    private int YellowBalloonPop = 0;
    private int OrangeBalloonPop = 0;
    private int arraycounter = 0;

    [SerializeField] private AudioSource pumpSound;
    [SerializeField] private AudioSource PopSound;

    private GameObject Balloon;

    
    [SerializeField] private ParticleSystem airParticles;
    
    private XRGrabInteractable grabInteractable;
    private Vector3 lastHandlePosition;
    private bool isGrabbed = false;
    private bool hasPumped = false;
    private enum BalloonType { Blue, Yellow, Orange }
    private BalloonType CurrentBalloonType = BalloonType.Blue;
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
        
        Balloon = BalloonArray[arraycounter];
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        GenerateExplosionPoint(); 

        for (int i = 0; i < BalloonArray.Length; i++)
        {
            if (i == arraycounter)
                BalloonArray[i].SetActive(true);
            else
                BalloonArray[i].SetActive(false);
        }

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

    StartCoroutine(CheckBalloonPop());
    }

    void GenerateExplosionPoint()
    {
        BlueBalloonPop = Random.Range(50, 78);
        YellowBalloonPop = Random.Range(8, 24);
        OrangeBalloonPop = Random.Range(2, 6);
    }

    IEnumerator CheckBalloonPop()
    {
        if (pumpCount == BlueBalloonPop && CurrentBalloonType == BalloonType.Blue)
        {
            Debug.Log("Blue Balloon Pop!");
            Balloon.SetActive(false);
            PopSound.Play();
            yield return new WaitForSeconds(1.5f);
            SwitchBalloon();
            yield break;
        }

        if (pumpCount == YellowBalloonPop && CurrentBalloonType == BalloonType.Yellow)
        {
            Debug.Log("Yellow Balloon Pop!");
            Balloon.SetActive(false);
            PopSound.Play();
            yield return new WaitForSeconds(1.5f);
            SwitchBalloon();
            yield break;
        }

        if (pumpCount == OrangeBalloonPop && CurrentBalloonType == BalloonType.Orange)
        {
            Debug.Log("Orange Balloon Pop!");
            Balloon.SetActive(false);
            PopSound.Play();
            yield return new WaitForSeconds(1.5f);
            SwitchBalloon();
            yield break;
        }

        yield break;
    }


    public void Cashout()
    {
        Debug.Log($"Cashed out at {pumpCount} pumps!");
        Balloon.SetActive(false);
        SwitchBalloon();
    }

    void SwitchBalloon()
    {
        pumpCount = -1;
        UpdateCounterText();
        arraycounter = (arraycounter + 1) % BalloonArray.Length;
        switch (arraycounter)
        {
            case 0:
                Balloon = BalloonArray[0];
                CurrentBalloonType = BalloonType.Blue;
                break;
            case 1:
                Balloon = BalloonArray[1];
                CurrentBalloonType = BalloonType.Yellow;
                break;
            case 2:
                Balloon = BalloonArray[2];
                CurrentBalloonType = BalloonType.Orange;
                break;
        }
        Balloon.SetActive(true);
    }
}