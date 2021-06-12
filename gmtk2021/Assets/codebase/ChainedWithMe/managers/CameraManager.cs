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

        public GameObject objRealCameraPOV;
        public GameObject objRealCameraPOV_Target;

        private void Update() {
            GameManager manager = GameManager.Instance;

            if (manager.IsEthereal) {

            } else {
                if (manager.RealPlayer == null) {
                    return;
                }

                Quaternion rot = objRealCameraPOV.transform.localRotation;

                Vector3 dist = objRealCameraPOV.transform.position - objRealCameraPOV_Target.transform.position;
                Vector3 pos = manager.RealPlayer.CharController.transform.position;

                objCamera.transform.position = pos + dist;
                objCamera.transform.localRotation = rot;
            }
        }
    }
}
