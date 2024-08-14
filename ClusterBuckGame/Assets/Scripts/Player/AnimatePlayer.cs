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
            Vector2 moveVect = ps.GetMoveInput();

            Debug.Log(moveVect);

            if (moveVect == Vector2.zero)
            {
                anim.SetBool("isIdle", true);
            }
            else
            {
                anim.SetBool("isIdle", false);
            }

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
