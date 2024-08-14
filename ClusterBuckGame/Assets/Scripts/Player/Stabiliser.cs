using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabiliser : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Rigidbody spineRB;
    [SerializeField] private ConfigurableJoint joint;

    [SerializeField] bool activateForce;

    [SerializeField] float forceVal = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //joint = GetComponent<ConfigurableJoint>();

    }

    private void FixedUpdate()
    {
        if(activateForce)
        {
            spineRB.AddForce(Vector3.up * forceVal * Time.deltaTime);

            //joint.targetVelocity = Vector3.up * forceVal;

        }
    }
}
