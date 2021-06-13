using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim_vertical_plat : MonoBehaviour {
    Animator animator;

    private bool bInsideTrigger;
    public float timeValue = 5;
    private bool canPressButton;



    // Start is called before the first frame update
    void Start() {

        animator = GetComponent<Animator>();
        canPressButton = true;

    }

    // Update is called once per frame
    void Update() {
        bool pressedInteractButton = Input.GetKeyDown(KeyCode.E);
        bool isGoingUp = animator.GetBool("isGoingUp");

        if (!isGoingUp && pressedInteractButton && bInsideTrigger && canPressButton) {
            canPressButton = false;
            Debug.Log("APERTOU");
            isGoingUp = true;
        }

        if (isGoingUp && timeValue >= 0) {
            timeValue -= Time.deltaTime;
        }


        if (isGoingUp && timeValue <= 0) {
            isGoingUp = false;
            timeValue = 5;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("plat_vertical_idle")) {

            canPressButton = true;
        }

        animator.SetBool("isGoingUp", isGoingUp);
  

    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;

        }

        bInsideTrigger = true;
    }


    private void OnTriggerExit(Collider other) {

        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;

        }

        bInsideTrigger = false;

    }





}
