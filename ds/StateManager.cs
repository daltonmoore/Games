using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        public Attributes attributes;
        public WeaponStats rHandWeapon = new WeaponStats();
        public BoxCollider bossDoor;
        public GameObject healingItem;
        public GameObject pauseOverlay;

        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;

        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5f;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        public float parryOffset = 1.4f;
        public float backstabOffset = 1.4f;

        [Header("States")]
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;
        public bool usingItem;
        public bool canBeParried;
        public bool parryIsOn;
        public bool isBlocking;
        public bool isLeftHand;
        public bool isSpellCasting;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;
        //public EnemyStates parryTarget;

        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;

        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        [HideInInspector]
        public Action currentAction;

        bool inBossMode = false;
        public bool attacking = false;

        public int stamina_max = 150;
        public float health = 350;
        public int damage = 75;
        public int defence = 50;
        public float stamina = 150;
        int poise = 35;
        float stamina_cooldown =1.1f;
        float stamina_regenrate = .8f;
        float stamina_timer = 0;
        float _actionDelay;
        public bool stamina_break = false;
        public float stamina_breakTimer = 0;
        public bonfireTrigger bon1;
        public ParticleSystem doorFog;
        public AudioSource deathMusic;

        AudioSource walkingAroundMusic;
        int healingItemsCount = 2;
        public Text healingItemCounterText;

        public healthPickUp healthPickUp1;

        public bool paused = false;

        public void Init()
        {
            pauseOverlay.SetActive(false);
            SetupAnimator();
            anim.SetFloat("animSpeed", 1f);
            rigid = GetComponentInChildren<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init(this);

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.GetComponent<AnimatorHook>();
            if(a_hook == null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this, null);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool(StaticStrings.onGround, true);
            walkingAroundMusic = GetComponent<AudioSource>();
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
            }
            else
            {
                activeModel = anim.gameObject;
            }

            if (anim == null)
            {
                anim = activeModel.GetComponent<Animator>();
            }

            anim.applyRootMotion = false;
        }

        bool regeneratingHP;
        float regenBegin = -3;
        int maxHealth = 350;
        public GameObject healBuff;
        bool debounce = true;
        float pauseTimer;

        public void Tick(float d)
        {
            if (healthPickUp1.inside && healthPickUp1.pickedUp == false)
            {
                if (Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.E))
                {
                    anim.Play("pick_up");
                    healthPickUp1.pickedUp = true;
                    healingItemsCount++;
                }
            }

            if((Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetButtonDown("Start")) && pauseTimer + 1f < Time.realtimeSinceStartup)
            {
                pauseTimer = Time.realtimeSinceStartup;
                if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                    paused = true;
                    pauseOverlay.SetActive(true);
                }
                else if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                    paused = false;
                    pauseOverlay.SetActive(false);
                }
            }

            if (regenBegin + 1.5f < Time.time)
            {
                debounce = true;
            }

            if(regenBegin > Time.time - 3)
            {
                if (health < maxHealth)
                {
                    health+=.4f;
                }
                healBuff.SetActive(true);
            }
            else
            {
                healBuff.SetActive(false);
            }

            if(healingItemsCount > 0)
            {
                healingItem.SetActive(true);
                healingItemCounterText.gameObject.SetActive(true);
                healingItemCounterText.text = healingItemsCount.ToString();
                if ((Input.GetButtonUp("X") || Input.GetKeyDown(KeyCode.R)) && debounce)
                {
                    anim.Play("use_item");
                    healingItemsCount--;
                    regeneratingHP = true;
                    regenBegin = Time.time;
                    debounce = false;
                }
            }
            else
            {
                healingItem.SetActive(false);
                healingItemCounterText.gameObject.SetActive(false);
            }

            if(BossTrigger.singleton.bossMode)
            {
                walkingAroundMusic.Stop();
            }
            delta = d;
            onGround = OnGround();

            anim.SetBool(StaticStrings.onGround, onGround);
            if (CanvasUpdater.singleton.restart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if(Input.GetButton("A") && door.singleton.entered)
            {
                timer = Time.time;
                anim.Play("GoThroughMist");
                bossDoor.isTrigger = true;
                doorFog.Stop();
            }
            if(!door.singleton.entered && timer+6 <Time.time)
            {
                doorFog.Play();
                bossDoor.isTrigger = false;
            }
            if(Input.GetAxis(StaticStrings.Vertical) != 0 || Input.GetAxis(StaticStrings.Horizontal) != 0)
            {
                anim.SetBool("sitting", false);
                inventoryManager.rightHandWeapon.w_hook.gameObject.SetActive(true);
                inventoryManager.leftHandWeapon.w_hook.gameObject.SetActive(true);
            }
            if (bon1.entered)
            {
                if(Input.GetButton("A"))
                {
                    inventoryManager.rightHandWeapon.w_hook.gameObject.SetActive(false);
                    inventoryManager.leftHandWeapon.w_hook.gameObject.SetActive(false);
                    anim.Play("Sit");
                    anim.SetBool("sitting", true);
                }
            }
        }

        void HandleRolls()
        {
            if (!rollInput || usingItem)
                return;

            float v = vertical;
            float h = horizontal;

            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;

            /*if(lockOn == false)
            {
                v = (moveAmount > 0.3f)? 1: 0;
                h = 0;
            }
            else
            {
                if (Mathf.Abs(v) < .3f)
                    v = 0;
                if (Mathf.Abs(h) < .3f)
                    h = 0;
            }*/
            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rm_multi = rollSpeed;
            }
            else
            {
                a_hook.rm_multi = 1.3f;
            }

            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, h);

            canMove = false;
            inAction = true;

            anim.CrossFade(StaticStrings.Rolls, .2f);
        }

        //float stamina_update()
        //{
        //    return
        //}
        bool tog = true;
        public void FixedTick(float d)
        {
            if (health <= 0)
            {
                anim.Play("death");
                if (tog)
                {
                    deathMusic.Play();
                    tog = false;
                }
            }
            if (stamina_breakTimer + 1f < Time.time)
            {
                stamina_break = false;
            }
            if (stamina_break)
                return;
            //print("blocking = "+isBlocking);
            if(stamina_timer + stamina_cooldown < Time.time)
            {
                if(stamina <= stamina_max)
                    stamina += stamina_regenrate;
            }
            delta = d;

            isBlocking = lb;
            usingItem = anim.GetBool(StaticStrings.interacting);
            anim.SetBool(StaticStrings.spellcasting, isSpellCasting);
            DetectAction();
            DetectItemAction();          
            inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

            anim.SetBool(StaticStrings.blocking, isBlocking);
            anim.SetBool(StaticStrings.isLeft, isLeftHand);

            if (inAction)
            {
                anim.applyRootMotion = true;

                _actionDelay += delta;
                if(_actionDelay > .3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                    return;
            }

            canMove = anim.GetBool(StaticStrings.canMove);

            if (!canMove)
                return;

            anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || !onGround)? 0 : 4;

            float targetSpeed = moveSpeed;
            if(usingItem || isSpellCasting)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.45f);
            }

            if(run)
            {
                targetSpeed = runSpeed;
            }

            if(onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;

            HandleRotation();

            anim.SetBool(StaticStrings.lockon, lockOn);

            if (lockOn == false)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDir);

            //if (isSpellCasting)
            //{
                //HandleSpellCasting();
                //return;
            //}

            a_hook.CloseRoll();
            HandleRolls();
        }

        void HandleRotation()
        {
            Vector3 targertDir = (lockOn == false) ?
               moveDir
               :
               (lockOnTransform != null) ?
                   lockOnTransform.transform.position - transform.position
                   :
                   moveDir;
            targertDir.y = 0;
            if (targertDir == Vector3.zero)
                targertDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targertDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }

        public void DetectItemAction()
        {
            if (canMove == false || usingItem || isBlocking)
                return;

            if (itemInput == false)
                return;

            ItemAction slot = actionManager.consumableItem;
            string targetAnim = slot.targetAnim;
            if (string.IsNullOrEmpty(targetAnim))
                return;

            usingItem = true;
            anim.Play(targetAnim);
        }

        public void DetectAction()
        {
            if(stamina <= 0)
            {
                stamina_cooldown = 1.6f;
                return;
            }
            else
            {
                stamina_cooldown = 1.1f;
            }
            if (canMove == false || usingItem)
                return;
            if (rb == false && rt == false && lb == false && lt == false)
                return;
            if(lb)
            {
                anim.SetBool(StaticStrings.mirror, true);
                isBlocking = true;
                anim.CrossFade("shield_l", .2f);
                stamina_regenrate = .3f;
            }
            else
            {
                isBlocking = false;
            }
            if(rb && !isBlocking && stamina_timer + .3f < Time.time)
            {
                
                print("stamina = "+stamina);
                stamina -= 40;
                stamina_timer = Time.time;
                //attacking = true;
                //rigid.velocity = Vector3.zero;
                anim.SetBool(StaticStrings.mirror, false);
                anim.Play("oh_attack_1");
            }
        }

        /*public void DetectAction()
        {
            if (canMove == false || usingItem || isSpellCasting)
                return;

            if (rb == false && rt == false && lb == false && lt == false)
                return;

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;

            switch (slot.type)
            {
                case ActionType.attack:
                    AttackAction(slot);
                    break;
                case ActionType.block:
                    BlockAction(slot);
                    break;
                case ActionType.spells:
                    SpellAction(slot);
                    break;
                case ActionType.parry:
                    ParryAction(slot);
                    break;
                default:
                    break;
            }
        }*/

        void AttackAction(Action slot)
        {
            if (CheckForParry(slot))
                return;

            if (CheckForBackstab(slot))
                return;

            string targetAnim = null;

            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            currentAction = slot;

            canMove = false;
            inAction = true;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            canBeParried = slot.canBeParried;
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, .2f);
        }

        /*void SpellAction(Action slot)
        {
            if (slot.spellClass != inventoryManager.currentSpell.instance.spellClass)
            {
                //targetAnim = cantcast;
                //anim.CrossFade(targetAnim, .2f);
                Debug.Log("Spell class doesn't match!");
                return;
            }

            ActionInput inp = actionManager.GetActionInput(this);
            if (inp == ActionInput.lb)
                inp = ActionInput.rb;
            if (inp == ActionInput.lt)
                inp = ActionInput.rt;

            Spell s_inst = inventoryManager.currentSpell.instance;
            SpellAction s_slot = s_inst.GetAction(s_inst.actions, inp);
            if (s_inst == null)
            {
                Debug.Log("Can't find spell slot");
                return;
            }

            isSpellCasting = true;
            spellCastTime = 0;
            max_spellCastTime = s_slot.castTime;
            spellTargetAnim = s_slot.throwAnim;
            spellIsMirrored = slot.mirror;

            string targetAnim = s_slot.targetAnim;
            if (spellIsMirrored)
                targetAnim += StaticStrings._l;
            else
                targetAnim += StaticStrings._r;

            projectileCanidate = inventoryManager.currentSpell.instance.projectile;

            //inventoryManager.CreateSpellParticle(inventoryManager.currentSpell, spellIsMirrored);
            anim.SetBool(StaticStrings.spellcasting, true);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, .2f);
        }*/

        //spellcasting vars
        //float spellCastTime;
        //float max_spellCastTime;
        //string spellTargetAnim;
        //bool spellIsMirrored;
        //GameObject projectileCanidate;
        //spellcasting vars

        /*void HandleSpellCasting()
        {
            spellCastTime += delta;

            if(inventoryManager.currentSpell.currentParticle != null)
                inventoryManager.currentSpell.currentParticle.SetActive(true);

            if(spellCastTime > max_spellCastTime)
            {
                canMove = false;
                inAction = true;
                isSpellCasting = false;

                string targetAnim = spellTargetAnim;
                anim.SetBool(StaticStrings.mirror, spellIsMirrored);
                anim.CrossFade(targetAnim, .2f);
            }
        }

        public void ThrowProjectile()
        {
            if (projectileCanidate == null)
                return;

            GameObject go = Instantiate(projectileCanidate) as GameObject;
            Transform p = anim.GetBoneTransform((spellIsMirrored) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            go.transform.position = p.position;

            if (lockOnTransform && lockOn)
                go.transform.LookAt(lockOnTransform.position);
            else
                go.transform.rotation = transform.rotation;

            Projectile proj = go.GetComponent<Projectile>();
            proj.Init();
        }*/

        bool CheckForBackstab(Action slot)
        {
            if (slot.canBackStab == false)
                return false;

            EnemyStates backstab = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 1, ignoreLayers))
            {
                backstab = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (backstab == null || backstab.isDead)
                return false;

            Vector3 dir = transform.position - backstab.transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(backstab.transform.forward, dir);

            if (angle > 150)
            {
                Vector3 targetPosition = dir * backstabOffset;
                targetPosition += backstab.transform.position;
                transform.position = targetPosition;

                backstab.transform.rotation = transform.rotation;
                backstab.IsGettingBackstabbed(slot);
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, .2f);
                lockOnTarget = null;
                return true;
            }

            return false;
        }

        bool CheckForParry(Action slot)
        {
            if (slot.canParry == false)
                return false;

            EnemyStates parryTarget = null; 
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (parryTarget == null || parryTarget.isDead)
                return false;

            if (parryTarget.states == null)
                return false;

            /*float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

            if (dis > 3)
                return false;*/

            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            if(angle < 60)
            {
                Vector3 targetPosition = -dir * parryOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;

                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.forward;

                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion ourRot = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = ourRot;

                parryTarget.IsGettingParried(slot);

                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, .2f);
                lockOnTarget = null;
                return true;
            }

            return false;
        }

        /*void BlockAction(Action slot)
        {
            isBlocking = true;
            isLeftHand = slot.mirror;
        }*/

        void ParryAction(Action slot)
        {
            string targetAnim = null;

            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);

            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, .2f);
        }

        void HandleMovementAnimations()
        {
            anim.SetBool(StaticStrings.run, run);
            anim.SetFloat(StaticStrings.vertical, moveAmount);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat(StaticStrings.vertical, v, .2f, delta);
            anim.SetFloat(StaticStrings.horizontal, h, .2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            if(Physics.Raycast(origin,dir,out hit,dis))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }

            return r;
        }

        public void HandleTwoHanded()
        {
            bool isRight = true;
            Weapon w = inventoryManager.rightHandWeapon.instance;
            if (w == null)
            {
                w = inventoryManager.leftHandWeapon.instance;
                isRight = false;
            }

            if(w == null)
            {
                return;
            }

            if (isTwoHanded)
            {
                anim.CrossFade(w.th_idle, .2f);
                actionManager.UpdateActionsTwoHanded();
                
                if(isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(false);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.weaponModel.SetActive(false);
                }
            }
            else
            {
                string targetAnim = w.oh_idle;
                targetAnim += (isRight) ? StaticStrings._r : StaticStrings._l;
                //anim.CrossFade(targetAnim, .2f);
                anim.Play(StaticStrings.equipWeapon_oh);
                actionManager.UpdateActionsOneHanded();

                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(true);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.weaponModel.SetActive(true);
                }
            }
        }

        float timer=0;
        public int doDamage(GameObject g)
        {
            BossAI e;
            e = g.GetComponent<BossAI>();
            print(e.health);
            if (timer + 1f < Time.time)
            {
                timer = Time.time;
                e.health -= damage;
            }
            return e.health;
        }
    }
}