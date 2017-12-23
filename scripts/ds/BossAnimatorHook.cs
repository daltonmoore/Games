using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class BossAnimatorHook : MonoBehaviour
    {
        public GameObject[] damageCollider;
        BossAI b;

        public void Init(BossAI bai)
        {
            b = bai;
        }
        public void OpenDamageColliders()
        {
            if (b)
            {
                damageCollider[0].SetActive(true);
            }

            //OpenParryFlag();
        }

        public void CloseDamageColliders()
        {
            if (b)
            {
                damageCollider[0].SetActive(false);
            }

            //CloseParryFlag();
        }
    }
}