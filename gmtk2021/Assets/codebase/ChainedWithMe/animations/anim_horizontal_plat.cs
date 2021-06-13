using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim_horizontal_plat : MonoBehaviour {
    
    
    Animator animator;
    float timeValue = 5;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {
        bool pressedInteractButton = Input.GetKeyDown(KeyCode.E);
        bool isGoingFoward = animator.GetBool("isGoingFoward");

        if(!isGoingFoward && )



    }
}
