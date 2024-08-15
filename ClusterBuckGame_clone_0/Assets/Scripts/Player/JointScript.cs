using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointScript : MonoBehaviour
{

    private Rigidbody rb;
    private ConfigurableJoint joint;

    [SerializeField] private Transform targetJoint;
    [SerializeField] private Quaternion initRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        initRotation = joint.transform.localRotation;
    }

    private void FixedUpdate()
    {
        JointRotation();
    }

    void JointRotation()
    {
        /*Quaternion inRot = targetJoint.rotation;

        if (invertX)
        {
            inRot *= Quaternion.Euler(180, 0, 0);
        }
        if (invertY)
        {
            inRot *= Quaternion.Euler(0, 180, 0);
        }
        if (invertZ)
        {
            inRot *= Quaternion.Euler(0, 0, 180);
        }
        if(swapXY)
        {
            inRot *= Quaternion.Euler(targetJoint.rotation.y, targetJoint.rotation.x, targetJoint.rotation.z);
        }
        if (swapXZ)
        {
            inRot *= Quaternion.Euler(targetJoint.rotation.z, targetJoint.rotation.y, targetJoint.rotation.x);
        }
        if (swapYZ)
        {
            inRot *= Quaternion.Euler(targetJoint.rotation.x, targetJoint.rotation.z, targetJoint.rotation.y);
        }

        

        joint.targetRotation = inRot;*/


        SetTargetRotation(joint, targetJoint.localRotation, initRotation, Space.Self);
    }

    //rotate the joints to copy the animated model's movements
    void SetTargetRotation(ConfigurableJoint inJoint, Quaternion inTargetRot, Quaternion inInitRot, Space inSpace)
    {
        var right = inJoint.axis;
        var forward = Vector3.Cross(inJoint.axis, inJoint.secondaryAxis).normalized;
        var up = Vector3.Cross(forward, right).normalized;
        Quaternion worldtoJointSpace = Quaternion.LookRotation(forward,up);

        Quaternion resultRot = Quaternion.Inverse(worldtoJointSpace);

        /*if(inSpace == Space.World)
        {
            resultRot *= inInitRot * Quaternion.Inverse(inTargetRot);
            
        }
        else
        {
            resultRot *= Quaternion.Inverse(inTargetRot) * inInitRot;

        }*/

        resultRot *= Quaternion.Inverse(inTargetRot) * inInitRot;

        resultRot *= worldtoJointSpace;

        inJoint.targetRotation = resultRot;
    }
}
