using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class PortalComponent : MonoBehaviour, IRestartable {
        public bool bDisabled;

        private AudioSource audioSource;

        private bool bStartDisabled;
        private void Start() {
            audioSource = GetComponent<AudioSource>();

            GameManager.Instance.RegisterRestartable(this);
            bStartDisabled = bDisabled;
        }

        public void Restart() {
            bDisabled = bStartDisabled;
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
