using MLAPI;
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

        void Start() {
            Instance = this;
            CameraManager = GetComponent<CameraManager>();

            StartGame();
            objOverlay.SetActive(true);
        }

        public void ReceiveLayer(int nLayerMask) {

        }

        public void StartGame(bool isHost) {
            objOverlay.SetActive(false);

            if (isHost) {
                bool bRandom = Random.Range(0, 100) > 50;

                if (bRandom) {
                    camera.cullingMask = CameraManager.layerA;
                } else {
                    camera.cullingMask = CameraManager.layerB;
                }
            }
        }

        public void RestartGame() {
            StartGame();
        }

        public void StartGame(NetworkPlayerComponent netPlayer) {
            netPlayer.SetPosition(level.objSpawn.transform.position);
        }

    }
}