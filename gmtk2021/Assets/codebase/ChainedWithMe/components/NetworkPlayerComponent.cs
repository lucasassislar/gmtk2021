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
    public class NetworkPlayerComponent : MonoBehaviour {
        public float fSpeed = 1;

        public NetworkVariableVector3 Data = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        private NetworkObject objNetwork;

        private CharacterController objCharController;

        private void Start() {
            objNetwork = GetComponent<NetworkObject>();

            objCharController = GetComponentInChildren<CharacterController>();

            GameManager.Instance.StartGame(this);
        }

        private void FixedUpdate() {
            if (NetworkManager.Singleton.IsHost) {
                float fGravity = Physics.gravity.y;

                Vector3 vData = Data.Value;
                objCharController.Move(new Vector3(vData.x * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, vData.y * -fSpeed * Time.deltaTime));
            }
        }


        private void Update() {
            if (objNetwork.IsOwner) {
                float fHor = Input.GetAxisRaw("Horizontal");
                float fVer = Input.GetAxisRaw("Vertical");

                float fAttack = 0;
                if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
                    fAttack = 1;
                }

                SendDataToHost(fHor, fVer, fAttack);
            }

            if (NetworkManager.Singleton.IsServer) {
                SendPosToClient();
            } else {
                objCharController.enabled = false;
                this.transform.position = Position.Value;
            }
        }

        [ServerRpc]
        private void SendDataToHost(float fHor, float fVer, float fAttack) {
            Data.Value = new Vector3(fHor, fVer, fAttack);
            Data.SetDirty(true);
        }

        [ClientRpc]
        private void SendPosToClient() {
            Position.Value = transform.position;
            Position.SetDirty(true);
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
