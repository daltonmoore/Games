using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{


    public class CanvasUpdater : MonoBehaviour
    {
        public BossAI bai;
        public StateManager states;
        public EnemyStates eStates;
        public Slider healthBar;
        public Slider healthBar2;
        public Slider bossHealth;
        public Slider bossHealth2;
        public Slider eHealthBar;
        public Slider staminaBar;
        public Text bossName;

        public Image deathOverlay;
        public Text deathText;
        public Image deathBar;

        public float deathTimeOut;
        public bool restart = false;
        bool toggle = true;
        private void Start()
        {
            deathOverlay.gameObject.SetActive(false);
            deathText.gameObject.SetActive(false);
            deathBar.gameObject.SetActive(false);
            deathOverlay.GetComponent<CanvasRenderer>().SetAlpha(0f);
            deathText.GetComponent<CanvasRenderer>().SetAlpha(0f);
            deathBar.GetComponent<CanvasRenderer>().SetAlpha(0f);
        }

        private void Update()
        {
            updateStaminaBar();
            updateHealthBar();
            if(BossTrigger.singleton.bossMode)
            {
                bossHealth.gameObject.SetActive(true);
                bossHealth2.gameObject.SetActive(true);
                bossName.gameObject.SetActive(true);
            }
            else
            {
                bossHealth.gameObject.SetActive(false);
                bossHealth2.gameObject.SetActive(false);
                bossName.gameObject.SetActive(false);
            }
            if (healthBar.value != healthBar2.value)
            {
                healthBar2.value -= 1f;
            }
            if(bossHealth.value != bossHealth2.value)
            {
                bossHealth2.value -= 1f;
            }
            if(healthBar.value<=0 && toggle)
            {
                deathTimeOut = Time.time;
                print(deathTimeOut);
                deathOverlay.gameObject.SetActive(true);
                deathOverlay.CrossFadeAlpha(1f, 1.0f, false);
                deathText.gameObject.SetActive(true);
                deathText.CrossFadeAlpha(1f, 1f, false);
                deathBar.gameObject.SetActive(true);
                deathBar.CrossFadeAlpha(1f, 1f, false);
                toggle = false;
                
            }
            if(!toggle && deathTimeOut != 0 && deathTimeOut + 5 < Time.time)
            {
                restart = true;
            }
        }

        public void updateCanvasWithBossDamageToPlayer()
        {  
            healthBar.value = bai.doDamageToPlayer();
        }

        public void updateCanvasWithPlayerDamageToBoss()
        {
            bossHealth.value = states.doDamage(bai.gameObject);
            print(bossHealth.value);
        }

        public void updateCanvasWithPlayerDamageToEnemy()
        {
            eHealthBar.value = states.doDamage(eStates.gameObject);
        }

        public void updateCanvasWithEnemyDamageToPlayer()
        {
            healthBar.value = eStates.doDamageToPlayer();
        }

        public void updateStaminaBar()
        {
            staminaBar.value = states.stamina;
        }

        public void updateHealthBar()
        {
            healthBar.value = states.health;
        }

        public static CanvasUpdater singleton;

        private void Awake()
        {
            singleton = this;
        }
    }
}