using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainedWithMe {
    public class NetworkGameManager : NetworkBehaviour {
        public static NetworkGameManager Instance { get; private set; }

        private void Start() {
            Instance = this;
        }

        [ClientRpc]
        public void SwapViewClientRpc() {
            GameManager.Instance.ClientSwapView();
            
        }
    }
}
