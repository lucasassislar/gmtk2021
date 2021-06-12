using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class CameraManager : MonoBehaviour, IRestartable {
        public LayerMask layerEthereal;
        public LayerMask layerReal;

        public Camera objCamera;

        public CinemachineVirtualCamera objVirtualCamera;

        //public GameObject objRealCameraPOV;
        //public GameObject objRealCameraPOV_Target;

        private bool bFirstFrame;

        public void Restart() {
            bFirstFrame = false;
        }

        private void LateUpdate() {
            GameManager manager = GameManager.Instance;

            if (manager.IsEthereal) {

            } else {
                if (manager.RealPlayer == null) {
                    return;
                }

                objVirtualCamera.Follow = manager.RealPlayer.CharController.transform;
                objVirtualCamera.LookAt = manager.RealPlayer.CharController.transform;

                //Quaternion rot = objRealCameraPOV.transform.localRotation;
                //objCamera.transform.localRotation = rot;

                //Vector3 vDir = objRealCameraPOV.transform.position - objRealCameraPOV_Target.transform.position;
                //Vector3 vPos = manager.RealPlayer.CharController.transform.position;

                //Vector3 vPoint = objCamera.WorldToViewportPoint(vPos);

                //float fLeftDistance = vPoint.x;
                //if (fLeftDistance <= 0.4f) {
                //    Vector3 vNewCameraPos = vPos + vDir;
                //    Vector3 vNewPos = vNewCameraPos - vDir;

                //    // get new point 
                //    Vector3 vPointNew = objCamera.WorldToViewportPoint(vNewPos);

                //    float fLeftDistanceNew = vPointNew.x;
                //    float fLeftDifference = fLeftDistance - fLeftDistanceNew;

                //    Vector3 vDirNew = vPoint - vNewPos;

                //    objCamera.transform.position = vNewPos + (vDirNew * fLeftDifference);
                //}

                //if (!bFirstFrame) {
                //    bFirstFrame = true;
                //    objCamera.transform.position = vPos + vDir;
                //}
            }
        }
    }
}
