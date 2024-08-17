using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;


public class AnimatePlayer : NetworkAnimator
{
    private PlayerScript ps;
    private Animator anim;

    [SerializeField] float lerpSensitivity = 1f;


    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ps = GetComponentInParent<PlayerScript>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (ps != null)
        {
            //get the player new input system movement
            Vector2 moveVect = ps.GetMoveInput();

            //0 the x and y values to play the idle animation from the blend tree
            if (moveVect.x == 0)
            {
                anim.SetFloat("xVal", 0);
            }
            if (moveVect.y == 0)
            {
                anim.SetFloat("yVal", 0);
            }

            //set x and y values for blend tree to use the 4 directional movement animations
            if (moveVect.x >= 0.2f)
            {
                anim.SetFloat("xVal", 1);

            }
            if(moveVect.x <= -0.2f)
            {
                anim.SetFloat("xVal", -1);

            }
            if(moveVect.y >= 0.2f)
            {
                anim.SetFloat("yVal", 1);

            }
            if (moveVect.y <= -0.2f)
            {
                anim.SetFloat("yVal", -1);

            }

            
           
            
            //if charge attack
            //anim.SetFloat("xVal", 0);
            //anim.SetFloat("yVal", 2);
        }

    }

}
