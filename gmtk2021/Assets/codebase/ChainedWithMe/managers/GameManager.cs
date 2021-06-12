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
        private bool bIsEthereal;

        private bool bIsFirst = true;

        private List<IRestartable> restartables;

        public NetworkPlayerComponent RealPlayer { get; private set; }
        public NetworkPlayerComponent EtherealPlayer { get; private set; }

        public bool IsEthereal {
            get { return bIsEthereal; }
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

            if (bIsEthereal) {
                RealPlayer = client.PlayerObject.GetComponent<NetworkPlayerComponent>();
            } else {
                EtherealPlayer = client.PlayerObject.GetComponent<NetworkPlayerComponent>();

                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++) {
                    NetworkClient objClient = NetworkManager.Singleton.ConnectedClientsList[i];
                    if (client == objClient) {
                        continue;
                    }

                    RealPlayer = objClient.PlayerObject.GetComponent<NetworkPlayerComponent>();
                    break;
                }
            }

            EtherealPlayer.HidePlayerClientRpc();
        }

        public void ReceiveLayer(bool bIsEthereal, int nLayerMask) {
            if (bIsHost) {
                return;
            }

            camera.cullingMask = nLayerMask;
            this.bIsEthereal = bIsEthereal;

            objOverlay.SetActive(false);
        }

        public void StartGame(bool bIsHost) {
            this.bIsHost = bIsHost;

            if (bIsHost) {
                bIsEthereal = Random.Range(0, 100) > 50;
                bIsEthereal = false;

                objOverlay.SetActive(false);

                if (bIsEthereal) {
                    camera.cullingMask = CameraManager.layerEthereal;
                } else {
                    camera.cullingMask = CameraManager.layerReal;
                }
            }
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

        private void OnGUI() {
            GUILayout.Label("Ethereal: " + bIsEthereal);
        }

        public void StartGame(NetworkPlayerComponent netPlayer) {
            if (bIsHost) {
                if (bIsFirst) {
                    bIsFirst = false;
                    if (bIsEthereal) {
                        EtherealPlayer = netPlayer;
                        EtherealPlayer.HidePlayerClientRpc();
                    } else {
                        RealPlayer = netPlayer;
                    }
                }
            }

            netPlayer.SetPosition(level.objSpawn.transform.position);
        }

    }
}