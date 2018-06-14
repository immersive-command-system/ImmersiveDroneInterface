namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using VRTK.UnityEventHelper;
    using VRTK;


    public class ControllerInteractions : MonoBehaviour
    {
        public static bool selectionZone = false; // Is the controller in a waypoint zone?
        public GameObject currentWaypointZone = null; //Waypoint of zone that controller is in
        public Material defaultMaterial;
        public GameObject controller_right;
        public GameObject sphereVRTK;
        public GameObject sparklePrefab;
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
        private GameObject toggleSparkle;
        public WaypointProperties properties;
        private bool nearWaypoint = false;
        private GameObject lineOriginWaypoint;
        private bool lineCollided;

        public void Start()
        {
            controller_right = GameObject.Find("controller_right");


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

            sphereVRTK = tempSphere;
            originalSphereScale = sphereVRTK.transform.localScale;
            //toggleSparkle = GameObject.Instantiate(sparklePrefab, tempSphere.transform);
            //toggleSparkle.transform.position = new Vector3(0F, 0F, 0.1F);
            //toggleSparkle.transform.localScale = new Vector3(1F, 1F, 1F);
            //toggleSparkle.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
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

            // Adjusting grab zone size.
            //if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
            //{
            //    haloStyleZoomToggleButton = !haloStyleZoomToggleButton;
            //    toggleSparkle.SetActive(!toggleSparkle.activeSelf);
            //}

            if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick)[1] != 0 && haloStyleZoomToggleButton == true)
            {
                if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick)[1] > 0 && sphereVRTK.transform.localScale[0] < 0.14F)
                {
                    Vector3 tempVector = sphereVRTK.transform.localScale;
                    Vector3 additionVector = new Vector3(0.001f, 0.001f, 0.001f);
                    sphereVRTK.transform.localScale = additionVector + tempVector;
                }

                if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick)[1] < 0 && sphereVRTK.transform.localScale[0] > 0.02F)
                {
                    Vector3 tempVector = sphereVRTK.transform.localScale;
                    Vector3 additionVector = new Vector3(0.001f, 0.001f, 0.001f);
                    sphereVRTK.transform.localScale = tempVector - additionVector;
                }
            }


            //Stopping Sphere Collision with the RayCast 
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f)
            {
                sphereVRTK.GetComponent<SphereCollider>().enabled = false;
            }
            else
            {
                sphereVRTK.GetComponent<SphereCollider>().enabled = true;
            }

            //Checks grip trigger for raycast toggle. Deactivates during height adjustment && !SetWaypoint.IsAdjustingHeight())
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f && !isGrabbing)
            {
                selectionZone = true; //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
                toggleRaycastOn();
            }
            else
            {
                if (raycastOn == true) //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
                {
                    selectionZone = false; //ULTRA TEMPORARY SOLUTION TO VRTK GRABBABLE WAYPOINT ISSUE 
                }
                toggleRaycastOff();

            }

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f)
            {
                GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = false;
            }
            else
            {
                GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = true;
            }
        }

        // Called when you let go of the right grip
        public void stopGrab()
        {
            toggleRaycastOff();
        }

        /// <summary>
        /// This function handles collisions with the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider"> This is the object that we have collided with </param>
        void OnTriggerEnter(Collider currentCollider)
        {
            // This helps prevent multiple zone selections
            if (selectionZone != true)  
            {
                // Checking to see if controller touched near a waypoint 
                if (currentCollider.gameObject.CompareTag("waypoint"))
                {
                    // Noting that we are in range to delete
                    selectionZone = true; // This tells us that there is something in the selection zone
                    nearWaypoint = true; // This tells us that the selection zone has a waypoint in it

                    currentWaypointZone = currentCollider.gameObject;
                }

                // This checks for collision with a line when we are placing a waypoint -- results in placing an intermediate Waypoint.
                else if (currentCollider.tag == "Line Collider" && !SetWaypoint.IsCurrentlySetting() && !nearWaypoint)
                {
                    selectionZone = true; // This tells us that there is something in the selection zone

                    // This is the waypoint class instance that the line is attached to
                    Waypoint lineOriginWaypointClass = currentCollider.GetComponent<LineProperties>().originWaypoint;

                    lineOriginWaypointClass.referenceDrone.gameObjectPointer.GetComponent<SetWaypoint>().settingIntermediateWaypoint = true;
                    lineCollided = true;

                    // Setting the lineOriginWaypoint.
                    lineOriginWaypoint = lineOriginWaypointClass.prevPathPoint.gameObjectPointer;

                    Debug.Log("Collided with line while placing waypoint: " + lineOriginWaypoint);
                }
            }
        }

        void OnTriggerExit(Collider currentCollider)
        {
            //Letting Unity know that the Object has left the 'deletion zone' of the waypoint 
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                Debug.Log("Leaving Deletion Zone");
                selectionZone = false;
                nearWaypoint = false;
            }
            if (currentCollider.tag == "Line Collider")
            {
                selectionZone = false;
                Waypoint lineOriginWaypoint = currentCollider.GetComponent<LineProperties>().originWaypoint;
                lineOriginWaypoint.referenceDrone.gameObjectPointer.GetComponent<SetWaypoint>().settingIntermediateWaypoint = false;
                lineCollided = false;
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

        public bool LineCollided()
        {
            return lineCollided;
        }

        public Waypoint GetLineOriginWaypoint()
        {
            return lineOriginWaypoint.GetComponent<WaypointProperties>().classPointer;
        }
    }
}