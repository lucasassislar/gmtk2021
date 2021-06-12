using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim_auto_door : MonoBehaviour {
    Animator animator;
    private bool bInsideTrigger;
    public float timeValue = 5;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {

        bool isOpened = animator.GetBool("isOpened");

        bool pressedOpen = Input.GetKeyDown(KeyCode.E);

        if (!isOpened && pressedOpen && bInsideTrigger) {
            isOpened = true;
            Debug.Log("aperto e");
        }

        if (isOpened && timeValue >=0) {
            timeValue -= Time.deltaTime;
            Debug.Log("comeco contar o tempo");

        }

        if (timeValue <= 0) {
            isOpened = false;
            Debug.Log("termino o tempo");


        }

    }

    private void OnTriggerEnter(Collider other) {

        bInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other) {
       
        bInsideTrigger = false;

    }
}
