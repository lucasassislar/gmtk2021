
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pusher_anim : MonoBehaviour {
    Animator animator;
    private float timeValue;
    public float fMaxPusherTime = 1;

    // Start is called before the first frame update
    void Start() {
        timeValue = fMaxPusherTime;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") && timeValue >= 0){
            timeValue -= Time.deltaTime;

        }

        if (timeValue <= 0) {
            timeValue = fMaxPusherTime;
            animator.SetTrigger("isPushing");
        }



    }
}
