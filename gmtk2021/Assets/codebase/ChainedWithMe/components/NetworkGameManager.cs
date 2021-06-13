using MLAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainedWithMe {
    public class NetworkGameManager : MonoBehaviour {
        private void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        }

        private void Singleton_OnClientDisconnectCallback(ulong obj) {
            if (NetworkManager.Singleton.IsServer) {
                NetworkManager.Singleton.StopServer();
            }

            if (NetworkManager.Singleton.IsClient) {
                NetworkManager.Singleton.StopClient();
            }

            SceneManager.LoadScene("MainMenu");
        }

        private void Singleton_OnClientConnectedCallback(ulong obj) {
        }
    }
}
