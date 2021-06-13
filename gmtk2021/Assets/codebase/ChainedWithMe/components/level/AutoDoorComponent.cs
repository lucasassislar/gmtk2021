using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class AutoDoorComponent : NetworkBehaviour {
        public float fMaxPlatformTime = 2;

        private float fTimeValue;
        private bool bInsideTrigger;
        private Animator animator;

        private AudioSource audioSource;

        private PlayerTriggerComponent trigger;

        void Start() {
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponentInChildren<AudioSource>();
            trigger = GetComponentInChildren<PlayerTriggerComponent>();

            fTimeValue = fMaxPlatformTime;
        }

        void Update() {
            bool bIsOpened = animator.GetBool("isOpened");

            if (IsServer) {
                if (fTimeValue > 0) {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("door_closed")) {
                        if (trigger.InsideTrigger && trigger.Interacting) {
                            trigger.HandledInteract();

                            OpenDoorClientRpc();
                        }
                    }

                    if (bIsOpened) {
                        trigger.HandledInteract();
                    }
                }
            }

            if (bIsOpened) {
                fTimeValue -= Time.deltaTime;

                if (fTimeValue <= 0) {
                    animator.SetBool("isOpened", false);
                    fTimeValue = fMaxPlatformTime;
                }
            }
        }

        [ClientRpc]
        private void OpenDoorClientRpc() {
            animator.SetBool("isOpened", true);
            audioSource.Play();

            GameManager.Instance.AudioManager.PlayClick();
        }
    }
}