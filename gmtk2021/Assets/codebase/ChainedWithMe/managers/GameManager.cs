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
            this.player.SetPosition(level.objSpawn.transform.position);
        }

        public void RestartGame() {
            StartGame();
        }

        void Update() {

        }
    }
}