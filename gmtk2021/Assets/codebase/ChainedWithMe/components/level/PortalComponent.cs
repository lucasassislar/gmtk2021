using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class PortalComponent : MonoBehaviour {
        public bool bDisabled;

        private AudioSource audioSource;

        private void Start() {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other) {
            if (bDisabled || 
                other.gameObject.layer != LayerMask.NameToLayer("Player")) {
                return;
            }

            bDisabled = true;
            GameManager.Instance.SwapView(audioSource);
        }
    }
}
