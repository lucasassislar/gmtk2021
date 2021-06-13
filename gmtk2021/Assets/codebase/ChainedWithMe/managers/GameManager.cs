using MLAPI;
using MLAPI.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainedWithMe {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        public CameraManager CameraManager { get; private set; }

        public LevelComponent level;

        public Camera camera;

        public GameObject objOverlay;
        public GameObject objKillBox;

        private bool bIsHost;
        private bool bIsEthereal;

        private bool bIsFirst = true;

        private List<IRestartable> restartables;

        public NetworkPlayerComponent RealPlayer { get; private set; }
        public NetworkPlayerComponent EtherealPlayer { get; private set; }
        public AudioManager AudioManager { get; private set; }

        public int ClientLayerMask { get; private set; }
        public bool ClientEthereal { get { return !bIsEthereal; } }

        public List<NetworkPlayerComponent> Players { get; private set; }

        public AudioClip clipGoing;
        public AudioClip clipComing;

        private AudioSource viewSource;

        public bool IsEthereal {
            get { return bIsEthereal; }
        }

        private void UpdateLayer() {
            if (bIsEthereal) {
                camera.cullingMask = CameraManager.layerEthereal;
                ClientLayerMask = CameraManager.layerReal;
            } else {
                camera.cullingMask = CameraManager.layerReal;
                ClientLayerMask = CameraManager.layerEthereal;
            }
        }

        public void SwapView(AudioSource source) {
            viewSource = source;
            if (!NetworkManager.Singleton.IsHost) {
                return;
            }

            List<NetworkPlayerComponent> netPlayers = Players;

            bIsEthereal = !bIsEthereal;
            UpdateLayer();

            Vector3 vPos = RealPlayer.CharController.transform.position;

            EtherealPlayer = null;
            RealPlayer = null;

            for (int i = 0; i < netPlayers.Count; i++) {
                NetworkPlayerComponent player = netPlayers[i];

                if (player.IsOwner) {
                    if (bIsEthereal) {
                        EtherealPlayer = player;
                    } else {
                        RealPlayer = player;
                    }
                } else {
                    player.SendClientVersionClientRpc(ClientEthereal, ClientLayerMask);

                    if (bIsEthereal) {
                        RealPlayer = player;
                    } else {
                        EtherealPlayer = player;
                    }
                }
            }

            EtherealPlayer.gameObject.name = "EtherealPlayer";
            RealPlayer.gameObject.name = "RealPlayer";

            if (EtherealPlayer != null) {
                EtherealPlayer.HidePlayerClientRpc();
            }

            if (RealPlayer != null) {
                RealPlayer.ShowPlayerClientRpc(vPos);
            }

            if (bIsEthereal) {
                source.clip = clipGoing;
            } else {
                source.clip = clipComing;
            }
            source.Play();
        }

        private void Start() {
            Players = new List<NetworkPlayerComponent>();

            Instance = this;
            CameraManager = GetComponent<CameraManager>();
            AudioManager = GetComponentInChildren<AudioManager>();

            objOverlay.SetActive(true);

            restartables = new List<IRestartable>();

            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;

            if (Globals.IsHost) {
                StartGame(true);
                NetworkManager.Singleton.StartHost();
            } else {
                StartGame(false);
                NetworkManager.Singleton.StartClient();
            }
        }

        private void OnDestroy() {
            if (NetworkManager.Singleton != null) {
                NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
            }
        }

        private void Update() {
            if (RealPlayer != null) {
                if (RealPlayer.CharController.transform.position.y < objKillBox.transform.position.y) {
                    // death
                    Restart();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                End();

                if (NetworkManager.Singleton.IsServer) {
                    NetworkManager.Singleton.StopServer();
                }

                if (NetworkManager.Singleton.IsClient) {
                    NetworkManager.Singleton.StopClient();
                }

                SceneManager.LoadScene("MainMenu");
            }
        }

        private void Singleton_OnClientConnectedCallback(ulong obj) {
            if (!bIsHost) {
                return;
            }

            NetworkClient client = NetworkManager.Singleton.ConnectedClients[obj];

            if (bIsEthereal) {
                RealPlayer = client.PlayerObject.GetComponent<NetworkPlayerComponent>();
                RealPlayer.SetPosition(level.objSpawn.transform.position);

            } else {
                EtherealPlayer = client.PlayerObject.GetComponent<NetworkPlayerComponent>();
                EtherealPlayer.SetPosition(level.objSpawn.transform.position);

                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++) {
                    NetworkClient objClient = NetworkManager.Singleton.ConnectedClientsList[i];
                    if (client == objClient) {
                        continue;
                    }

                    RealPlayer = objClient.PlayerObject.GetComponent<NetworkPlayerComponent>();
                    break;
                }
            }

            EtherealPlayer.gameObject.name = "EtherealPlayer";
            RealPlayer.gameObject.name = "RealPlayer";

            EtherealPlayer.HidePlayerClientRpc();
        }

        public void ReceiveLayer(bool bIsEthereal, int nLayerMask) {
            if (bIsHost) {
                return;
            }

            for (int i = 0; i < Players.Count; i++) {
                NetworkPlayerComponent netPlayer = Players[i];

                if (netPlayer.IsOwner) {
                    if (bIsEthereal) {
                        EtherealPlayer = netPlayer;
                    } else {
                        RealPlayer = netPlayer;
                    }
                } else {
                    if (bIsEthereal) {
                        RealPlayer = netPlayer;
                    } else {
                        EtherealPlayer = netPlayer;
                    }
                }
            }

            EtherealPlayer.gameObject.name = "EtherealPlayer";
            RealPlayer.gameObject.name = "RealPlayer";

            camera.cullingMask = nLayerMask;
            this.bIsEthereal = bIsEthereal;

            objOverlay.SetActive(false);

            if (viewSource != null) {
                if (bIsEthereal) {
                    viewSource.clip = clipGoing;
                } else {
                    viewSource.clip = clipComing;
                }
                viewSource.Play();
            }
        }

        public void StartGame(bool bIsHost) {
            this.bIsHost = bIsHost;

            if (bIsHost) {
                bIsEthereal = Random.Range(0, 100) > 50;
                bIsEthereal = true;

                objOverlay.SetActive(false);

                UpdateLayer();
            }

            AudioManager.StartGame();
        }

        public void RegisterRestartable(IRestartable restartable) {
            restartables.Add(restartable);
        }

        public void End() {
            objOverlay.SetActive(true);
            Restart();
        }

        public void Restart() {
            Players.Clear();

            bIsFirst = true;

            for (int i = 0; i < restartables.Count; i++) {
                restartables[i].Restart();
            }
        }

        private void OnGUI() {
            GUILayout.Label("Ethereal: " + bIsEthereal);
        }

        public void StartGame(NetworkPlayerComponent netPlayer) {
            Players.Add(netPlayer);

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