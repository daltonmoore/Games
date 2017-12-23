using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public bool bossMode = false;
    public static BossTrigger singleton;

    private void Awake()
    {
        singleton = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            bossMode = true;
        }
    }
}
