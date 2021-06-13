using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class HorizontalPlatformComponent : NetworkBehaviour {
        public float fMaxPlatformTime = 1;

        private Animator animator;

        private float fTimeValue;

        private AudioSource audioSource;

        public AudioClip clipGoing;
        public AudioClip clipComing;

        private PlayerTriggerComponent trigger;

        private void Start() {
            animator = GetComponentInChildren<Animator>();
            trigger = GetComponent<PlayerTriggerComponent>();

            audioSource = GetComponent<AudioSource>();
            fTimeValue = fMaxPlatformTime;
        }

        private void Update() {
            bool bIsGoingFoward = animator.GetBool("isGoingFoward");

            if (IsServer) {
                if (fTimeValue > 0) {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("anim_plat_horizontal_back_idle")) {
                        if (trigger.InsideTrigger && trigger.Interacting) {
                            trigger.HandledInteract();

                            EnablePlatformClientRpc();
                        }
                    }

                    if (bIsGoingFoward) {
                        trigger.HandledInteract();
                    }
                }
            }

            if (bIsGoingFoward) {
                fTimeValue -= Time.deltaTime;

                if (fTimeValue <= 0) {
                    animator.SetBool("isGoingFoward", false);
                    fTimeValue = fMaxPlatformTime;

                    audioSource.clip = clipComing;
                    audioSource.Play();
                }
            }
        }

        [ClientRpc]
        private void EnablePlatformClientRpc() {
            animator.SetBool("isGoingFoward", true);

            audioSource.clip = clipGoing;
            audioSource.Play();

            GameManager.Instance.AudioManager.PlayClick();
        }
    }
}