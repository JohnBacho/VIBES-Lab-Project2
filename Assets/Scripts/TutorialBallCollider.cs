using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBallCollider : MonoBehaviour
{
        [SerializeField] private EffortTaskHandler EffortTaskHandler;

    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

    }
}
