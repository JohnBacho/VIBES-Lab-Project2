using UnityEngine;

public enum BallType { Red, Blue, Green }

public class Ball : MonoBehaviour
{
    public BallType ballType; // Assign in Inspector

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb == null)
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
}
