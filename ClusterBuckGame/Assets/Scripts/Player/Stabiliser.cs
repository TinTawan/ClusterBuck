using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabiliser : MonoBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint joint;

    [SerializeField] bool activateForce;

    [SerializeField] float forceVal = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

    }

    private void FixedUpdate()
    {
        if(activateForce)
        {
            //rb.AddForce(upForce * forceVal * Time.deltaTime);
            //rb.velocity = upForce * forceVal * Time.deltaTime;

            rb.MovePosition(rb.position + Vector3.up * forceVal * Time.fixedDeltaTime);
        }
    }
}
