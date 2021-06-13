using ChainedWithMe;
using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class VerticalPlatformComponent : MonoBehaviour {
        public float fMaxPlatformTime = 5;

        private float fTimeValue;
        private bool bInsideTrigger;
        private Animator animator;

        private AudioSource audioSource;

        public AudioClip clipGoing;
        public AudioClip clipComing;

        private void Start() {
            animator = GetComponent<Animator>();
            fTimeValue = fMaxPlatformTime;

            audioSource = GetComponent<AudioSource>();
        }

        private void Update() {
            if (GameManager.Instance.RealPlayer == null) {
                return;
            }

            NetworkPlayerComponent netPlayer = GameManager.Instance.RealPlayer;
            bool bIsGoingUp = animator.GetBool("isGoingUp");

            bool bIsIdle = false;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("plat_vertical_idle")) {
                bIsIdle = true;
            }

            if (bInsideTrigger && netPlayer.Interacting) {
                if (bIsIdle) {
                    bIsGoingUp = true;

                    audioSource.clip = clipGoing;
                    audioSource.Play();

                    GameManager.Instance.AudioManager.PlayClick();

                }
            }

            if (bIsGoingUp) {
                fTimeValue -= Time.deltaTime;

                if (fTimeValue <= 0) {
                    bIsGoingUp = false;
                    fTimeValue = fMaxPlatformTime;

                    audioSource.clip = clipComing;
                    audioSource.Play();
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
}