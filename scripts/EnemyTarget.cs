﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class EnemyTarget : MonoBehaviour
    {
        public int index=0;
        public List<Transform> targets = new List<Transform>();
        public List<HumanBodyBones> h_bones = new List<HumanBodyBones>();

        public EnemyStates eStates;

        Animator anim;

        public void Init(EnemyStates eSt)
        {
            eStates = eSt;
            anim = eStates.anim;
            if (anim.isHuman == false)
                return;

            for (int i = 0; i < h_bones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(h_bones[i]));
            }

            EnemyManager.singleton.enemyTargets.Add(this);
        }

        /*public void Init(BossAI b)
        {
            bAI = b;
            anim = b.anim;
            if (anim.isHuman == false)
                return;

            for (int i = 0; i < h_bones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(h_bones[i]));
            }

            EnemyManager.singleton.enemyTargets.Add(this);
            print(EnemyManager.singleton.enemyTargets);
        }*/

        public Transform GetTarget(bool negative = false)
        {
            //print(targets.Count);
            //print(negative);
            if (targets.Count == 0)
                return transform;

            int targetIndex = index;

            if (negative == false)
            {

                //if (index < targets.Count - 1)
                //    index++;
                //else
                //    index = 0;
            }
            else
            {
                if(index < 0)
                    index = targets.Count - 1;
                else
                    index--;
            }

            index = Mathf.Clamp(index, 0, targets.Count);

            return targets[index];
        }
    }
}