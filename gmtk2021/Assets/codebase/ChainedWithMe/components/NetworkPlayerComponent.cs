using ChainedWithMe.Level;
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

        private AudioSource audioSource;

        private Vector2 vInputData;
        private float fTimer;

        private float fJumpTimer;

        private bool bSent;
        private float fCurrentJumpForce;


        public CharacterController CharController { get; private set; }

        public AudioClip clipJump;

        public NetworkVariableVector2 Data = new NetworkVariableVector2(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        private PlayerTriggerComponent playerTrigger;

        public void SetInside(PlayerTriggerComponent inside) {
            this.playerTrigger = inside;
        }

        public void SetInsideOut(PlayerTriggerComponent inside) {
            if (this.playerTrigger == inside) {
                this.playerTrigger = null;
            }
        }

        private Vector3 vStartPos;

        private void Start() {
            CharController = GetComponentInChildren<CharacterController>();

            setPosition(vStartPos);

            GameManager.Instance.StartGame(this);

            GameManager.Instance.RegisterRestartable(this);

            audioSource = GetComponentInChildren<AudioSource>();
        }

        public void Restart() {
            GameManager.Instance.StartGame(this);

            this.playerTrigger = null;
        }

        private void FixedUpdate() {
            if (fJumpTimer < fJumpTime) {
                fCurrentJumpForce = fJumpForce;
            } else {
                fCurrentJumpForce = 0;
            }

            float fGravity = Physics.gravity.y;

            if (this == GameManager.Instance.EtherealPlayer) {

            } else if (this == GameManager.Instance.RealPlayer) {
                if(IsOwner){
                    Vector3 vToAdd = new Vector3();
                    CharController.Move(new Vector3(
                           (vInputData.x * -fSpeed * Time.deltaTime) + vToAdd.x,
                           (fCurrentJumpForce * Time.deltaTime) + (fGravity * Time.deltaTime) + vToAdd.y,
                            (vInputData.y * -fSpeed * Time.deltaTime) + vToAdd.z));

                    SendData();

                    SendPosition();
                } else {
                    Debug.LogError("REAL PLAYER WAT");

                    SetPosition(Position.Value);
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

                if (this == GameManager.Instance.EtherealPlayer) {
                    if (fJumpTimer > fJumpTime) {
                        if (Input.GetKeyDown(KeyCode.Space)) {
                            Jump();
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.E)) {
                        Interact();
                    }
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
            if (this.playerTrigger == null) {
                return;
            }

            this.playerTrigger.Interact();
        }

        [ServerRpc]
        public void InteractServerRpc() {
            if (this.playerTrigger == null) {
                return;
            }

            this.playerTrigger.Interact();
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
            GameManager.Instance.RealPlayer.fJumpTimer = 0;

            audioSource.clip = clipJump;
            audioSource.Play();
        }

        [ServerRpc]
        public void JumpServerRpc() {
            GameManager.Instance.RealPlayer.fJumpTimer = 0;

            audioSource.clip = clipJump;
            audioSource.Play();
        }

        private void SendData() {
            if (IsServer) {
                SendDataClientRpc();
            } else {
                SendDataServerRpc();
            }
        }

        [ServerRpc]
        private void SendDataServerRpc() {
            Data.Value = new Vector2(vInputData.x, vInputData.y);
        }

        [ClientRpc]
        private void SendDataClientRpc() {
            Data.Value = new Vector2(vInputData.x, vInputData.y);
        }

        private void SendPosition() {
            Position.Value = CharController.transform.position;

            //if (IsServer) {
            //    SendPosClientRpc();
            //} else {
            //    SendPosServerRpc();
            //}
        }

        [ServerRpc]
        private void SendPosServerRpc() {
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
            Position.Value = CharController.transform.position;

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
            if (!CharController) {
                CharController = GetComponent<CharacterController>();
            }
            if (!CharController) {
                vStartPos = pos;
                return;
            }

            setPosition(pos);
        }
    }
}
