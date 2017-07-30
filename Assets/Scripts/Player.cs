using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Camera playersCamera;
    public bool isHoldingAnObject;
    GameObject heldItem;
    public GameObject objectHoldLocation;
    SpringJoint holdJoint;
    Rigidbody heldItemRB;
    // when we pick up an object, we increase its drag while we hold it. we need this variable to change it back once we drop it
    float dragOfObjectToSwitchBackToWhenDropped;
    float massOfObjectToSwitchBackToWhenDropped;
    Vector3 holdObjectLocationsStartingPosition;

    // moneys
    public int stars;

    // crosshair stuff
    public Texture2D Texture_CrosshairDefault, Texture_CrosshairHand, Texture_CrosshairBlack;
    public float crosshairScale = 1;
    Texture2D crosshairTexture;

    // Use this for initialization
    void Start ()
    {
        playersCamera = Camera.main;
        holdJoint = objectHoldLocation.GetComponent<SpringJoint>();
        holdObjectLocationsStartingPosition = objectHoldLocation.transform.localPosition;
        crosshairTexture = Texture_CrosshairDefault;
    }

    // Update is called once per frame
    void Update()
    {
        ///////////
        // RAYCASTING FROM CENTER OF CAMERA
        ///////////
        Ray ray = playersCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            // does the object we are looking at have a resource script?
            if (hit.transform.gameObject.GetComponent<pickUppable>() != null)
            {
                // crosshair stuff
                crosshairTexture = Texture_CrosshairHand;

                if (!isHoldingAnObject)
                {
                    // are we holding down left click?
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {

                        heldItem = hit.transform.gameObject;
                        heldItemRB = heldItem.GetComponent<Rigidbody>();
                        //heldItemRB.isKinematic = true;
                        //we change the layer so we can raycast through the held object, so we can then move our holdlocation by raycasting through thheld object, its complicates, ma dood
                        heldItem.layer = LayerMask.NameToLayer("Ignore Raycast");


                        dragOfObjectToSwitchBackToWhenDropped = heldItemRB.drag;
                        massOfObjectToSwitchBackToWhenDropped = heldItemRB.mass;
                        heldItemRB.drag = 800;
                        heldItemRB.mass *= 10;

                        // CREATE A NEW SPRING WHENEVER WE PICK UP AN OBJECT
                        // get the position of the object we are picking up to revert back to after making the spring
                        Vector3 storedPOSofObjectWePickedUp = heldItem.transform.position;
                        // move held item to the hold position
                        heldItem.transform.position = objectHoldLocation.transform.position;
                        // make the spring
                        holdJoint = objectHoldLocation.gameObject.AddComponent<SpringJoint>();
                        holdJoint.connectedBody = heldItemRB;
                        holdJoint.autoConfigureConnectedAnchor = true;
                        holdJoint.anchor = Vector3.zero;
                        holdJoint.connectedAnchor = Vector3.zero;
                        holdJoint.spring = 5000;
                        holdJoint.damper = 10f;
                        holdJoint.minDistance = 0;
                        holdJoint.maxDistance = 0;
                        // put the object back where it was
                        heldItem.transform.position = storedPOSofObjectWePickedUp;

                        // constraints
                        heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;

                    }
                }
            }          
            else
            {
                crosshairTexture = Texture_CrosshairDefault;
            }
        }
        else
        {
            crosshairTexture = Texture_CrosshairDefault;
        }

        if (isHoldingAnObject)
        {
            // cross hair
            crosshairTexture = Texture_CrosshairBlack;
        }

        // dropping items
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // if we have an item to drop
            if (heldItem)
            {
                
                // destroy the spring
                Destroy(holdJoint);
                heldItemRB.drag = dragOfObjectToSwitchBackToWhenDropped;
                heldItemRB.mass = massOfObjectToSwitchBackToWhenDropped;
                heldItemRB.velocity = heldItemRB.velocity * .8f;
                heldItemRB.constraints = RigidbodyConstraints.None;


                /*
                heldItemRB.isKinematic = false;
                heldItem.layer = LayerMask.NameToLayer("Default");
                // slow the velocity of the object as we let go so its not crazy shits
                heldItemRB.velocity = heldItemRB.velocity * .30f;
                objectHoldLocation.transform.localPosition = holdObjectLocationsStartingPosition;
                */
                heldItem.layer = LayerMask.NameToLayer("Default");
                heldItem = null;
            }
        }

        // if we are holding an object, then isholdinganobject is true
        isHoldingAnObject = heldItem != null;
    }

    void OnGUI()
    {
        //if not paused
        if (Time.timeScale != 0)
        {
            if (crosshairTexture != null)
                GUI.DrawTexture(new Rect((Screen.width - crosshairTexture.width * crosshairScale) / 2, (Screen.height - crosshairTexture.height * crosshairScale) / 2, crosshairTexture.width * crosshairScale, crosshairTexture.height * crosshairScale), crosshairTexture);
            else
                Debug.Log("No crosshair texture set in the Inspector");
        }
    }
}
