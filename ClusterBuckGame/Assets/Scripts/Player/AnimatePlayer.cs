using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatePlayer : MonoBehaviour
{
    private PlayerScript ps;
    private Animator anim;


    
    private void Start()
    {
        ps = GetComponentInParent<PlayerScript>();
        anim = GetComponent<Animator>();

    }

    private void Update()
    {
        if(ps != null)
        {
            //get the player new input system movement
            Vector2 moveVect = ps.GetMoveInput();

            //set idle and 0 the x and y values when player is still
            if (moveVect == Vector2.zero)
            {
                anim.SetBool("isIdle", true);
                anim.SetFloat("xVal", 0);
                anim.SetFloat("yVal", 0);
            }
            else
            {
                anim.SetBool("isIdle", false);
            }

            //set x and y values for blend tree to use the 4 directional movement animations
            if(moveVect.x >= 0.2f)
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
        }
        
    }
}
