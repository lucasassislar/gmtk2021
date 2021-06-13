﻿using MLAPI;
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
        public float fAttackTime = 0.5f;

        public MeshRenderer meshRenderer;

        private Vector3 vInputData;
        private float fTimer;

        private float fAttackTimer;
        private float fAttack;

        private bool bSent;
        private bool bSetPosition;

        public CharacterController CharController { get; private set; }

        public NetworkVariableVector3 Data = new NetworkVariableVector3(new NetworkVariableSettings {
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
            fAttackTimer += Time.deltaTime;
            fAttack -= Time.deltaTime * 10000;
            fAttack = Math.Max(0, fAttack);

            float fGravity = Physics.gravity.y;

            if (IsServer) {
                SendDataClientRpc(vInputData.x, vInputData.y, vInputData.z);
            } else {
                SendDataServerRpc(vInputData.x, vInputData.y, vInputData.z);
            }

            if (IsOwner) {
                CharController.SimpleMove(new Vector3(vInputData.x * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, vInputData.y * -fSpeed * Time.deltaTime));
            } else {
                Vector3 vData = Data.Value;
                CharController.SimpleMove(new Vector3(vData.x * -fSpeed * Time.deltaTime, fGravity * Time.deltaTime, vData.y * -fSpeed * Time.deltaTime));
            }

            if (IsServer) {
                RunOnServer();
            } else {
                RunOnClient();
            }
        }

        private void RunOnServer() {
            SendPosClientRpc();

            //Vector3 vData = Data.Value;
            //if (vData.z > 0) {
            //    fAttackTimer = 0;

            //    List<BodyComponent> bodies = objBodyList.bodies;
            //    if (bodies.Count == 0) {
            //        return;
            //    }

            //    inside = bodies[0];
            //    GameManager.Instance.EnterBody(inside);

            //    inside.EnterClientRpc();
            //    HidePlayer();
            //}
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

            if (IsOwner) {
                float fHor = Input.GetAxisRaw("Horizontal");
                float fVer = Input.GetAxisRaw("Vertical");

                if (Input.GetKeyDown(KeyCode.Return)) {
                    fAttack = 0.05f * 10000;
                }

                vInputData = new Vector3(fHor, fVer, fAttack);
            }

            if (!bSent) {
                bSent = true;

                GameManager gameManager = GameManager.Instance;
                SendClientVersionClientRpc(gameManager.ClientEthereal, gameManager.ClientLayerMask);
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
