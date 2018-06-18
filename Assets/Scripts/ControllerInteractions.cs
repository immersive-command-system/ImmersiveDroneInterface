namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using VRTK.UnityEventHelper;
    using VRTK;

    // THis class handles all controller interactions with the waypoints and drone

    public class ControllerInteractions : MonoBehaviour
    {
        public enum ControllerState {IDLE, GRABBING, PLACING_WAYPOINT, POINTING, SETTING_HEIGHT}; // These are the possible values for the controller's state
        public ControllerState currentControllerState; // We use this to determine what state the controller is in - and what actions are available

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
        public Material adjustMaterial;

        private static bool indexPressed;
        private static bool indexReleased;
        private static bool isGrabbing;

        private static GameObject placePoint; // Place waypoint in front of controller
        public Material placePointMaterial;

        private Waypoint currentWaypoint; // The current waypoint we are trying to place

        public void Start()
        {
            // Defining the selection zone variables
            inSelectionZone = CollisionType.NOTHING;
            lastSelectedObject = null;

            // Assigning the controller and setting the controller state
            controller_right = GameObject.Find("controller_right");
            controller = GameObject.FindGameObjectWithTag("GameController");
            currentControllerState = ControllerState.IDLE;

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
            
            // Creating the placePoint
            placePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            placePoint.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            placePoint.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            placePoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            placePoint.SetActive(true);
        }

        /// <summary>
        /// The Update method handles changing the controller state and waypoint placement/deletion
        /// </summary>
        void Update()
        {
            indexPressed = false;
            indexReleased = false;

            isGrabbing = controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null;

            // Checks for index button press
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                indexPressed = true;
            }

            // Checks for index button release
            if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
            {
                indexPressed = false;
                indexReleased = true;
            }

            // UNDO AND DELETE (B - BUTTON)
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                UndoAndDeleteWaypoints();
            }

            // SELECTION POINTER
            // Activated by the right grip trigger
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.8f && !isGrabbing)
            {
                toggleRaycastOn();
            }
            else if(currentControllerState == ControllerState.POINTING)
            {
                toggleRaycastOff();
            }

            //PRIMARY PLACEMENT
            if (currentControllerState == ControllerState.IDLE && indexPressed)
            {
                currentWaypoint = CreateWaypoint(placePoint.transform.position);
                currentControllerState = ControllerState.PLACING_WAYPOINT;
            }
            if (currentControllerState == ControllerState.PLACING_WAYPOINT)
            {
                currentWaypoint.gameObjectPointer.transform.position = placePoint.transform.position;
                currentWaypoint.gameObjectPointer.GetComponent<WaypointProperties>().UpdateLine();
            }
            if (currentControllerState == ControllerState.PLACING_WAYPOINT && indexReleased)
            {
                currentControllerState = ControllerState.IDLE;
            }

            // SECONDARY PLACEMENT
            // Initializing groundPoint
            if (currentControllerState == ControllerState.POINTING && indexPressed==true)
            {
                Vector3 groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetGroundPoint();
                currentWaypoint = (Waypoint) CreateWaypoint(groundPoint);
                currentControllerState = ControllerState.SETTING_HEIGHT;
                indexPressed = false;
            }
            // Adjusting the height for secondary placement
            if(currentControllerState == ControllerState.SETTING_HEIGHT)
            {
                AdjustHeight(currentWaypoint);
            }
            // Ending the height adjustment
            if (currentControllerState == ControllerState.SETTING_HEIGHT && indexPressed)
            {
                currentControllerState = ControllerState.IDLE;
            }
        }

        /// <summary>
        /// This handles setting the height of the new waypoint
        /// </summary>
        /// <param name="newWaypoint"></param>
        private void AdjustHeight(Waypoint newWaypoint)
        {
            GameObject newWaypointGameObject = newWaypoint.gameObjectPointer;
            float groundX = newWaypointGameObject.transform.position.x;
            float groundY = newWaypointGameObject.transform.position.y;
            float groundZ = newWaypointGameObject.transform.position.z;
            float localX = controller.transform.position.x;
            float localY = controller.transform.position.y;
            float localZ = controller.transform.position.z;
            float height = 2.147f + (float)Distance(groundX, groundZ, 0f, 0f, localX, localZ) * (float)Math.Tan(Math.PI * (ControllerInteractions.getLocalControllerRotation(OVRInput.Controller.RTouch).x));
            float heightMin = 2.3f + WorldProperties.actualScale.y / 200; //mesh height = 2.147

            height = Math.Min(WorldProperties.GetMaxHeight(), Math.Max(heightMin, height));
            newWaypointGameObject.transform.position = new Vector3(groundX, height, groundZ);
        }

        /// <summary>
        /// Instantiates and returns a new waypoint at the placePoint position.
        /// Modifies behavior to add or insert based on current collisions
        /// </summary>
        /// <param name="groundPoint"> This is the location on the ground that the waypoint will be directly above. </param>
        /// <returns></returns>
        private Waypoint CreateWaypoint(Vector3 groundPoint)
        {
            // We will use the placePoint location.
            Vector3 newLocation = new Vector3(groundPoint.x, placePoint.transform.position.y, groundPoint.z);
            Drone currentlySelectedDrone = WorldProperties.selectedDrone; // Grabbing the drone that we are creating this waypoint for

            // Create a new waypoint at that location
            Waypoint newWaypoint = new Waypoint(currentlySelectedDrone, newLocation);

            // INSERT
            // Placing a new waypoint in between old ones - triggers if a line is in the selection zone
            if (inSelectionZone == CollisionType.LINE)
            {
                // Grabbing the waypoint at the origin of the line (the lines point back towards the start)
                Waypoint lineOriginWaypoint = lastSelectedObject.GetComponent<WaypointProperties>().classPointer;

                // Insert the new waypoint into the drone path just behind the lineOriginWaypoint
                currentlySelectedDrone.InsertWaypoint(newWaypoint, lineOriginWaypoint.prevPathPoint);
            }

            // ADD
            // If we don't have a line selected, we default to placing the new waypoint at the end of the path
            else
            {
                // Create a new waypoint and add it to the end of the drone path
                currentlySelectedDrone.AddWaypoint(newWaypoint);
            }
            return newWaypoint;
        }

        /// <summary>
        /// This method handles the undo and delete functionality
        /// Removes the waypoint from the scene and from the drone's path
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
        /// This function handles objects entering the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider"> This is the collider that our selection zone is intersecting with </param>
        void OnTriggerEnter(Collider currentCollider)
        {
            Debug.Log("Controller has collided with something");

            // WAYPOINT COLLISION
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                Debug.Log("Controller has collided with a waypoint");

                inSelectionZone = CollisionType.WAYPOINT; // Noting that we collided with a waypoint
                lastSelectedObject = currentCollider.gameObject; // This gives us the gameObject that was most recently in the selectionZone.
            }

            // LINE COLLISION
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

        // Turn the raycast on and the place point off
        private void toggleRaycastOn()
        {
            GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = false; // This prevents the raycast from colliding with the grab zone
            placePoint.SetActive(false); // Prevents placePoint from blocking raycast
            controller.GetComponent<VRTK_Pointer>().Toggle(true);

            currentControllerState = ControllerState.POINTING; // Switch to the controller's pointing state
        }

        // Turn the raycast off and the place point on
        private void toggleRaycastOff()
        {
            controller.GetComponent<VRTK_Pointer>().Toggle(false);
            placePoint.SetActive(true); // turn placePoint back on
            GameObject.Find("sphereVRTK").GetComponent<SphereCollider>().enabled = true;

            currentControllerState = ControllerState.IDLE; // Switch to the controller's idle state
        }

        //Get local rotation of controller
        public static Quaternion getLocalControllerRotation(OVRInput.Controller buttonType)
        {
            return OVRInput.GetLocalControllerRotation(buttonType);
        }

        // Finds the distance between the controller and the ground
        private double Distance(float groundX, float groundZ, float groundY, float controllerY, float controllerX, float controllerZ)
        {
            return Math.Sqrt(Math.Pow((controllerX - groundX), 2) + Math.Pow((controllerY - groundY), 2) + Math.Pow((controllerZ - groundZ), 2));
        }
    }
}