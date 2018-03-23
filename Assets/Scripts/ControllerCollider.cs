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
            UndoWayPoints.UndoAndDeleteWaypoints(selectionZone, currentWaypointZone);

            
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
