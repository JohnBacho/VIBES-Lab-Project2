using System.Collections;
using UnityEngine;

public class ControllerThrowAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private GameObject controllerModel;
    [SerializeField] private GameObject ball;
    
    [Header("Rotation Settings")]
    [SerializeField] private float windupAngle = -45f; 
    [SerializeField] private float windupDuration = 0.5f;
    [SerializeField] private float throwDuration = 0.3f; 
    [SerializeField] private float holdDuration = 0.5f;
    
    [Header("Throw Settings")]
    [SerializeField] private bool animateBall = true;
    [SerializeField] private float ballThrowDistance = 5f;
    [SerializeField] private float ballArcHeight = 2f;
    
    private Vector3 controllerStartRotation;
    private Vector3 ballStartPosition;
    
    void Start()
    {
        if (controllerModel != null)
        {
            controllerStartRotation = controllerModel.transform.localEulerAngles;
        }
        
        if (ball != null)
        {
            ballStartPosition = ball.transform.localPosition;
        }
    }
    
    void OnEnable()
    {
        StartCoroutine(PlayThrowAnimation());
    }
    
    private IEnumerator PlayThrowAnimation()
    {
        if (controllerModel == null)
        {
            Debug.LogError("Controller model not assigned!");
            yield break;
        }
        
        while (true)
        {
            yield return StartCoroutine(RotateController(controllerStartRotation, 
                controllerStartRotation + new Vector3(windupAngle, 0, 0), 
                windupDuration));
            
            yield return new WaitForSeconds(holdDuration);
            
            if (animateBall && ball != null)
            {
                StartCoroutine(ThrowBall());
            }
            
            yield return StartCoroutine(RotateController(
                controllerModel.transform.localEulerAngles, 
                controllerStartRotation, 
                throwDuration));
            
            yield return new WaitForSeconds(1f);
            if (ball != null)
            {
                ball.transform.localPosition = ballStartPosition;
            }
        }
    }
    
    private IEnumerator RotateController(Vector3 fromRotation, Vector3 toRotation, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            t = Mathf.SmoothStep(0, 1, t);
            
            controllerModel.transform.localEulerAngles = Vector3.Lerp(fromRotation, toRotation, t);
            yield return null;
        }
        
        controllerModel.transform.localEulerAngles = toRotation;
    }
    
    private IEnumerator ThrowBall()
    {
        if (ball == null) yield break;
        
        Vector3 startPos = ball.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 0, ballThrowDistance);
        
        float elapsed = 0f;
        
        while (elapsed < throwDuration + 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (throwDuration + 0.5f);
            
            
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            
            float arc = ballArcHeight * Mathf.Sin(t * Mathf.PI);
            currentPos.y += arc;
            
            ball.transform.localPosition = currentPos;
            yield return null;
        }
    }
    
}