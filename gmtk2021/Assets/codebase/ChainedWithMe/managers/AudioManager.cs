using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class AudioManager : MonoBehaviour {
        public AudioSource audioEthereal;
        public AudioSource audioReal;

        public float audioSpeed = 5;

        public AudioSource audioClick;

        private void Start() {
            audioEthereal.volume = 0;
            audioReal.volume = 0;
        }

        public void PlayClick() {
            audioClick.Play();
        }

        public void StartGame() {
            audioEthereal.Play();
            audioReal.Play();
        }

        private void Update() {
            if (GameManager.Instance.IsEthereal) {
                audioEthereal.volume = Math.Min(1, audioEthereal.volume + (audioSpeed * Time.deltaTime));
                audioReal.volume = Math.Max(0, audioReal.volume - (audioSpeed * Time.deltaTime));
            } else {
                audioReal.volume = Math.Min(1, audioReal.volume + (audioSpeed * Time.deltaTime));
                audioEthereal.volume = Math.Max(0, audioEthereal.volume - (audioSpeed * Time.deltaTime));
            }
        }
    }
}
