using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class CameraManager : MonoBehaviour {
        public LayerMask layerEthereal;
        public LayerMask layerReal;

        public Camera objCamera;

        public CinemachineVirtualCamera objVirtualCamera;

        private void LateUpdate() {
            GameManager manager = GameManager.Instance;

            if (manager.RealPlayer == null) {
                return;
            }

            objVirtualCamera.Follow = manager.RealPlayer.CharController.transform;
            objVirtualCamera.LookAt = manager.RealPlayer.CharController.transform;
        }
    }
}
