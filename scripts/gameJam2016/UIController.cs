using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
    public UnityEngine.UI.Image pistol, assualtRifle, shotgun;
    public UnityEngine.UI.Text pistolAmmo, assaultAmmo, shotgunAmmo, enemiesLeft;
    public int maxPistolAmmo, maxAssaultAmmo, maxShotgunAmmo, enemies;
    public Material selected, unselected;
    public bool wait = false;

    int currentWeapon = 0;
    int currentPistolAmmo, currentAssaultAmmo, currentShotgunAmmo;
    
	// Use this for initialization
	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        Scroll();
        DisplayAmmo(currentPistolAmmo + "/" + maxPistolAmmo, currentAssaultAmmo + "/" + maxAssaultAmmo, currentShotgunAmmo + "/" + maxShotgunAmmo);
        DisplayEnemiesLeft(""+enemies);
    }

    void DisplayEnemiesLeft(string enemies)
    {
        enemiesLeft.text = "Enemies left: "+enemies;
    }

    void DisplayAmmo(string p, string a, string sg )
    {
        pistolAmmo.text = p;
        assaultAmmo.text = a;
        shotgunAmmo.text = sg;
    }

    void Scroll()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && !wait)
        {

            if (currentWeapon == 0)
            {
                //print("Switching to shotgun");
                currentWeapon = 2;
                pistol.material = unselected;
                shotgun.material = selected;
            }
            else if (currentWeapon == 1)
            {
                //print("Switching to pistol");
                currentWeapon = 0;
                assualtRifle.material = unselected;
                pistol.material = selected;
            }
            else if (currentWeapon == 2)
            {
                //print("Switching to assualtRifle");
                currentWeapon = 1;
                shotgun.material = unselected;
                assualtRifle.material = selected;
            }
            StartCoroutine("ScrollDelay");
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && !wait)
        {
            if (currentWeapon == 0)
            {
                //print("Switching to assaultRifle");
                currentWeapon = 1;
                pistol.material = unselected;
                assualtRifle.material = selected;
            }
            else if (currentWeapon == 1)
            {
                //print("Switching to shotgun");
                currentWeapon = 2;
                assualtRifle.material = unselected;
                shotgun.material = selected;
            }
            else if (currentWeapon == 2)
            {
                //print("Switching to pistol");
                currentWeapon = 0;
                shotgun.material = unselected;
                pistol.material = selected;
            }
            StartCoroutine("ScrollDelay");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0;
            pistol.material = selected;
            shotgun.material = unselected;
            assualtRifle.material = unselected;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1;
            pistol.material = unselected;
            shotgun.material = unselected;
            assualtRifle.material = selected;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2;
            pistol.material = unselected;
            shotgun.material = selected;
            assualtRifle.material = unselected;
        }
    }

    IEnumerator ScrollDelay()
    {
        wait = true;
        yield return new WaitForSeconds(.3f);
        wait = false;
    }
}
