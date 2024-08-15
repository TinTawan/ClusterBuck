using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabiliser : MonoBehaviour
{
    [SerializeField] private Rigidbody spineRB;

    [SerializeField] bool activateForce;
    [SerializeField] float forceVal = 2f;


    private void FixedUpdate()
    {
        if(activateForce)
        {
            spineRB.AddForce(Vector3.up * forceVal * Time.deltaTime);


        }
    }
}
