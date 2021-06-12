using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim_player_01 : MonoBehaviour {

    Animator animator;
    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        bool isWalking = animator.GetBool("isWalking");
        //float fVertical = Input.GetAxisRaw("Vertical");
        //float fHorizontal = Input.GetAxisRaw("Horizontal");

        bool pressedAnim = Input.GetKey(KeyCode.W);
        bool attackPressed = Input.GetKeyDown(KeyCode.Space);
        bool deadPressed = Input.GetKeyDown(KeyCode.G);

        if (!isWalking && pressedAnim) {
            animator.SetBool("isWalking", true);

        }

        if (isWalking && !pressedAnim) {
            animator.SetBool("isWalking", false);

        }

        if (attackPressed)
            Attack();


        if (deadPressed)
            isDead();

        //if (!isWalking && fVertical >0 || fHorizontal > 0) {
        //    animator.SetBool("isWalking", true);

        //}

        //if (isWalking && fVertical < 1 || fHorizontal < 1) {
        //    animator.SetBool("isWalking", false);

        //}



    }

    void isDead() {
        animator.SetTrigger("isDead");
    }

    void Attack() {
        animator.SetTrigger("isAttacking");
    }
}
