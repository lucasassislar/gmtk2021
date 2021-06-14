using MLAPI;
using MLAPI.Transports.UNET;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChainedWithMe.System {
    public class MainMenuManager : MonoBehaviour {
        public Button showPublicIP;
        public Text publicIP;

        public InputField fieldIP;

        public string strIP = "0.0.0.0";

        private void Start() {
            var discoverer = new NatDiscoverer();
            discoverer.DiscoverDeviceAsync().ContinueWith(device => {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                device.Result.CreatePortMapAsync(new Mapping(Protocol.Udp, 52000, 52000, "Guide Me Please (UDP)"));

                device.Result.GetExternalIPAsync().ContinueWith(ip => {                    
                    strIP = ip.Result.ToString();
                });
            });
        }

        private void Update() {
            publicIP.text = strIP;
        }

        public void ShowPublicIP() {
            showPublicIP.gameObject.SetActive(false);
        }

        public void Host() {
            Globals.IsHost = true;

            SceneManager.LoadScene("ChainedWithMe");
        }

        public void Connect() {
            Globals.IsHost = false;

            UNetTransport unet = NetworkManager.Singleton.GetComponent<UNetTransport>();

            string ipAddress = fieldIP.text;
            unet.ConnectAddress = ipAddress;


            SceneManager.LoadScene("ChainedWithMe");
        }
    }
}
