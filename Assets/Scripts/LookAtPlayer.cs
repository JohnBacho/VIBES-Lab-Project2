using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{

    [SerializeField] private Transform playerCamera;


void Update()
{
    Vector3 targetDirection = transform.position - playerCamera.position;
        transform.rotation = Quaternion.LookRotation(targetDirection);
}
}
