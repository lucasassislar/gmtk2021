using ChainedWithMe;
using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatformComponent : NetworkBehaviour {
    public float fMaxPlatformTime = 5;

    private float fTimeValue;
    private bool bInsideTrigger;
    private bool bIsIdle;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        fTimeValue = fMaxPlatformTime;
    }

    private void Update() {
        if (GameManager.Instance.RealPlayer == null) {
            return;
        }

        NetworkPlayerComponent netPlayer = GameManager.Instance.RealPlayer;
        bool bIsGoingUp = animator.GetBool("isGoingUp");

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("plat_vertical_idle")) {
            bIsIdle = true;
        }

        if (bInsideTrigger && netPlayer.Interacting) {
            if (bIsIdle) {
                bIsGoingUp = true;
            }
        }

        if (bIsGoingUp) {
            fTimeValue -= Time.deltaTime;

            if (fTimeValue <= 0) {
                bIsGoingUp = false;
                fTimeValue = fMaxPlatformTime;
            }
        }

        animator.SetBool("isGoingUp", bIsGoingUp);
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
