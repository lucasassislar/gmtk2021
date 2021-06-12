using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainedWithMe {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        public LevelComponent level;

        public PlayerComponent player;

        void Start() {
            Instance = this;

            StartGame();
        }

        public void StartGame() {
            
        }

        public void RestartGame() {
            StartGame();
        }

        public void StartGame(NetworkPlayerComponent netPlayer) {
            netPlayer.SetPosition(level.objSpawn.transform.position);
        }

    }
}