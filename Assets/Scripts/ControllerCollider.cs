using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerCollider : MonoBehaviour {

    public static bool selectionZone = false;
    public GameObject currentWaypointZone = null;
    public Material defaultMaterial;
    public Material selectedMaterial;
    
	// Update is called once per frame
	void Update () {
       
        //Checks to see if B button was pressed the previous frame
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            //Makes sure that there are some waypoints
            if (SetWaypoint.waypoints.Count > 0)
            {
                //THIS IS THE DELETE FUNCTION
                //Checking to see if the controller is near a specific waypoint
                if (selectionZone)
                {
                    Debug.Log("attempting to remove!!!");
                    SetWaypoint.ClearSpecificWayPoint(currentWaypointZone.gameObject);
                    selectionZone = false;
                }
               
                //THIS IS THE UNDO FUNCTION (Delete except not in zone) 
                else
                {
                    Debug.Log("YES");
                    SetWaypoint.ClearWaypoint();
                    
                }

            }


            //THIS IS THE UNDO FUNCTION if no waypoints
            else
            {
                Debug.Log("Undo: Delete Drone");
                //Destroy(SetWaypoint.getDrone());
            }

            
        }
    }

    public void Start()
    {
        this.gameObject.AddComponent<SphereCollider>();
        gameObject.GetComponent<SphereCollider>().radius = 0.1f;
    }


    
    void OnTriggerEnter(Collider currentCollider)
    {
        //Checking to see if controller touched near a waypoint 
        if(currentCollider.gameObject.CompareTag("waypoint")) {
           
            //Telling Unity that the Controller is in range to delete
            selectionZone = true;
            Debug.Log("setting deletion Zone");
            currentWaypointZone = currentCollider.gameObject;
            print(currentCollider.gameObject.GetComponent<MeshRenderer>().material);

            currentCollider.gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
            print(currentCollider.gameObject.GetComponent<MeshRenderer>().material);
        }
        

        
    }

    void OnTriggerExit(Collider currentCollider)
    {
        //Letting Unity know that the Object has left the 'deletion zone' of the waypoint 
        if (currentCollider.gameObject.CompareTag("waypoint"))
        {
            Debug.Log("Leaving Deletion Zone");
            selectionZone = false;
            currentCollider.gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }



    }


}
