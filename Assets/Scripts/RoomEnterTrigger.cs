using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour {

    public RoomDoor door;
    Animator doorAnim;

	// Use this for initialization
	void Start ()
    {
        doorAnim = door.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            doorAnim.SetTrigger("Open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            doorAnim.SetTrigger("Close");
        }
    }
}
