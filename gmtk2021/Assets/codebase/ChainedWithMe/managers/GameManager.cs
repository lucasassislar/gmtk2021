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

        void Start() {
            Instance = this;
            CameraManager = GetComponent<CameraManager>();

            objOverlay.SetActive(true);
        }

        public void ReceiveLayer(int nLayerMask) {
            if (bIsHost) {
                return;
            }

            camera.cullingMask = nLayerMask;

            objOverlay.SetActive(false);
        }

        public void StartGame(bool bIsHost) {
            this.bIsHost = bIsHost;

            if (bIsHost) {
                objOverlay.SetActive(false);

                bool bRandom = Random.Range(0, 100) > 50;

                if (bRandom) {
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
        }

        public void StartGame(NetworkPlayerComponent netPlayer) {
            netPlayer.SetClientLayer(layerMaskB);
            netPlayer.SetPosition(level.objSpawn.transform.position);
        }

    }
}