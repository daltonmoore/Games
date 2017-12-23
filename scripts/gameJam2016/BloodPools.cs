using UnityEngine;
using System.Collections;

public class BloodPools : MonoBehaviour
{
    public GameObject pool1;
    public GameObject pool2;
    public GameObject pool3;
    public GameObject pool4;
    public Transform bloodSpawn;
    public int numberOfPools;
    public int radius;
    public float decayWait;
    public float decayLength;
    int sum = 0;
    int sumExpected;

    GameObject[] pools;
    bool isRunning = false;

	// Use this for initialization
	void Start ()
    {
        pools = new GameObject[numberOfPools];
        sumExpected = sumUp(numberOfPools-1);
	}

    int sumUp(int n)
    {
        if (n == 1)
            return 1;
        return n + sumUp(n - 1);
    }
	
	// Update is called once per frame
	void Awake ()
    {

        //if (Input.GetKeyDown(KeyCode.A) && !isRunning)
        {
            isRunning = true;
            for(int i = 0; i < numberOfPools; i++)
            {
                Vector3 position = new Vector3(Random.insideUnitCircle.x * radius, Random.insideUnitCircle.y * radius, 0);
                var temp = Random.Range(0, 4);
                if (temp == 0)
                    pools[i] = (GameObject)Instantiate(pool1, bloodSpawn.position + position, transform.rotation);
                if (temp == 1)
                    pools[i] = (GameObject)Instantiate(pool2, bloodSpawn.position + position, transform.rotation);
                if (temp == 2)
                    pools[i] = (GameObject)Instantiate(pool3, bloodSpawn.position + position, transform.rotation);
                if (temp == 3)
                    pools[i] = (GameObject)Instantiate(pool4, bloodSpawn.position + position, transform.rotation);
                pools[i].transform.Rotate(0, 0, Random.Range(-180, 180));
                StartCoroutine("Decay", i);
            }
            sum = 0;
        }
	}
    IEnumerator Decay(int i)
    {
        Color c = pools[i].GetComponent<Renderer>().material.color;
        yield return new WaitForSeconds(decayWait);
        for (float f = Random.Range(0,1.5f); f >= 0; f -= 0.1f)
        {
            c.a = f;
            pools[i].GetComponent<Renderer>().material.color = c;
            yield return new WaitForSeconds(decayLength);
            isRunning = true;
        }
        Destroy(pools[i]);
        sum += i;
        if(sum == sumExpected)//ensures all the loops complete before starting new coroutine
            isRunning = false;
    }
}
