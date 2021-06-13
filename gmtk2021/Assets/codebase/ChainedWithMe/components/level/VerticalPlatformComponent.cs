using ChainedWithMe;
using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class VerticalPlatformComponent : NetworkBehaviour {
        public float fMaxPlatformTime = 5;

        private float fTimeValue;
        private Animator animator;

        private AudioSource audioSource;

        public AudioClip clipGoing;
        public AudioClip clipComing;

        private PlayerTriggerComponent trigger;

        public GameObject objPressE;

        private void Start() {
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponentInChildren<AudioSource>();
            trigger = GetComponentInChildren<PlayerTriggerComponent>();

            fTimeValue = fMaxPlatformTime;

            objPressE.SetActive(false);
        }

        private void Update() {
            objPressE.SetActive(trigger.InsideTrigger);

            bool bIsGoingUp = animator.GetBool("isGoingUp");

            if (IsServer) {
                if (fTimeValue > 0) {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("plat_vertical_idle")) {
                        if (trigger.InsideTrigger && trigger.Interacting) {
                            trigger.HandledInteract();

                            EnablePlatformClientRpc();
                        }
                    }

                    if (bIsGoingUp) {
                        trigger.HandledInteract();
                    }
                }
            }

            if (bIsGoingUp) {
                fTimeValue -= Time.deltaTime;

                if (fTimeValue <= 0) {
                    animator.SetBool("isGoingUp", false);
                    fTimeValue = fMaxPlatformTime;

                    audioSource.clip = clipComing;
                    audioSource.Play();
                }
            }
        }

        [ClientRpc]
        private void EnablePlatformClientRpc() {
            animator.SetBool("isGoingUp", true);

            audioSource.clip = clipGoing;
            audioSource.Play();

            GameManager.Instance.AudioManager.PlayClick();
        }
    }
}