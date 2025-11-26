using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    //Reel rigidbody, audio source, whether the reel should spin or not, and the speed at which the reel spins
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioSource source;
    bool spin = false;
    float speed = 50f;

    private void Awake()
    {
        //Don't want it spinning as soon as the player loads in
        spin = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If the reel should be spinning, spin the reel along its x-axis at the given speed
        if (spin)
        {
            rb.angularVelocity = Vector3.left * speed;
        }
    }

    //Allows other scripts to tell the reel that it should be spinning
    public void Spin()
    {
        spin = true;
        rb.freezeRotation = false;
        source.Play();
    }

    //Allows other scripts to tell the reel to stop on a certain number depending on the given result
    public void StopSpin(int result)
    {
        source.Stop();
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = true;
        spin = false;
        gameObject.transform.rotation = Quaternion.Euler(18 * (result + 1) - 90, 0, 0);
    }
}
