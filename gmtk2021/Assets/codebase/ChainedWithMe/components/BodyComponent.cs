using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe { 
    public class BodyComponent : NetworkBehaviour {
        public CharacterController CharController { get; private set; }

        public NetworkVariableInt TotalInside = new NetworkVariableInt(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        [ClientRpc]
        public void EnterClientRpc() {
            TotalInside.Value = TotalInside.Value + 1;
        }

        [ClientRpc]
        public void ExitClientRpc() {
            TotalInside.Value = TotalInside.Value - 1;
        }

        private void Start() {
            CharController = GetComponent<CharacterController>();
        }
    }
}
