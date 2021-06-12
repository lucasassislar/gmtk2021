using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class EnemyComponent : NetworkBehaviour {
        [ClientRpc]
        public void DieClientRpc() {
            gameObject.SetActive(false);
        }
    }
}
