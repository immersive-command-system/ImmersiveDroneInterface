using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK.UnityEventHelper;
using VRTK;

public class ControllerInteractions : MonoBehaviour {
    public static bool selectionZone = false; // Is the controller in a waypoint zone?
    public GameObject currentWaypointZone = null; //Waypoint of zone that controller is in
    public Material defaultMaterial;
    public GameObject controller_right;
    public GameObject sphereVRTK;
    public Material selectedMaterial;
    public Material opaqueMaterial;
    //private GameObject sphereVRTK;
    private GameObject controller; //needed to access pointer
    private static bool raycastOn; //raycast state
    private static bool indexPressed;
    private static bool indexReleased;
    private static bool isGrabbing;
    private static bool haloStyleZoomToggleButton = false; //Yes i know it's a shitty name but I dont know what its actually called
    private float minScale;
    private float maxScale;
    private float fakeTime;
    private float timerScale;
    private Vector3 originalSphereScale; 

	// Update is called once per frame
	void Update () {
        indexPressed = false;
        indexReleased = false;

        isGrabbing = controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null;

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

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            haloStyleZoomToggleButton = !haloStyleZoomToggleButton;
            Debug.Log("toggleboi");
        }

        if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick)[1] != 0 && haloStyleZoomToggleButton == true)
        {
            if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick)[1] > 0 && sphereVRTK.transform.localScale[0] < 0.14F)
            {
                Vector3 tempVector = sphereVRTK.transform.localScale;
                //sphereVRTK.transform.localScale = originalSphereScale * timerScale;
                //fakeTime += 0.5f * Time.deltaTime;
                //Debug.Log(fakeTime);
                //Debug.Log(timerScale);
                Vector3 additionVector = new Vector3(0.001f, 0.001f, 0.001f);
                //float scalarFloat = 1.001f;
                sphereVRTK.transform.localScale = additionVector + tempVector;
                //Debug.Log("original" + tempVector.ToString());
                //Debug.Log("addition" + additionVector.ToString());
                //Debug.Log("final" + sphereVRTK.transform.localScale.ToString());
            }

            if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick)[1] < 0 && sphereVRTK.transform.localScale[0] > 0.02F)
            {
                Vector3 tempVector = sphereVRTK.transform.localScale;
                //sphereVRTK.transform.localScale = originalSphereScale * timerScale;
                //fakeTime += 0.5f * Time.deltaTime;
                //Debug.Log(fakeTime);
                //Debug.Log(timerScale);
                Vector3 additionVector = new Vector3(0.001f, 0.001f, 0.001f);
                //float scalarFloat = 1.001f;
                sphereVRTK.transform.localScale = tempVector - additionVector;
                //Debug.Log("original" + tempVector.ToString());
                //Debug.Log("addition" + additionVector.ToString());
                //Debug.Log("final" + sphereVRTK.transform.localScale.ToString());
            }

        }
        

        //Stopping Sphere Collision with the RayCast 
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f)
        {
            sphereVRTK.GetComponent<SphereCollider>().enabled = false;
        } else
        {
            sphereVRTK.GetComponent<SphereCollider>().enabled = true;
        }

        //Checks grip trigger for raycast toggle. Deactivates during height adjustment && !SetWaypoint.IsAdjustingHeight())
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f && !isGrabbing)
        {
            selectionZone = true; //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
            toggleRaycastOn();
        } else
        {
            if(raycastOn == true) //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
            {
                selectionZone = false; //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
            }
            toggleRaycastOff();
            
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f)
        {
            GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = false;
        } else
        {
            GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = true;
        }



    }
    
    public void startGrab()//GETS CALLED WHEN YOU START GRABBING AN OBJECT
    {
        //controller.GetComponent<VRTK_Pointer>().Toggle(false);
        //controller.GetComponent<VRTK_StraightPointerRenderer>().Toggle(false, false);
        //GameObject.Find("[VRTK][AUTOGEN][RightController][StraightPointerRenderer_Tracer]").SetActive(false);
        //toggleRaycastOff();
        //Debug.Log("startgrab");
        //GameObject.Find("[VRTK][AUTOGEN][RightController][StraightPointerRenderer_Tracer]").GetComponent<MeshRenderer>().enabled = false;
    }

    public void stopGrab() //GETS CALLED WHEN YOU STOP GRABBING ON OBJECT
    {
        //controller.GetComponent<VRTK_Pointer>().Toggle(true);
        //controller.GetComponent<VRTK_StraightPointerRenderer>().Toggle(true, true);
        //GameObject.Find("[VRTK][AUTOGEN][RightController][StraightPointerRenderer_Tracer]").SetActive(true);

        //raycastOn = true;
        toggleRaycastOff();
        //Debug.Log("stopgrab");
        //GameObject.Find("[VRTK][AUTOGEN][RightController][StraightPointerRenderer_Tracer]").GetComponent<MeshRenderer>().enabled = true;

    }

    public void Start()
    {
        //minScale = 1;
        //maxScale = 2;
        //fakeTime = 0.0f;
        //timerScale = Mathf.Lerp(minScale, maxScale, fakeTime);

        controller_right = GameObject.Find("controller_right");
        //sphereVRTK = GameObject.Find("sphereVRTK");

        this.gameObject.AddComponent<SphereCollider>(); //Adding Sphere collider to controller
        gameObject.GetComponent<SphereCollider>().radius = 0.040f;
        controller = GameObject.FindGameObjectWithTag("GameController");


        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.gameObject.transform.position = new Vector3(0F, 0F, 0F);
        tempSphere.transform.position = new Vector3(0F, 0F, 0.1F);
        tempSphere.transform.parent = this.gameObject.transform;
        tempSphere.transform.localScale = new Vector3(0.08F, 0.08F, 0.08F);
        this.gameObject.GetComponent<VRTK_InteractTouch>().customColliderContainer = tempSphere;
        tempSphere.gameObject.name = "sphereVRTK";
        Renderer tempRend = tempSphere.GetComponent<Renderer>();
        tempRend.material = opaqueMaterial;
        //tempSphere.GetComponent<SphereCollider>().enabled = false;
        sphereVRTK = tempSphere;
        originalSphereScale = sphereVRTK.transform.localScale;

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
                //WayPointColorSelection.AddWayPointColor(selectionZone, currentWaypointZone);
            }

            else if (currentCollider.tag == "Line Collider")
            {
                Debug.Log("eyey");
                
            }
        }   
    }

    void OnTriggerExit(Collider currentCollider)
    {
        //Letting Unity know that the Object has left the 'deletion zone' of the waypoint 
        if (currentCollider.gameObject.CompareTag("waypoint"))
        {
            Debug.Log("Leaving Deletion Zone");
            //WayPointColorSelection.RemoveWayPointColor(selectionZone, currentWaypointZone); //Requires selection zone to be true
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

    private void toggleRaycastOn()
    {

        controller.GetComponent<VRTK_Pointer>().Toggle(true);
        raycastOn = true;

    }

    private void toggleRaycastOff()
    {
        controller.GetComponent<VRTK_Pointer>().Toggle(false);
        raycastOn = false;
    }

    public static bool IsGrabbing()
    {
        return isGrabbing;
    }
}
