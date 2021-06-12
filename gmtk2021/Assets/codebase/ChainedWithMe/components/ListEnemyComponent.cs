using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainedWithMe {
    public class ListEnemyComponent : MonoBehaviour {
        public List<EnemyComponent> enemies;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) {
                return;
            }

            EnemyComponent enemyComp = other.GetComponent<EnemyComponent>();
            enemies.Add(enemyComp);
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) {
                return;
            }

            EnemyComponent enemyComp = other.GetComponent<EnemyComponent>();
            enemies.Remove(enemyComp);
        }
    }
}
