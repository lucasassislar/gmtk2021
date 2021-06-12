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
            EnemyComponent enemyComp = other.GetComponent<EnemyComponent>();
            if (enemyComp == null) {
                return;
            }

            enemies.Add(enemyComp);
        }

        private void OnTriggerExit(Collider other) {
            EnemyComponent enemyComp = other.GetComponent<EnemyComponent>();
            if (enemyComp == null) {
                return;
            }

            enemies.Remove(enemyComp);
        }
    }
}
