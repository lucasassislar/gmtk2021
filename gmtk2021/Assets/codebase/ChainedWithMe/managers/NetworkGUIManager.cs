using MLAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class NetworkGUIManager : MonoBehaviour {
        static void StartButtons() {
            if (GUILayout.Button("Host")) {
                GameManager.Instance.StartGame(true);
                NetworkManager.Singleton.StartHost();
            }
            if (GUILayout.Button("Client")) {
                GameManager.Instance.StartGame(false);
                NetworkManager.Singleton.StartClient();
            }
            if (GUILayout.Button("Server")) {
                GameManager.Instance.StartGame(true);
                NetworkManager.Singleton.StartServer();
            }

        }

        static void StatusLabels() {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
                StartButtons();
            } else {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        static void SubmitNewPosition() {
            if (GUILayout.Button("Stop")) {
                GameManager.Instance.End();
                NetworkManager.Singleton.Shutdown();
            }
        }
    }
}
