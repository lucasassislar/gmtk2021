using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class AutoDoorComponent : MonoBehaviour {
        public float fMaxPlatformTime = 2;

        private float fTimeValue;
        private bool bInsideTrigger;
        private Animator animator;

        // Start is called before the first frame update
        void Start() {
            animator = GetComponent<Animator>();
            fTimeValue = fMaxPlatformTime;
        }

        // Update is called once per frame
        void Update() {
            if (GameManager.Instance.RealPlayer == null) {
                return;
            }

            NetworkPlayerComponent netPlayer = GameManager.Instance.RealPlayer;
            bool bIsOpened = animator.GetBool("isOpened");

            bool bIsIdle = false;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("door_closed")) {
                bIsIdle = true;
            }

            if (bInsideTrigger && netPlayer.Interacting) {
                if (bIsIdle) {
                    bIsOpened = true;
                }
            }

            if (bIsOpened) {
                fTimeValue -= Time.deltaTime;

                if (fTimeValue <= 0) {
                    bIsOpened = false;
                    fTimeValue = fMaxPlatformTime;
                }
            }

            animator.SetBool("isOpened", bIsOpened);
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