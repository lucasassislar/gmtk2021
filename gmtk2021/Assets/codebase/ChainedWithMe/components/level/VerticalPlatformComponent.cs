using ChainedWithMe;
using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class VerticalPlatformComponent : NetworkBehaviour {
        public float fMaxPlatformTime = 5;

        private float fTimeValue;
        private Animator animator;

        private AudioSource audioSource;

        public AudioClip clipGoing;
        public AudioClip clipComing;

        private PlayerTriggerComponent trigger;

        private void Start() {
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponentInChildren<AudioSource>();
            trigger = GetComponentInChildren<PlayerTriggerComponent>();

            fTimeValue = fMaxPlatformTime;

        }

        private void Update() {
            if (GameManager.Instance.RealPlayer == null) {
                return;
            }

            if (NetworkManager.Singleton.IsHost) {
                NetworkPlayerComponent netPlayer = GameManager.Instance.RealPlayer;

                bool bIsIdle = false;
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("plat_vertical_idle")) {
                    bIsIdle = true;
                }

                if (trigger.InsideTrigger && netPlayer.Interacting) {
                    if (bIsIdle) {
                        EnablePlatformClientRpc();
                    }
                }
            }

            bool bIsGoingUp = animator.GetBool("isGoingUp");
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