using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerInteractions : MonoBehaviour {

    public static bool selectionZone = false; // Is the controller in a waypoint zone?
    public GameObject currentWaypointZone = null; //Waypoint of zone that controller is in
    public Material defaultMaterial;
    public Material selectedMaterial;
    private GameObject controller; //needed to access pointer
    private static bool raycastOn; //raycast state
    private static bool indexPressed;
    private static bool indexReleased;
    
	// Update is called once per frame
	void Update () {
        indexPressed = false;
        indexReleased = false;
       
        //Checks to see if B button was pressed the previous frame
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            UndoWayPoints.UndoAndDeleteWaypoints(selectionZone, currentWaypointZone);
        }


        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            indexPressed = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            indexReleased = true;
        }

        //Checks grip trigger for raycast toggle. Deactivates during height adjustment
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f && controller.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null && !SetWaypoint.IsAdjustingHeight())
        {
            selectionZone = true; //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
            controller.GetComponent<VRTK_Pointer>().Toggle(true);
            raycastOn = true;
        } else
        {
            if(raycastOn == true) //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
            {
                selectionZone = false; //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
            }
            controller.GetComponent<VRTK_Pointer>().Toggle(false);
            raycastOn = false;
            
        }


        


    }

    public void Start()
    {
        this.gameObject.AddComponent<SphereCollider>(); //Adding Sphere collider to controller
        gameObject.GetComponent<SphereCollider>().radius = 0.070f;

        controller = GameObject.FindGameObjectWithTag("GameController");
    }


    
    void OnTriggerEnter(Collider currentCollider)
    {
        if (selectionZone != true) //Preventing multiple zone selections 
        {
            //Checking to see if controller touched near a waypoint 
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {

                //Telling Unity that the Controller is in range to delete
                selectionZone = true;
                Debug.Log("setting deletion Zone");
                currentWaypointZone = currentCollider.gameObject;
                //print(currentCollider.gameObject.GetComponent<MeshRenderer>().material);

                //currentCollider.gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
                //print(currentCollider.gameObject.GetComponent<MeshRenderer>().material);
                WayPointColorSelection.AddWayPointColor(selectionZone, currentWaypointZone);
            }
        }   
    }

    void OnTriggerExit(Collider currentCollider)
    {
        //Letting Unity know that the Object has left the 'deletion zone' of the waypoint 
        if (currentCollider.gameObject.CompareTag("waypoint"))
        {
            Debug.Log("Leaving Deletion Zone");
            WayPointColorSelection.RemoveWayPointColor(selectionZone, currentWaypointZone); //Requires selection zone to be true
            selectionZone = false;
            //currentCollider.gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
           
        }

    }

    //Checks if index pressed a second time, after it was pressed in Update()
    public static bool secondIndexPressed()
    {
        return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
    }

    //Get local rotation of controller
    public static Quaternion getLocalControllerRotation(OVRInput.Controller buttonType)
    {
        return OVRInput.GetLocalControllerRotation(buttonType);
    }

    public static bool IsRaycastOn()
    {
        return raycastOn;
    }

    public static bool IsIndexPressed()
    {
        return indexPressed;
    }

    public static bool IsIndexReleased()
    {
        return indexReleased;
    }
}
