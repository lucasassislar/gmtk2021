using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class ListBodyComponent : MonoBehaviour, IRestartable {
        public List<BodyComponent> bodies;

        private void Start() {
            GameManager.Instance.RegisterRestartable(this);

            bodies.Clear();
        }

        public void Restart() {
            bodies.Clear();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Body")) {
                return;
            }

            BodyComponent bodyComp = other.GetComponent<BodyComponent>();
            bodies.Add(bodyComp);
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Body")) {
                return;
            }

            BodyComponent bodyComp = other.GetComponent<BodyComponent>();
            bodies.Remove(bodyComp);
        }

    }
}
