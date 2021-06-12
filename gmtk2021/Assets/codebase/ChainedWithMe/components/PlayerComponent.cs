using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class PlayerComponent : MonoBehaviour {
        private void OnControllerColliderHit(ControllerColliderHit hit) {
            int nLayer = hit.collider.gameObject.layer;
            if (nLayer == LayerMask.NameToLayer("Enemy") ||
                nLayer == LayerMask.NameToLayer("Wall") ||
                nLayer == LayerMask.NameToLayer("OuterWall")) {
                GameManager.Instance.Restart();
            }
        }
    }
}
