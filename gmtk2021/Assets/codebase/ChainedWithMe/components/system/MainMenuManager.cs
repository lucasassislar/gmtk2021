using MLAPI;
using MLAPI.Transports.UNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChainedWithMe.System {
    public class MainMenuManager : MonoBehaviour {
        public InputField fieldIP;

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
