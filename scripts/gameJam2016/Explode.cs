using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour
{

    public GameObject chunk1;
    public GameObject chunk2;
    public GameObject chunk3;
    public GameObject Skull;
    public GameObject Leg;
    public int numberOfChunks;
    public float chunkLife;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject[] chunkList = new GameObject[numberOfChunks];
            int skullCount = 0;
            int legCount = 0;
            for(int i = 0; i < numberOfChunks; i++)
            {
                var temp = Random.Range(0, 5);
                if (temp == 0)
                    chunkList[i] = (GameObject)Instantiate(chunk1, transform.position, transform.rotation);
                else if (temp == 1)
                    chunkList[i] = (GameObject)Instantiate(chunk2, transform.position, transform.rotation);
                else if (temp == 2)
                    chunkList[i] = (GameObject)Instantiate(chunk3, transform.position, transform.rotation);
                else if (temp == 3 && skullCount == 0)
                {
                    chunkList[i] = (GameObject)Instantiate(Skull, transform.position, transform.rotation);
                    skullCount++;
                }
                else if (skullCount == 1 && temp == 3)
                    chunkList[i] = (GameObject)Instantiate(chunk3, transform.position, transform.rotation);

                else if (temp == 4 && legCount < 2)
                { 
                    chunkList[i] = (GameObject)Instantiate(Leg, transform.position, transform.rotation);
                    legCount++;
                }
                else
                    chunkList[i] = (GameObject)Instantiate(chunk3, transform.position, transform.rotation);
            }
            for(int i = 0; i < numberOfChunks; i++)
            {
                chunkList[i].transform.Rotate(0, 0, Random.Range(0, 360));
                chunkList[i].GetComponent<Rigidbody2D>().velocity = chunkList[i].transform.right * Random.Range(50,150) * Time.deltaTime;
                Destroy(chunkList[i], chunkLife);
            }
        }
	}
}
