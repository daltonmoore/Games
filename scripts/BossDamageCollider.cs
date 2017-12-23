using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class BossDamageCollider : MonoBehaviour
    {
        StateManager states;
        CanvasUpdater canvas;
        
        void Start()
        {
            states = GameObject.FindGameObjectWithTag("Player").GetComponent<StateManager>();
            canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasUpdater>();
        }

        private void OnTriggerEnter(Collider other)
        {
            print(other.tag);
            if (other.gameObject.tag == "PlayerHit")
            {
                if (!states.isBlocking)
                {
                    states.anim.Play("damage_1");
                    canvas.updateCanvasWithBossDamageToPlayer();
                }
                else
                {
                    states.stamina -= 55;
                    if (states.stamina <= 0)
                    {
                        states.anim.Play("attack_interrupt");
                        states.isBlocking = false;
                        states.stamina_break = true;
                        states.stamina_breakTimer = Time.time;
                    }
                    canvas.updateStaminaBar();
                }
            }
        }
    }
}