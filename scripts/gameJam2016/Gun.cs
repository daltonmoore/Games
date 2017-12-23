using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float fireRate;
    public float strayFactor;
    public int numberOfProjectiles;
    public float bulletSpeed;
    public float bulletLife;

    bool canFire = true;

	// Use this for initialization
	void Start ()
    {
       
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Get the Screen positions of the object
        Vector3 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        //Get the Screen position of the mouse
        Vector3 mouseOnScreen = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(mouseOnScreen, positionOnScreen);

        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));


        if(Input.GetMouseButton(0))//left click is down
        {
            if(canFire)
                Fire();
        }
    }

    void Fire()
    {
        GameObject[] bulletList = new GameObject[numberOfProjectiles];

        for(int i = 0; i < numberOfProjectiles; i++)
        {
            bulletList[i] = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, transform.rotation);
            var randomNumberX = Random.Range(-strayFactor, strayFactor);
            var randomNumberY = Random.Range(-strayFactor, strayFactor);
            var randomNumberZ = Random.Range(-strayFactor, strayFactor);

            bulletList[i].transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);

            //bulletList[i].GetComponent<Rigidbody2D>().velocity = bulletList[i].transform.right * bulletSpeed * Time.deltaTime;
            //Destroy(bulletList[i], bulletLife);
        }

        for(int i = 0; i< numberOfProjectiles; i++)
        {
            bulletList[i].GetComponent<Rigidbody2D>().velocity = bulletList[i].transform.right * bulletSpeed * Time.deltaTime;
            Destroy(bulletList[i], bulletLife);
        }

        StartCoroutine("RateOfFire");
    }

    IEnumerator RateOfFire()
    {
        canFire = false;
        yield return new WaitForSeconds(1/fireRate);
        canFire = true;
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    
}
