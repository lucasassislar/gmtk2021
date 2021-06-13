using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainedWithMe.System {
    public class MenuLoadComponent : MonoBehaviour {

        public void LoadMenu() {
            SceneManager.LoadScene("MainMenu");
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                LoadMenu();
            }
        }
    }
}
