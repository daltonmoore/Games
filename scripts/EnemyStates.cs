using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class EnemyStates : MonoBehaviour
    {
        public int health;
        public int defence = 30;
        public int damage = 100;
        float speed = 3;

        public bool canBeParried = true;
        public bool parryIsOn = true;
        public bool isInvicible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;
        bool inRange;

        public Animator anim;
        AnimatorHook a_hook;
        public BoxCollider attackRange;
        protected EnemyTarget enTarget;
        public Rigidbody rigid;
        public float delta;
        public float poiseDegrade = 2;
        public StateManager states;

        public GameObject damageCollider;

        protected List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        protected List<Collider> ragdollColliders = new List<Collider>();

        protected float timer = 0;

        protected void Start()
        {
            //damageCollider.SetActive(false);
            //health = 300;
            defence = 30;
            damage = 100;
            anim = GetComponentInChildren<Animator>();
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(this);

            rigid = GetComponent<Rigidbody>();
            rigid.drag = 30;
            a_hook = anim.gameObject.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();
            a_hook.Init(null, this);

            InitRagDoll();
            parryIsOn = false;
            attackRange = GetComponent<BoxCollider>();
        }

        public void OpenDamageCollider()
        {
            damageCollider.SetActive(true);
        }

        public void CloseDamageCollider()
        {
            damageCollider.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
                inRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
                inRange = false;
        }

        protected void InitRagDoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == rigid)
                    continue;

                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagDoll()
        {
            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
            }

            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            rigid.isKinematic = true;

            StartCoroutine("CloseAnimator");
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
            this.enabled = false;
        }

        private void Update()
        {
            transform.LookAt(states.gameObject.transform);
            delta = Time.deltaTime;
            //canMove = anim.GetBool(StaticStrings.canMove);

            if(dontDoAnything)
            {
                dontDoAnything = !canMove;

                return;
            }
            if(health <= 0)
            {
                if(!isDead)
                {
                    isDead = true;
                    canBeParried = false;
                    EnableRagDoll();
                }
            }

            //if (isInvicible)
            // {
            //     isInvicible = !canMove;
            // }
            //if (canMove)
            // {
            /*parryIsOn = false;
            anim.applyRootMotion = false;

            timer += Time.deltaTime;
            if (timer > 3)
            {
                DoAction();
                timer = 0;
            }*/
            //}
            if (states.health > 0)
            {
                if (inRange)
                {
                    timer = Time.time;
                    anim.Play("oh_attack_1");
                }
                if (!inRange)
                {
                    if (timer + 1f < Time.time)
                    {
                        transform.LookAt(states.gameObject.transform);
                        float step = speed * Time.deltaTime;
                        transform.position = Vector3.MoveTowards(transform.position, states.gameObject.transform.position, step);
                        anim.Play("Locomotion Normal");
                        anim.SetFloat("vertical", 1);
                    }
                }
            }
            else
            {
                anim.Play("point down");
            }
            /*characterStats.poise -= delta * poiseDegrade;
            if (characterStats.poise < 0)
                characterStats.poise = 0;*/
        }

        void DoAction()
        {
            anim.Play("oh_attack_1");
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
        }

        public void ReceiveDamage(Action a)
        {
            if (isInvicible)
                return;
            /*int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats);

            characterStats.poise += damage;
            health -= damage;

            if(canMove || characterStats.poise > 100)
            {
                if (a.overideDamageAnim)
                    anim.Play(a.damageAnim);
                else
                {
                    int ran = Random.Range(0, 100);
                    string tA = (ran > 50) ? StaticStrings.damage1 : StaticStrings.damage2;
                    anim.Play(tA);
                }
            }*/

            if(canMove)
            {
                anim.Play(StaticStrings.damage1);
            }
            health -= states.damage - defence;
            isInvicible = true;
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
        }

        public void CheckForParry(Transform target, StateManager states)
        {
            if (canBeParried == false || parryIsOn == false || isInvicible)
                return;

            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            if (dot < 0)
                return;

            isInvicible = true;
            anim.Play(StaticStrings.attack_interrupt);
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
            //states.parryTarget = this;
            this.states = states;
            return;
        }

        public void IsGettingParried(Action a)
        {
            //int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.parryMultiplier);
            health -= damage+50;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.parry_received);
        }

        public void IsGettingBackstabbed(Action a)
        {
            //int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.backstabMultiplier);
            health -= damage+100;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.backstabbed);
        }

        public float doDamageToPlayer()
        {
            if (states.isBlocking)
                return states.health;
            states.health -= damage - states.defence;
            return states.health;
        }
    }
}