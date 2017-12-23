using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SA
{
    public class BossAI  : MonoBehaviour
    {
        public AudioClip bossMusic1;
        public CanvasUpdater canvas;
        public AudioSource audioSource;
        AudioSource shriekSource;
        GameObject player;

        bool inRange;
        bool toggleShrieks = true;
        float speed = 3;
        int phase = 1;
        int attackCounter = 0;
        bool brokenPoise = false;
        int lastHealth;
        int poise = 80;
        public int health;
        public int damage;
        float timer;
        public GameObject damageCollider;
        public Animator anim;
        public StateManager states;
        bool isDead;
        public GameObject WINSCREEN;
        public GameObject HUD;
        public GameObject Pause;
        public Button restartGame;
        public AudioSource endGameMusic;

        void Start()
        {
            WINSCREEN.SetActive(false);
            restartGame = WINSCREEN.GetComponentInChildren<Button>();
            restartGame.onClick.AddListener(TaskOnClick);
            //anim.SetFloat("animSpeed", .9f);
            damage = 125;
            //health = 1000;
            lastHealth = health;
            timer = 0;
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");


            canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasUpdater>();
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = bossMusic1;
            shriekSource = GameObject.Find("Shriek").GetComponent<AudioSource>();
        }
        private void Awake()
        {
            //audioSource.Play();
        }

        void TaskOnClick()
        {
            
            SceneManager.LoadScene("mainmenu");
            Time.timeScale = 1;
            
        }

        bool tog = true;
        bool toggle = true;
        void Update()
        {
            if (isDead)
            {
                //Destroy(gameObject);
                if(toggle)
                {
                    audioSource.Stop();
                    endGameMusic.Play();
                    toggle = false;
                }
                WINSCREEN.SetActive(true);
                Pause.SetActive(false);
                HUD.SetActive(false);
                return;
                //Destroy(states.gameObject);
            }
            if(damOpen+.5f<Time.time)
            {
                damageCollider.SetActive(false);
            }
            if(!inRange)
                transform.LookAt(player.transform);
            anim.SetBool("inRange", inRange);
            if (BossTrigger.singleton.bossMode)
            {
                if(tog)
                {
                    audioSource.Play();
                    tog = false;
                }
                if (poise <= 0)
                {
                    brokenPoise = true;
                    poise = 80;
                    //anim.Play("attack_interrupt");
                    return;
                }

                if (states.health <= 0)
                {
                    //anim.Play("point down");
                    audioSource.Stop();
                }
                else
                {
                    if (phase == 1)
                    {
                        phaseOne();
                    }
                    /*if (health - 500 <= 0)
                    {
                        phase = 2;
                    }
                    if (phase == 2)
                    {
                        phaseTwo();
                    }
                    */
                    //if (Mathf.Abs(player.transform.position.z - transform.position.z) < 2)
                    //anim.applyRootMotion = false;

                    if (health <= 0)
                        isDead = true;

                    if (lastHealth != health)
                    {
                        poise -= 20;
                        lastHealth = health;
                    }
                }
            }
        }
        float damOpen;

        void phaseOne()
        {
            if(brokenPoise)
            {

            }
            //transform.LookAt(player.transform);
            //print(inRange);
            //print("timer = "+timer);
            if (inRange && timer + 2f < Time.time)
            {
                timer = Time.time;
                anim.applyRootMotion = true;
                damageCollider.SetActive(true);
                anim.Play("Attack_1");
                damOpen = Time.time;
            }
            else if (timer == 0 || (!inRange && timer + 2f < Time.time))
            {
                anim.Play("Run");
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, states.gameObject.transform.position, step);
                //anim.Play("Locomotion Normal");
                //anim.SetFloat("vertical", .3f);
            }

            if(health <= 250 && toggleShrieks)
            {
                toggleShrieks = false;
                shriekSource.Play();
            }
        }
        bool equipSword = true;
        void phaseTwo()
        {
            if(equipSword)
            {
                anim.Play("Sword_Grab");
                equipSword = false;
                attackCounter = 0;
            }
            if(inRange && timer + 1.8f < Time.time)
            {
                timer = Time.time;
                if (attackCounter == 0)
                {
                    anim.Play("Sword_Attacks");
                }
                if (attackCounter == 1)
                {
                    anim.Play("Sword_Attacks_001");
                }
                if (attackCounter == 2)
                {
                    anim.Play("Sword_Attacks_002");
                }
                if (attackCounter == 3)
                {
                    anim.Play("Sword_Attacks_003");
                }
                attackCounter++;
                if(attackCounter == 4)
                {
                    attackCounter = 0;
                }
            }
            else if(!inRange)
            {
                anim.Play("Run");
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, states.gameObject.transform.position, step);
            }
        }

        public float doDamageToPlayer()
        {
            if (states.isBlocking)
                return states.health;
            states.health -= damage - states.defence;
            if (states.health < 0)
                states.health = 0;
            return states.health;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                inRange = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                inRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                inRange = false;
            }
        }
    }
}