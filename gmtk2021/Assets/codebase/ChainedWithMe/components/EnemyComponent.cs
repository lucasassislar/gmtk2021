using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class EnemyComponent : NetworkBehaviour, IRestartable {
        [ClientRpc]
        public void DieClientRpc() {
            gameObject.SetActive(false);
        }

        private void Start() {
            GameManager.Instance.RegisterRestartable(this);
        }

        public void Restart() {
            gameObject.SetActive(true);
        }
    }
}
