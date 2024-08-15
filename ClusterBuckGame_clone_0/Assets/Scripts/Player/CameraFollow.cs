using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    static CameraFollow instance;

    private CinemachineFreeLook freeLookCam;

    public static CameraFollow Instance 
    { 
        get 
        { 
            if(instance == null)
            {
                instance = new CameraFollow();
            }

            return instance; 
        }
        
    }

    private void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();
    }

    public void LookToPlayer(Transform transform)
    {
        if(freeLookCam != null)
        {
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;
        }
        
    }
}
