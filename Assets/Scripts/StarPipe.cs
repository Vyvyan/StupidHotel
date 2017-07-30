using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPipe : MonoBehaviour {

    PrizeBear bearScript;

	// Use this for initialization
	void Start ()
    {
        bearScript = GameObject.FindGameObjectWithTag("Bear").GetComponent<PrizeBear>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision other)
    {
        // if a blorp is dropped in us, we take the value of the blorp and add it to the bear to dispense
        if (other.gameObject.tag == "Blorp")
        {
            bearScript.starsToDispense += other.gameObject.GetComponent<Blorp>().Value;
            Destroy(other.gameObject);
        }
    }
}
