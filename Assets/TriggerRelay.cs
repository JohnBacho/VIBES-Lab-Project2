using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    [SerializeField] private ParlayTutorial parlayTutorial;

    private void OnTriggerEnter(Collider other)
    {
        if (parlayTutorial != null)
        {
            parlayTutorial.OnControllerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (parlayTutorial != null)
        {
            parlayTutorial.OnControllerExit(other);
        }
    }
}