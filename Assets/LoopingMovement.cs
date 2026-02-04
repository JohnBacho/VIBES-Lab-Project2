using UnityEngine;
using System.Collections;

public class LoopingMovement : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float speed = 1f;
    private Vector3 startLocalPosition;
    
    private void Start() {
        startLocalPosition = transform.localPosition;
    }
    
    private void OnEnable() {
        StartCoroutine(MoveLoop());
    }
    
    IEnumerator MoveLoop()
    {
        while (true)
        {
            while (Vector3.Distance(transform.position, targetPoint.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            transform.localPosition = startLocalPosition;
            yield return new WaitForSeconds(0.7f);
            yield return null;
        }
    }
}