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
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        private CharacterController objCharController;

        private void Start() {
            objCharController = GetComponentInChildren<CharacterController>();

            GameManager.Instance.StartGame(this);
        }

        private Vector3 vInputData;
        private float fTimer;

        private void FixedUpdate() {
            float fGravity = Physics.gravity.y;

            if (fTimer > 1) {
                float fDistance = Vector3.Distance(objCharController.transform.position, Position.Value);
                if (fDistance > 0.25f) {
                    fTimer = 0;
                    SetPosition(Position.Value);
                }
            }

            if (IsServer) {
                SendPosClientRpc();
            }

            if (IsOwner) {
                objCharController.SimpleMove(new Vector3(vInputData.x * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, vInputData.y * -fSpeed * Time.deltaTime));

                if (IsServer) {
                    SendDataClientRpc(vInputData.x, vInputData.y, vInputData.z);
                } else {
                    SendDataServerRpc(vInputData.x, vInputData.y, vInputData.z);
                }
            } else {
                Vector3 vData = Data.Value;
                objCharController.SimpleMove(new Vector3(vData.x * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, vData.y * -fSpeed * Time.deltaTime));

                //objCharController.enabled = false;
                //objCharController.transform.position = Position.Value;
            }
        }

        private void Update() {
            fTimer += Time.deltaTime;

            if (IsOwner) {
                float fHor = Input.GetAxisRaw("Horizontal");
                float fVer = Input.GetAxisRaw("Vertical");

                float fAttack = 0;
                if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
                    fAttack = 1;
                }

                vInputData = new Vector3(fHor, fVer, fAttack);
            }
        }

        [ServerRpc]
        private void SendDataServerRpc(float fHor, float fVer, float fAttack) {
            Data.Value = new Vector3(fHor, fVer, fAttack);
        }

        [ClientRpc]
        private void SendDataClientRpc(float fHor, float fVer, float fAttack) {
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
            objCharController.transform.position = pos;
            objCharController.enabled = true;
        }
    }
}
