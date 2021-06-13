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
    public class NetworkPlayerComponent : NetworkBehaviour, IRestartable {
        public float fSpeed = 1;
        public float fJumpForce = 40;
        public float fJumpTime = 0.1f;

        public MeshRenderer meshRenderer;

        private Vector2 vInputData;
        private float fTimer;

        private float fJumpTimer;

        private bool bSent;
        private bool bSetPosition;

        private float fCurrentJumpForce;

        public CharacterController CharController { get; private set; }

        public bool Interacting { get; private set; }

        public NetworkVariableVector2 Data = new NetworkVariableVector2(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        private void Start() {
            CharController = GetComponentInChildren<CharacterController>();

            GameManager.Instance.StartGame(this);

            GameManager.Instance.RegisterRestartable(this);
        }

        public void Restart() {
            GameManager.Instance.StartGame(this);
        }

        private void FixedUpdate() {
            if (fJumpTimer < fJumpTime) {
                fCurrentJumpForce = fJumpForce;
            } else {
                fCurrentJumpForce = 0;
            }

            float fGravity = Physics.gravity.y;
            if (IsOwner) {
                CharController.Move(new Vector3(vInputData.x * -fSpeed * Time.deltaTime,
                    (fCurrentJumpForce * Time.deltaTime) + (fGravity * Time.deltaTime),
                    vInputData.y * -fSpeed * Time.deltaTime));
            } else {
                Vector3 vData = Data.Value;
                CharController.Move(new Vector3(vData.x * -fSpeed * Time.deltaTime,
                    (fCurrentJumpForce * Time.deltaTime) + (fGravity * Time.deltaTime),
                    vData.y * -fSpeed * Time.deltaTime));
            }

            if (IsServer) {
                SendDataClientRpc(vInputData.x, vInputData.y);

                RunOnServer();
            } else {
                SendDataServerRpc(vInputData.x, vInputData.y);

                RunOnClient();
            }
        }

        private void RunOnServer() {
            SendPosClientRpc();


        }

        private void RunOnClient() {
            if (bSetPosition) {
                bSetPosition = false;
                fTimer = 0;
                return;
            }

            if (fTimer > 0.25f) {
                float fDistance = Vector3.Distance(CharController.transform.position, Position.Value);
                if (fDistance > 0.25f) {
                    fTimer = 0;
                    setPosition(Position.Value);
                }
            }
        }

        private void Update() {
            fTimer += Time.deltaTime;
            fJumpTimer += Time.deltaTime;

            if (IsOwner) {
                float fHor = Input.GetAxisRaw("Horizontal");
                float fVer = Input.GetAxisRaw("Vertical");

                vInputData = new Vector2(fHor, fVer);
            }

            if (this == GameManager.Instance.RealPlayer) {
                if (fJumpTimer > fJumpTime) {
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        Jump();
                    }
                }

                Interacting = false;
                if (Input.GetKeyDown(KeyCode.E)) {
                    Interact();
                }
            }

            if (!bSent) {
                bSent = true;

                GameManager gameManager = GameManager.Instance;
                SendClientVersionClientRpc(gameManager.ClientEthereal, gameManager.ClientLayerMask);
            }
        }

        private void Interact() {
            if (IsServer) {
                InteractClientRpc();
            } else {
                InteractServerRpc();
            }
        }

        [ClientRpc]
        public void InteractClientRpc() {
            Interacting = true;
        }

        [ServerRpc]
        public void InteractServerRpc() {
            Interacting = true;
        }

        private void Jump() {
            if (IsServer) {
                JumpClientRpc();
            } else {
                JumpServerRpc();
            }
        }

        [ClientRpc]
        public void JumpClientRpc() {
            fJumpTimer = 0;
        }

        [ServerRpc]
        public void JumpServerRpc() {
            fJumpTimer = 0;
        }

        [ServerRpc]
        private void SendDataServerRpc(float fHor, float fVer) {
            Data.Value = new Vector3(fHor, fVer);
        }

        [ClientRpc]
        private void SendDataClientRpc(float fHor, float fVer) {
            Data.Value = new Vector2(fHor, fVer);
        }

        [ClientRpc]
        private void SendPosClientRpc() {
            Position.Value = CharController.transform.position;
        }

        [ClientRpc]
        public void HidePlayerClientRpc() {
            HidePlayer();
        }

        [ClientRpc]
        public void ShowPlayerClientRpc(Vector3 vPos) {
            CharController.enabled = true;
            meshRenderer.enabled = true;
            SetPosition(vPos);
        }

        private void HidePlayer() {
            CharController.enabled = false;
            meshRenderer.enabled = false;
        }

        [ClientRpc]
        public void SendClientVersionClientRpc(bool bIsEtheral, int nLayerMask) {
            GameManager.Instance.ReceiveLayer(bIsEtheral, nLayerMask);
        }

        private void setPosition(Vector3 pos) {
            CharController.enabled = false;
            CharController.transform.position = pos;
            CharController.enabled = true;
        }

        public void SetPosition(Vector3 pos) {
            bSetPosition = true;

            if (!CharController) {
                CharController = GetComponent<CharacterController>();
            }

            CharController.enabled = false;
            CharController.transform.position = pos;
            CharController.enabled = true;
        }
    }
}
