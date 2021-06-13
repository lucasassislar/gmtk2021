using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class HorizontalPlatformComponent : NetworkBehaviour {
        public float fMaxPlatformTime = 1;

        private Animator animator;

        private bool bInsideTrigger;
        private float fTimeValue;

        private AudioSource audioSource;

        public AudioClip clipGoing;
        public AudioClip clipComing;

        private void Start() {
            animator = GetComponentInChildren<Animator>();

            audioSource = GetComponent<AudioSource>();
        }

        private void Update() {
            if (GameManager.Instance.RealPlayer == null) {
                return;
            }

            if (NetworkManager.Singleton.IsHost) {
                NetworkPlayerComponent netPlayer = GameManager.Instance.RealPlayer;

                bool bIsIdle;
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("anim_plat_horizontal_back_idle")) {
                    bIsIdle = true;
                } else {
                    bIsIdle = false;
                }

                if (bInsideTrigger && netPlayer.Interacting) {
                    if (bIsIdle) {
                        EnablePlatformClientRpc();
                    }
                }
            }

            bool bIsGoingFoward = animator.GetBool("isGoingFoward");
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