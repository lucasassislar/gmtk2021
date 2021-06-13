
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pusher_anim : MonoBehaviour {
    Animator animator;
    public float timeValue = 3;
    // Start is called before the first frame update
    void Start() {

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        if (timeValue >= 0)
            timeValue -= Time.deltaTime;


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
            timeValue = 3;
            animator.SetTrigger("isPushing");

        }

    }
}
