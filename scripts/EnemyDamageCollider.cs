using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class EnemyDamageCollider : MonoBehaviour
    {
        public EnemyStates eStates;
        CanvasUpdater canvas;

        void Start()
        {
            canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasUpdater>();
        }

        private void OnTriggerEnter(Collider other)
        {
            print(other.tag);
            if (other.gameObject.tag == "Player" && eStates)
        {
                canvas.updateCanvasWithEnemyDamageToPlayer();
            }
        }
    }
}