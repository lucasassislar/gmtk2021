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

        private bool bIsHost;
        private bool bIsLegs;

        private bool bIsFirst = true;

        private List<IRestartable> restartables;

        public NetworkPlayerComponent LegsPlayer { get; private set; }
        public NetworkPlayerComponent ArmsPlayer { get; private set; }

        public bool IsLegs {
            get { return bIsLegs; }
        }

        void Start() {
            Instance = this;
            CameraManager = GetComponent<CameraManager>();

            objOverlay.SetActive(true);

            restartables = new List<IRestartable>();

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

            //ArmsPlayer.HidePlayerClientRpc();
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
                bIsLegs = Random.Range(0, 100) > 50;
            }

            camera.cullingMask = CameraManager.layerGhost;
            objOverlay.SetActive(false);
        }

        public void RegisterRestartable(IRestartable restartable) {
            restartables.Add(restartable);
        }

        public void End() {
            objOverlay.SetActive(true);
            Restart();
        }

        public void Restart() {
            bIsFirst = true;

            for (int i = 0; i < restartables.Count; i++) {
                restartables[i].Restart();
            }
        }

        public void EnterBody(BodyComponent body) {
            int totalInside = body.TotalInside.Value;

            if (totalInside == 0) {
                camera.cullingMask = CameraManager.layerEnemies;
            } else if (totalInside == 1) {
                camera.cullingMask = CameraManager.layerWalls;
            }
        }

        public void StartGame(NetworkPlayerComponent netPlayer) {
            if (bIsHost) {
                if (bIsFirst) {
                    bIsFirst = false;
                    if (bIsLegs) {
                        LegsPlayer = netPlayer;
                    } else {
                        ArmsPlayer = netPlayer;
                        //ArmsPlayer.HidePlayerClientRpc();
                    }
                }
            }

            netPlayer.SetPosition(level.objSpawn.transform.position);
        }

    }
}