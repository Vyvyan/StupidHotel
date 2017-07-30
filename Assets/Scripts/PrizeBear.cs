using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeBear : MonoBehaviour {

    public int starsToDispense;
    public GameObject star;
    public GameObject starSpawnPoint;

    public float starFireSpeed;
    float starFireTimer, startingFireSpeed;


	// Use this for initialization
	void Start ()
    {
        // we reference this number so we can reset it
        startingFireSpeed = starFireSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (starsToDispense > 0)
            {
                if (starFireTimer <= 0)
                {
                    GameObject tempStar = Instantiate(star, starSpawnPoint.transform.position, Quaternion.identity) as GameObject;
                    tempStar.GetComponent<Rigidbody>().AddForce(Vector3.back * 5, ForceMode.Impulse);
                    starFireTimer = starFireSpeed;
                    starsToDispense--;

                    // now we lower the spawn rate slightly, so it spawns each additional star faster
                    if (starFireSpeed > .05f)
                    {
                        starFireSpeed *= .85f;
                    }
                }
                else
                {
                    starFireTimer -= Time.deltaTime;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // reset it to the original fire speed
            starFireSpeed = startingFireSpeed;
        }
    }
}
