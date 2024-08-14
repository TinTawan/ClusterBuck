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
            if (ps.GetMoveInput() == Vector2.zero)
            {
                anim.SetBool("isWalking", false);
            }
            else
            {
                anim.SetBool("isWalking", true);
            }
        }
        
    }
}
