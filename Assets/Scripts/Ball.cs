using UnityEngine;

public enum BallType { Red, Blue, Green }

public class Ball : MonoBehaviour
{
    public BallType ballType; // Assign in Inspector
    private Rigidbody rb;
    private bool hasCounted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = true;

        MeshRenderer renderer = GetComponent<MeshRenderer>();

        switch (ballType)
        {
            case BallType.Red:
                renderer.material.color = Color.red;
                break;
            case BallType.Blue:
                renderer.material.color = Color.blue;
                break;
            case BallType.Green:
                renderer.material.color = Color.green;
                break;
        }
    }


    private void Update() {
    {
        if (!hasCounted &&  new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 1f)
        {
            hasCounted = true;
            sxr.IncrementBallsThrown();
        }
    }
    }
}
