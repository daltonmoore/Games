using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossStats : MonoBehaviour
{
    [Header("Base Power")]
    public int hp = 1000;
    public int fp = 100;
    public int stamina = 250;
    public float equipLoad = 40;
    public float poise = 20;
    public int itemDiscovery = 5;

    [Header("Attack Power")]
    public int R_weapon_1 = 51;
    public int R_weapon_2 = 51;
    public int R_weapon_3 = 51;
    public int L_weapon_1 = 51;
    public int L_weapon_2 = 51;
    public int L_weapon_3 = 51;

    [Header("Defence")]
    public int physical = 87;
    public int vs_strike = 87;
    public int vs_slash = 87;
    public int vs_thrust = 87;
    public int magic = 30;
    public int fire = 30;
    public int lightning = 30;
    public int dark = 30;

    [Header("Resistances")]
    public int bleed = 100;
    public int poison = 100;
    public int frost = 100;
    public int curse = 100;
}
