using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class AutoDoorComponent : NetworkBehaviour {
        public float fMaxPlatformTime = 2;

        private float fTimeValue;
        private bool bInsideTrigger;
        private Animator animator;

        private AudioSource audioSource;

        private AutoDoorTriggerComponent trigger;

        void Start() {
            animator = GetComponentInChildren<Animator>();
            fTimeValue = fMaxPlatformTime;

            audioSource = GetComponentInChildren<AudioSource>();
            trigger = GetComponentInChildren<AutoDoorTriggerComponent>();
        }

        void Update() {
            if (GameManager.Instance.RealPlayer == null) {
                return;
            }

            if (NetworkManager.Singleton.IsHost) {
                NetworkPlayerComponent netPlayer = GameManager.Instance.RealPlayer;

                bool bIsIdle = false;
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("door_closed")) {
                    bIsIdle = true;
                }

                if (trigger.InsideTrigger && netPlayer.Interacting) {
                    if (bIsIdle) {
                        OpenDoorClientRpc();
                    }
                }
            }

            bool bIsOpened = animator.GetBool("isOpened");
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