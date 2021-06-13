using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe.Level {
    public class TeleporterComponent : MonoBehaviour, IRestartable {

        public bool bDisabled;
        public GameObject objTarget;

        private PlayerTriggerComponent trigger;
        public GameObject objPressE;

        private bool bStartDisabled;

        private void Start() {
            trigger = GetComponent<PlayerTriggerComponent>();
            objPressE.SetActive(false);
            bStartDisabled = bDisabled;
        }

        public void Restart() {
            bDisabled = bStartDisabled;
        }

        private void Update() {
            objPressE.SetActive(trigger.InsideTrigger);

            if (trigger.InsideTrigger && trigger.Interacting) {
                // do teleport
                GameManager.Instance.RealPlayer.Teleport(objTarget.transform.position);
            }
        }
    }
}
