using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class VolumePlusComponent : MonoBehaviour {
        public float fSpeed = 1;

        private AudioSource audioSource;

        private void Start() {
            audioSource = GetComponent<AudioSource>();
        }

        public void Update() {
            float fVolume = audioSource.volume;

            fVolume += fSpeed * Time.deltaTime;
            fVolume = Math.Min(1, fVolume);

            audioSource.volume = fVolume;
        }
    }
}
