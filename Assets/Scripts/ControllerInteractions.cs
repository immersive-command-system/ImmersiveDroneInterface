namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using VRTK.UnityEventHelper;
    using VRTK;

    // THis class handles all controller interactions with the waypoints and drone

    public class ControllerInteractions : MonoBehaviour
    {
        public enum ControllerState {IDLE, GRABBING, PLACING, POINTING, SETTING_HEIGHT}; // These are the possible values for the controller's state
        public enum CollisionType {NOTHING, WAYPOINT, LINE, OTHER}; // These are the possible values for objects we could be colliding with
        public CollisionType inSelectionZone; // This tells us if we are colliding with something and what that thing is
        public GameObject lastSelectedObject; // This is the last object that we collided with

        public GameObject controller_right; // Our right controller
        private GameObject controller; //needed to access pointer

        public GameObject sphereVRTK;
        public GameObject sparklePrefab;

        public Material defaultMaterial;
        public Material selectedMaterial;
        public Material opaqueMaterial;

        private static bool raycastOn; //raycast state
        private static bool indexPressed;
        private static bool indexReleased;
        private static bool isGrabbing;

        public void Start()
        {
            // Defining the selection zone variables
            inSelectionZone = CollisionType.NOTHING;
            lastSelectedObject = null;

            // Assigning the controller
            controller_right = GameObject.Find("controller_right");
            controller = GameObject.FindGameObjectWithTag("GameController");

            // Creating the sphereVRTK
            this.gameObject.AddComponent<SphereCollider>(); //Adding Sphere collider to controller
            gameObject.GetComponent<SphereCollider>().radius = 0.040f;
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
        }

        /// <summary>
        /// The Update method handles the state of the index trigger, undo / delete activation, and selection pointer toggling
        /// </summary>
        void Update()
        {
            indexPressed = false;
            indexReleased = false;

            isGrabbing = controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null;

            // Handles B button - Undo and Delete Functionality
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                UndoAndDeleteWaypoints();
            }

            // Checks for index button press
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                indexPressed = true;
            }

            // Checks for index button release
            if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
            {
                indexReleased = true;
            }

            // Handles selection pointer toggling 
            // Activated by the right grip trigger
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f && !isGrabbing)
            {
                GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = false; // This prevents the raycast from colliding with the grab zone
                toggleRaycastOn();
            }
            else
            {
                GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = true;
                toggleRaycastOff();
            }

            /////////////////////////////
            // STOLEN FROM SETWAYPOINT //
            /////////////////////////////

            UpdateScale();

            // Prevents the placePoint from blocking the raycast.
            if (ControllerInteractions.IsRaycastOn())
            {
                deactivatePlacePoint();
            }
            else
            {
                activatePlacePoint();
            }

            // Allows user to select a groundpoint which a new waypoint will appear above
            if (setWaypointState && ControllerInteractions.IsIndexPressed() && !ControllerInteractions.IsGrabbing())
            {
                adjustingWaypoint = SetGroundpoint();
                if (adjustingWaypoint == null)
                {
                    return;
                }
                currentlySetting = true;
                deactivateSetWaypointState();

            }
            else if (ControllerInteractions.IsIndexPressed() && !ControllerInteractions.IsGrabbing())
            {
                activateSetWaypointState();
            }

            // Checking to see if you have let go
            if (currentlySetting && !firstClickFinished && ControllerInteractions.IsIndexReleased())
            {
                firstClickFinished = true;
            }
            else if (currentlySetting && !firstClickFinished && placeAtHand)
            {
                currentWaypoint.transform.position = placePoint.transform.position;
                currentWaypoint.GetComponent<WaypointProperties>().UpdateLine();

            }

            // Allows user to adjust the newly placed waypoints height
            if (adjustingHeight && firstClickFinished)
            {
                if (ControllerInteractions.IsIndexPressed())
                {
                    activateSetWaypointState();
                }
                deactivatePlacePoint();
                AdjustHeight(adjustingWaypoint);

            }
            else if (firstClickFinished)
            {
                firstClickFinished = false;
                activateSetWaypointState();
                settingIntermediateWaypoint = false;
                currentlySetting = false;
                placeAtHand = false;
            }
        }

        /// <summary>
        /// Instantiates and returns a new waypoint at the placePoint position.
        /// </summary>
        /// <param name="groundPoint"> This is the location on the ground that the waypoint will be directly above. </param>
        /// <returns></returns>
        private GameObject CreateWaypoint(Vector3 groundPoint)
        {
            // We will use the placePoint location.
            Vector3 newLocation = new Vector3(groundPoint.x, placePoint.transform.position.y, groundPoint.z);

            // Create a new waypoint at that location
            Waypoint newWaypoint = new Waypoint(thisDrone, newLocation);

            // INSERT
            // Placing a new waypoint in between old ones - triggers if a line is in the selection zone
            if (inSelectionZone == CollisionType.LINE)
            {
                // Grabbing the waypoint at the origin of the line (the lines point back towards the start)
                Waypoint lineOriginWaypoint = lastSelectedObject.GetComponent<WaypointProperties>.classPointer;

                // Insert the new waypoint into the drone path just behind the lineOriginWaypoint
                thisDrone.InsertWaypoint(newWaypoint, lineOriginWaypoint.prevPathPoint);
            }

            // ADD
            // If we don't have a line selected, we default to placing the new waypoint at the end of the path
            else
            {
                // Create a new waypoint and add it to the end of the drone path
                thisDrone.AddWaypoint(newWaypoint);
            }
            return newWaypoint.gameObjectPointer;
        }

        /// <summary>
        /// This method handles the undo and delete functionality
        /// </summary>
        public void UndoAndDeleteWaypoints()
        {
            Drone currentlySelectedDrone = WorldProperties.selectedDrone;

            // Make sure the currently selected drone has waypoints
            if (currentlySelectedDrone.waypoints != null && currentlySelectedDrone.waypoints.Count > 0)
            {
                //Checking to see if we are colliding with one of those
                if (inSelectionZone == CollisionType.WAYPOINT && currentlySelectedDrone.waypoints.Contains(lastSelectedObject))
                {
                    // Remove the highlighted waypoint (DELETE)
                    Debug.Log("Removing waypoint in grab zone");

                    inSelectionZone = CollisionType.NOTHING;
                    lastSelectedObject = null;

                    Waypoint selectedWaypoint = lastSelectedObject.GetComponent<WaypointProperties>().classPointer;
                    currentlySelectedDrone.DeleteWaypoint(selectedWaypoint);
                }
                else
                {
                    // Otherwise we default to removing the last waypoint (UNDO)
                    Debug.Log("Removing last waypoint");

                    Waypoint lastWaypoint = (Waypoint) currentlySelectedDrone.waypoints[currentlySelectedDrone.waypoints.Count - 1];

                    if(lastWaypoint.gameObjectPointer == lastSelectedObject)
                    {
                        inSelectionZone = CollisionType.NOTHING;
                        lastSelectedObject = null;
                    }

                    currentlySelectedDrone.DeleteWaypoint(lastWaypoint);
                }
            }
        }

        /// <summary>
        /// This function handles collisions with the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider"> This is the collider that our selection zone is intersecting with </param>
        void OnTriggerEnter(Collider currentCollider)
        {
            Debug.Log("Controller has collided with something");

            // Checking to see if the grab zone collided with a waypoint
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                Debug.Log("Controller has collided with a waypoint");

                inSelectionZone = CollisionType.WAYPOINT; // Noting that we collided with a waypoint
                lastSelectedObject = currentCollider.gameObject; // This gives us the gameObject that was most recently in the selectionZone.
            }

            // Checking to see if the grab zone collided with a line instead
            // We must have left a waypoint (and had our inSelectionZone set to NOTHING) in order to switch to selecting a line
            else if (currentCollider.tag == "Line Collider" && inSelectionZone != CollisionType.WAYPOINT)
            {
                // This is the waypoint class instance that the line is attached to
                inSelectionZone = CollisionType.LINE; // Noting that we are only colliding with a line right now

                Waypoint lineOriginWaypointClass = currentCollider.GetComponent<LineProperties>().originWaypoint;
                lastSelectedObject = currentCollider.gameObject; // This gives us the gameObject that was most recently in the selectionZone.

                // OUTDATED
                lineOriginWaypointClass.referenceDrone.gameObjectPointer.GetComponent<SetWaypoint>().settingIntermediateWaypoint = true;
                
                Debug.Log("Collided with a line whose origin point is at " + lineOriginWaypointClass.id);
            }

            else
            {
                inSelectionZone = CollisionType.OTHER; // Noting that we collided with something other than a waypoint or line
                lastSelectedObject = null; // we can't select other kinds of objects
            }
        }

        /// <summary>
        /// This function handles objects leaving the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider">  This is the collider for the object leaving our zone </param>
        void OnTriggerExit(Collider currentCollider)
        {
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                Debug.Log("A waypoint is leaving the grab zone");
                inSelectionZone = CollisionType.NOTHING; // We may still have something in our collision zone, but we need to note that it is not what we had there before.
            }
            if (currentCollider.tag == "Line Collider")
            {
                Debug.Log("A line is leaving the grab zone");
                inSelectionZone = CollisionType.NOTHING; // We may still have something in our collision zone, but we need to note that it is not what we had there before.
            }
        }

        // UTILITY FUNCTIONS

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

        // Called when you let go of the right grip
        public void stopGrab()
        {
            toggleRaycastOff();
        }

        public static bool IsGrabbing()
        {
            return isGrabbing;
        }
    }
}