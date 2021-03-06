using MLAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class PlayerTriggerComponent : MonoBehaviour {
        public bool InsideTrigger { get; private set; }

        public bool Interacting { get; private set; }

        public bool bInsideTrigger;

        public void Interact() {
            Interacting = true;
        }

        public void HandledInteract() {
            Interacting = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
                return;
            }
            InsideTrigger = true;

            bInsideTrigger = true;

            GameManager.Instance.EtherealPlayer.SetInside(this);
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
                return;
            }
            InsideTrigger = false;
            bInsideTrigger = false;

            GameManager.Instance.EtherealPlayer.SetInsideOut(this);
        }
    }
}
