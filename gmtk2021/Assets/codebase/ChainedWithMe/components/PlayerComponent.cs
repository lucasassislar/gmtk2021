using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class PlayerComponent : MonoBehaviour {
        public float fSpeed = 1;

        private CharacterController objCharController;

        private InputData inputData;

        private void Start() {
            objCharController = GetComponent<CharacterController>();
        }

        private void Update() {
            inputData = new InputData();
            inputData.Horizontal = Input.GetAxisRaw("Horizontal");
            inputData.Vertical = Input.GetAxisRaw("Vertical");
        }

        private void FixedUpdate() {
            float fGravity = Physics.gravity.y;

            objCharController.Move(new Vector3(inputData.Horizontal * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, inputData.Vertical * -fSpeed * Time.deltaTime));
        }

        public void SetPosition(Vector3 pos) {
            objCharController.enabled = false;
            this.transform.position = pos;
            objCharController.enabled = true;
        }

        void OnControllerColliderHit(ControllerColliderHit hit) {
            int layer = hit.collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Wall") ||
                layer == LayerMask.NameToLayer("Enemy") ||
                layer == LayerMask.NameToLayer("OuterWall")) {
                // death
                GameManager.Instance.RestartGame();
                return;
            }
        }
    }
}
