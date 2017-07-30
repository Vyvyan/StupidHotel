using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour {

    public GameObject level;
    GameObject lobby;
    public bool canOpen;
    Animator anim;

	// Use this for initialization
	void Start ()
    {
        lobby = GameObject.FindGameObjectWithTag("Lobby");
        canOpen = true;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ShowLevel()
    {
        level.SetActive(true);
    }

    public void HideLevel()
    {
        anim.SetTrigger("Close");
        level.SetActive(false);
    }
}
