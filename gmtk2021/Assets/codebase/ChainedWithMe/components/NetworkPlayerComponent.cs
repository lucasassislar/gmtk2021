using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class NetworkPlayerComponent : NetworkBehaviour {
        public float fSpeed = 1;

        public NetworkVariableVector3 Data = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        private CharacterController objCharController;

        private void Start() {
            objCharController = GetComponentInChildren<CharacterController>();

            GameManager.Instance.StartGame(this);
        }

        private void FixedUpdate() {
            if (IsServer) {
                float fGravity = Physics.gravity.y;

                Vector3 vData = Data.Value;
                objCharController.Move(new Vector3(vData.x * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, vData.y * -fSpeed * Time.deltaTime));
            }

            if (IsServer) {
                SendPosClientRpc();
            } else {
                objCharController.enabled = false;
                objCharController.transform.position = Position.Value;
            }
        }

        private void Update() {
            if (IsOwner) {
                float fHor = Input.GetAxisRaw("Horizontal");
                float fVer = Input.GetAxisRaw("Vertical");

                float fAttack = 0;
                if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
                    fAttack = 1;
                }

                SendDataServerRpc(fHor, fVer, fAttack);
            }
        }

        [ServerRpc]
        private void SendDataServerRpc(float fHor, float fVer, float fAttack) {
            Data.Value = new Vector3(fHor, fVer, fAttack);
        }

        [ClientRpc]
        private void SendPosClientRpc() {
            Position.Value = objCharController.transform.position;
        }

        [ClientRpc]
        public void SendClientVersionClientRpc(int nLayerMask) {
            GameManager.Instance.ReceiveLayer(nLayerMask);
        }

        public void SetPosition(Vector3 pos) {
            if (!objCharController) {
                objCharController = GetComponent<CharacterController>();
            }

            objCharController.enabled = false;
            this.transform.position = pos;
            objCharController.enabled = true;
        }
    }
}
