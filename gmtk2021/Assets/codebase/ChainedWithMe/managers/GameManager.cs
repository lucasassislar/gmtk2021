using MLAPI;
using MLAPI.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        public CameraManager CameraManager { get; private set; }

        public LevelComponent level;

        public Camera camera;

        public GameObject objOverlay;

        private int layerMaskB;

        private bool bIsHost;
        private bool bIsLegs;

        public bool IsLegs {
            get { return bIsLegs; }
        }

        public int ClientLayerMask {
            get { return this.layerMaskB; }
        }

        public NetworkPlayerComponent LegsPlayer { get; private set; }
        public NetworkPlayerComponent ArmsPlayer { get; private set; }

        void Start() {
            Instance = this;
            CameraManager = GetComponent<CameraManager>();

            objOverlay.SetActive(true);

            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        }

        private void Singleton_OnClientConnectedCallback(ulong obj) {
            if (!bIsHost) {
                return;
            }

            NetworkClient client = NetworkManager.Singleton.ConnectedClients[obj];

            if (bIsLegs) {
                ArmsPlayer = client.PlayerObject.GetComponent<NetworkPlayerComponent>();
            } else {
                LegsPlayer = client.PlayerObject.GetComponent<NetworkPlayerComponent>();

                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++) {
                    NetworkClient objClient = NetworkManager.Singleton.ConnectedClientsList[i];
                    if (client == objClient) {
                        continue;
                    }

                    ArmsPlayer = objClient.PlayerObject.GetComponent<NetworkPlayerComponent>();
                    break;
                }
            }

            ArmsPlayer.HidePlayerClientRpc();
        }

        public void ReceiveLayer(bool bIsLegs, int nLayerMask) {
            if (bIsHost) {
                return;
            }

            camera.cullingMask = nLayerMask;
            this.bIsLegs = bIsLegs;

            objOverlay.SetActive(false);
        }

        public void StartGame(bool bIsHost) {
            this.bIsHost = bIsHost;

            if (bIsHost) {
                objOverlay.SetActive(false);

                bIsLegs = Random.Range(0, 100) > 50;

                if (bIsLegs) {
                    camera.cullingMask = CameraManager.layerA;
                    layerMaskB = CameraManager.layerB;
                } else {
                    camera.cullingMask = CameraManager.layerB;
                    layerMaskB = CameraManager.layerA;
                }

                Debug.Log($"Camera: {camera.cullingMask} Side B: {layerMaskB}");
            }
        }

        public void Restart() {
            objOverlay.SetActive(true);
            bIsFirst = true;
        }

        private bool bIsFirst = true;

        public void StartGame(NetworkPlayerComponent netPlayer) {
            if (bIsHost) {
                if (bIsFirst) {
                    bIsFirst = false;
                    if (bIsLegs) {
                        LegsPlayer = netPlayer;
                    } else {
                        ArmsPlayer = netPlayer;
                        ArmsPlayer.HidePlayerClientRpc();
                    }
                }
            }

            netPlayer.SetPosition(level.objSpawn.transform.position);
        }

    }
}