using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class SwitchBetweenEnemies : MonoBehaviour
    {
        public Collider sphere;

        StateManager states;
        //CameraManager camManager;
        GameObject player;
        List<GameObject> enemies = new List<GameObject>();

        bool rightAxisDown, usedRightAxis;
        int index = 0;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            //camManager = CameraManager.singleton;
            states = player.GetComponent<StateManager>();
        }

        private void Update()
        {
            rightAxisDown = Input.GetButtonUp(StaticStrings.L);
            float c_v = Input.GetAxis("RightAxis Y");
            float c_h = Input.GetAxis("RightAxis X");
            if (enemies.Count > 0 && rightAxisDown)
            {
                states.lockOn = !states.lockOn;

                states.lockOnTarget = EnemyManager.singleton.GetEnemy(transform.position);
                if (states.lockOnTarget == null)
                    states.lockOn = false;

                //camManager.lockOnTarget = states.lockOnTarget;
                states.lockOnTransform = states.lockOnTarget.GetTarget();
                //camManager.lockonTransform = states.lockOnTransform;
                //camManager.lockon = states.lockOn;
            }
            if (enemies.Count > 1 && states.lockOn)
            {
                if (Mathf.Abs(c_h) > .6f)
                {
                    if (!usedRightAxis)
                    {
                        index++;
                        if (index > enemies.Count)
                            index = 0;
                        //camManager.lockOnTarget = enemies[index].GetComponent<EnemyTarget>();
                        states.lockOnTransform = enemies[index].transform;
                        //camManager.lockonTransform = states.lockOnTransform;
                        //camManager.lockon = states.lockOn;
                        usedRightAxis = true;
                    }
                }
            }
            if (usedRightAxis)
            {
                if (Mathf.Abs(c_h) < .6f)
                {
                    usedRightAxis = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy" || other.tag == "Boss")
            {
                enemies.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Enemy" || other.tag == "Boss")
            {
                enemies.Remove(other.gameObject);
            }
        }
    }
}