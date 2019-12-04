namespace ISAACS
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using VRTK.UnityEventHelper;
    using VRTK;
    using ROSBridgeLib.interface_msgs;

    // THis class handles all controller interactions with the waypoints and drone

    public class ControllerInteractions : MonoBehaviour
    {
        public enum ControllerState { IDLE, GRABBING, PLACING_DRONE, PLACING_WAYPOINT, POINTING, SETTING_HEIGHT, SCALING, DELETING }; // These are the possible values for the controller's state
        public static ControllerState currentControllerState; // We use this to determine what state the controller is in - and what actions are available

        public enum CollisionType { NOTHING, WAYPOINT, LINE, OTHER }; // These are the possible values for objects we could be colliding with
        public CollisionPair mostRecentCollision;
        private List<CollisionPair> currentCollisions;

        public GameObject controller_right; // Our right controller
        private GameObject controller; //needed to access pointer

        public GameObject grabZone;
        private static GameObject placePoint; // Place waypoint in front of controller
        private GameObject heightSelectionPlane;

        private Waypoint currentWaypoint; // The current waypoint we are trying to place
        private Waypoint grabbedWaypoint; // The current waypoint we are grabbing and moving

        public Material defaultMaterial;
        public Material selectedMaterial;
        public Material opaqueMaterial;
        public Material adjustMaterial;
        public Material placePointMaterial;
        public Material heightSelectionPlaneMaterial;

        public static Waypoint currentWayPointZone;

        /// <summary>
        /// The start method initializes all necessary variables and creates the selection zone (grabZone) and the place point
        /// </summary>
        public void Start()
        {
            // Defining the selection zone variables
            mostRecentCollision = new CollisionPair(null, CollisionType.NOTHING);
            currentCollisions = new List<CollisionPair>();

            // Assigning the controller and setting the controller state
            controller_right = GameObject.Find("controller_right");
            controller = GameObject.FindGameObjectWithTag("GameController");
            currentControllerState = ControllerState.IDLE;

            // Creating the grabZone GameObject with offset
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tempSphere.transform.parent = this.gameObject.transform;
            this.gameObject.transform.position = new Vector3(0F, 0F, 0F);
            tempSphere.transform.position = new Vector3(0F, 0F, 0.1F);
            tempSphere.transform.localScale = new Vector3(0.08F, 0.08F, 0.08F);
            this.gameObject.GetComponent<VRTK_InteractTouch>().customColliderContainer = tempSphere;
            tempSphere.gameObject.name = "grabZone";
            Renderer tempRend = tempSphere.GetComponent<Renderer>();
            tempRend.material = opaqueMaterial;
            tempSphere.GetComponent<SphereCollider>().isTrigger = true;
            grabZone = tempSphere;

            // Creating the grabZone collider with offset
            this.gameObject.AddComponent<SphereCollider>(); //Adding Sphere collider to controller
            gameObject.GetComponent<SphereCollider>().radius = 0.040f;
            gameObject.GetComponent<SphereCollider>().center = new Vector3(0F, 0F, 0.1F);

            // Creating the placePoint
            placePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            placePoint.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            placePoint.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            placePoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            placePoint.SetActive(true);

            // Creating the heightSelectionPlane
            heightSelectionPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            heightSelectionPlane.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            heightSelectionPlane.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            heightSelectionPlane.transform.localScale = new Vector3(10f, 0.0001f, 10f);
            heightSelectionPlane.GetComponent<Renderer>().material = heightSelectionPlaneMaterial;
            Color newColor = heightSelectionPlaneMaterial.color;
            newColor.a = 0.0f;
            heightSelectionPlane.GetComponent<Renderer>().material.color = newColor;
            StandardShaderUtils.ChangeRenderMode(heightSelectionPlane.GetComponent<Renderer>().material, StandardShaderUtils.BlendMode.Fade);
            heightSelectionPlane.gameObject.name = "heightSelectionPlane";
            heightSelectionPlane.layer = 8;
            heightSelectionPlane.SetActive(false);
        }

        /// <summary>
        /// The Update method calls all the various state and collision checks
        /// </summary>
        void Update()
        {
            /**
            foreach (Waypoint x in WorldProperties.selectedDrone.waypoints)
            {
                Debug.Log(x.id + " : " + x.unityLocation);
            }
            Debug.Log("---");
            **/

            // SELECTION POINTER  
            SelectionPointerChecks();

            if (WorldProperties.selectedDrone != null)
            {
                // WAYPOINT GRABBING
                GrabbingChecks();

                // UNDO AND DELETE (B - BUTTON)
                if (OVRInput.GetDown(OVRInput.Button.Two))
                {
                    UndoAndDeleteWaypoints();
                }

                //PRIMARY PLACEMENT
                PrimaryPlacementChecks();

                // SECONDARY PLACEMENT
                SecondaryPlacementChecks();
            }

            //Debug.Log(currentControllerState);
        }

        /// <summary>
        /// This function handles objects entering the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider"> This is the collider that our selection zone is intersecting with </param>
        void OnTriggerEnter(Collider currentCollider)
        {
            // WAYPOINT COLLISION
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                //Debug.Log("getting here");
                
                Waypoint collidedWaypoint = currentCollider.gameObject.GetComponent<WaypointProperties>().classPointer;
                if (collidedWaypoint.id == "A0")
                {
                    Debug.Log("destroy");
                    Destroy(collidedWaypoint.gameObjectPointer.GetComponent<SphereCollider>());
                    return;
            
                }
                if (!currentCollisions.Any(x => (x.waypoint == collidedWaypoint && x.type == CollisionType.WAYPOINT)))
                {
                    //Debug.Log("A waypoint is entering the grab zone");

                    // We automatically default to the most recent waypointCollision
                    mostRecentCollision = new CollisionPair(collidedWaypoint, CollisionType.WAYPOINT);
                    ////Debug.Log("New mostRecentCollision is a waypoint - " + mostRecentCollision.waypoint.id);
                    currentCollisions.Add(mostRecentCollision);
                }
            }

            // LINE COLLISION
            // We must have left a waypoint (and had our mostRecentCollision.type set to NOTHING) in order to switch to selecting a line
            else if (currentCollider.tag == "Line Collider")
            {
                // This is the waypoint at the end of the line (the line points back toward the path origin / previous waypoint)
                Waypoint lineOriginWaypoint = currentCollider.GetComponent<LineProperties>().originWaypoint;
                if (!currentCollisions.Any(x => (x.waypoint == lineOriginWaypoint && x.type == CollisionType.LINE)))
                {
                    //Debug.Log("A line is entering the grab zone");
                    currentCollisions.Add(new CollisionPair(lineOriginWaypoint, CollisionType.LINE));
                }

            }
        }

        /// <summary>
        /// This function handles objects leaving the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider">  This is the collider for the object leaving our zone </param>
        void OnTriggerExit(Collider currentCollider)
        {
            //CollisionsDebug();
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                Waypoint collidedWaypoint = currentCollider.gameObject.GetComponent<WaypointProperties>().classPointer;
                currentCollisions.RemoveAll(collision => collision.waypoint == collidedWaypoint &&
                                            collision.type == CollisionType.WAYPOINT);

                //Debug.Log("A waypoint is leaving the grab zone");
            }
            if (currentCollider.tag == "Line Collider")
            {
                Waypoint lineOriginWaypoint = currentCollider.GetComponent<LineProperties>().originWaypoint;
                currentCollisions.RemoveAll(collision => collision.waypoint == lineOriginWaypoint &&
                                        collision.type == CollisionType.LINE);

                //Debug.Log("A line is leaving the grab zone");
            }

            if (currentCollisions.Count > 0)
            {
                mostRecentCollision = currentCollisions[currentCollisions.Count - 1];
            }
            else
            {
                mostRecentCollision = new CollisionPair(null, CollisionType.NOTHING);
            }

        }

        /// <summary>
        /// Handles the controller state switch to grabbing
        /// </summary>
        private void GrabbingChecks()
        {
            if (currentControllerState == ControllerState.IDLE &&
                controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null)
            {
                // Updating to note that we are currently grabbing a waypoint
                grabbedWaypoint = controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject().GetComponent<WaypointProperties>().classPointer;
                currentControllerState = ControllerState.GRABBING;
                Debug.Log(grabbedWaypoint + "grabbing 0");
                Debug.Log("Grabbing!");
            }
            else if (currentControllerState == ControllerState.GRABBING &&
              controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null)
            {
                // Updating the line colliders
                grabbedWaypoint.UpdateLineColliders();
                grabbedWaypoint.UpdateLocation(grabbedWaypoint.gameObjectPointer.transform.localPosition);

                // Sending a ROS MODIFY Update
                UserpointInstruction msg = new UserpointInstruction(grabbedWaypoint, "MODIFY");

                //grabbedWaypoint.UpdateLocation()
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);

                // Updating the controller state and noting that we are not grabbing anything
                grabbedWaypoint = null;
                currentControllerState = ControllerState.IDLE;
            }
        }

        /// <summary>
        /// Checks to see if we are scaling (actual scaling code is in Map Interactions)
        /// </summary>
        private void ScalingChecks()
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                currentControllerState = ControllerState.SCALING;
            }
            else if (currentControllerState == ControllerState.SCALING)
            {
                currentControllerState = ControllerState.IDLE;
            }
        }

        /// <summary>
        /// This handles the Selection Pointer toggling, which is activated by the right grip trigger.
        /// Both grip triggers at the same time means we are scaling.
        /// </summary>
        private void SelectionPointerChecks()
        {
            if (currentControllerState == ControllerState.IDLE
                && OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)
                && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                toggleRaycastOn();
                currentControllerState = ControllerState.POINTING; // Switch to the controller's pointing state
            }

            if ((currentControllerState == ControllerState.POINTING             // Checking for releasing grip
                && OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger)) ||
                (currentControllerState == ControllerState.POINTING             // Checking for scaling interaction
                && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)))
            {
                toggleRaycastOff();
                currentControllerState = ControllerState.IDLE; // Switch to the controller's idle state
            }


        }

        /// <summary>
        /// Turn the raycast on and the place point off
        /// </summary>
        private void toggleRaycastOn()
        {
            controller_right.GetComponent<SphereCollider>().enabled = false; // This prevents the raycast from colliding with the grab zone
            grabZone.SetActive(false);
            placePoint.SetActive(false); // Prevents placePoint from blocking raycast
            controller.GetComponent<VRTK_Pointer>().Toggle(true);
        }

        /// <summary>
        /// Turn the raycast off and the place point on
        /// </summary>
        private void toggleRaycastOff()
        {
            controller.GetComponent<VRTK_Pointer>().Toggle(false);
            placePoint.SetActive(true); // turn placePoint back on
            grabZone.SetActive(true);
            controller_right.GetComponent<SphereCollider>().enabled = true; // This prevents the raycast from colliding with the grab zone
        }

        /// <summary>
        /// This handles the primary placement states
        /// </summary>
        private void PrimaryPlacementChecks()
        {
            // Checks for right index Pressed and no waypoint in collision
            if (currentControllerState == ControllerState.IDLE &&
                OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) &&
                mostRecentCollision.type != CollisionType.WAYPOINT)

            {
                currentWaypoint = CreateWaypoint(placePoint.transform.position);

                //Check to make sure we have successfully placed a waypoint
                if (currentWaypoint != null)
                {
                    currentControllerState = ControllerState.PLACING_WAYPOINT;
                }
            }

            // Updates new waypoint location as long as the index is held
            if (currentControllerState == ControllerState.PLACING_WAYPOINT)
            {
                currentWaypoint.gameObjectPointer.transform.position = placePoint.transform.position;
                currentWaypoint.gameObjectPointer.GetComponent<WaypointProperties>().UpdateGroundpointLine();
            }

            // Releases the waypoint when the right index is released
            if (currentControllerState == ControllerState.PLACING_WAYPOINT && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
            {
                UserpointInstruction msg = new UserpointInstruction(currentWaypoint, "MODIFY");
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
                currentControllerState = ControllerState.IDLE;
            }
        }

        /// <summary>
        /// This handles the secondary placement states
        /// </summary>
        private void SecondaryPlacementChecks()
        {
            // Ending the height adjustment
            if (currentControllerState == ControllerState.SETTING_HEIGHT && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                toggleHeightPlaneOff();
                currentControllerState = ControllerState.POINTING;

                if (!OVRInput.Get(OVRInput.RawButton.RHandTrigger))
                {
                    toggleRaycastOff();
                    currentControllerState = ControllerState.IDLE;
                }
            }
            // Initializing groundPoint when pointing and pressing index trigger
            else if (currentControllerState == ControllerState.POINTING && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                if (controller.GetComponent<VRTK_Pointer>().IsStateValid() &&
                    controller.GetComponent<VRTK_StraightPointerRenderer>().GetDestinationHit().point.y < WorldProperties.placementPlane.transform.position.y + 0.1)

                {
                    Vector3 groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetDestinationHit().point;
                    currentWaypoint = (Waypoint)CreateWaypoint(groundPoint);
                    toggleHeightPlaneOn();
                    currentControllerState = ControllerState.SETTING_HEIGHT;
                }
            }
            // Adjusting the height after groundpoint has been placed
            if (currentControllerState == ControllerState.SETTING_HEIGHT)
            {
                AdjustHeight(currentWaypoint);
            }
        }

        /// <summary>
        /// Turn the height plane on
        /// </summary>
        private void toggleHeightPlaneOn()
        {
            controller_right.GetComponent<SphereCollider>().enabled = false;
            grabZone.SetActive(false);
            placePoint.SetActive(false);
            controller.GetComponent<VRTK_Pointer>().Toggle(false);
            heightSelectionPlane.SetActive(true);
        }

        /// <summary>
        /// Turn the height plane off
        /// </summary>
        private void toggleHeightPlaneOff()
        {
            controller.GetComponent<VRTK_Pointer>().Toggle(true);
            placePoint.SetActive(false);
            grabZone.SetActive(false);
            controller_right.GetComponent<SphereCollider>().enabled = false;
            heightSelectionPlane.SetActive(false);
        }

        /// <summary>
        /// This handles setting the height of the new waypoint
        /// </summary>
        /// <param name="newWaypoint"></param>
        private void AdjustHeight(Waypoint newWaypoint)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            Vector3 waypointLocation = newWaypoint.gameObjectPointer.transform.position;

            RaycastHit upHit;
            RaycastHit downHit;

            if (Physics.Raycast(waypointLocation, Vector3.up, out upHit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(waypointLocation, Vector3.up * upHit.distance, Color.yellow);
                //Debug.Log("Did Hit");

                newWaypoint.gameObjectPointer.transform.position = upHit.point;
            }
            else if (Physics.Raycast(waypointLocation, -Vector3.up, out downHit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(waypointLocation, -Vector3.up * downHit.distance, Color.yellow);
                //Debug.Log("Did Hit");
                newWaypoint.gameObjectPointer.transform.position = downHit.point;
            }
            else
            {
                Debug.DrawRay(waypointLocation, Vector3.up * 1000, Color.white);
                Debug.Log("Did not Hit from: " + waypointLocation + " in the direction of " + Vector3.up);
            }
        }

        /// <summary>
        /// Instantiates and returns a new waypoint at the placePoint position.
        /// Modifies behavior to add or insert if we are currently colliding with a line
        /// </summary>
        /// <param name="groundPoint"> This is the location on the ground that the waypoint will be directly above. </param>
        /// <returns></returns>
        private Waypoint CreateWaypoint(Vector3 groundPoint)
        {
            // We will use the placePoint location.
            Vector3 newLocation = new Vector3(groundPoint.x, placePoint.transform.position.y, groundPoint.z);
            Drone currentlySelectedDrone = WorldProperties.selectedDrone; // Grabbing the drone that we are creating this waypoint for

            // Make sure our drone exists
            if (currentlySelectedDrone != null)
            {
                // INSERT
                // Placing a new waypoint in between old ones - triggers if a line is in the selection zone
                if (mostRecentCollision.type == CollisionType.LINE)
                {
                    // Create a new waypoint at that location
                    Waypoint newWaypoint = new Waypoint(currentlySelectedDrone, newLocation);
                    Debug.Log("**creating waypoint at: "+  newLocation + ". This is before Unity to  ROS conversions");

                    // Grabbing the waypoint at the origin of the line (the lines point back towards the start)
                    Waypoint lineOriginWaypoint = mostRecentCollision.waypoint;

                    // Insert the new waypoint into the drone path just behind the lineOriginWaypoint
                    currentlySelectedDrone.InsertWaypoint(newWaypoint, lineOriginWaypoint.prevPathPoint);

                    // Return the waypoint to announce that we successfully created one
                    return newWaypoint;
                }

                // ADD
                // If we don't have a line selected, we default to placing the new waypoint at the end of the path
                else
                {
                    Waypoint newWaypoint = new Waypoint(currentlySelectedDrone, newLocation);

                    // Add the new waypoint to the drone's path
                    currentlySelectedDrone.AddWaypoint(newWaypoint);

                    // Return the waypoint to announce that we successfully created one
                    return newWaypoint;
                }
            }

            // If we have not added or inserted a waypoint, we need to return null
            return null;
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
                currentControllerState = ControllerState.DELETING;

                //Checking to see if we are colliding with one of those
                if (mostRecentCollision.type == CollisionType.WAYPOINT && currentlySelectedDrone.waypoints.Contains(mostRecentCollision.waypoint))
                {
                    // Remove the highlighted waypoint (DELETE)
                    Debug.Log("Removing waypoint in grab zone");

                    Waypoint selectedWaypoint = mostRecentCollision.waypoint;
                    currentlySelectedDrone.DeleteWaypoint(selectedWaypoint);

                    // Remove from collisions zone list and variables
                    currentCollisions.RemoveAll(collision => collision.waypoint == selectedWaypoint &&
                                                collision.type == CollisionType.WAYPOINT);
                    currentCollisions.RemoveAll(collision => collision.waypoint == selectedWaypoint &&
                                            collision.type == CollisionType.LINE);

                    mostRecentCollision.type = CollisionType.NOTHING;
                    mostRecentCollision.waypoint = null;

                    currentControllerState = ControllerState.IDLE;
                }
                else
                {
                    // Otherwise we default to removing the last waypoint (UNDO)
                    Debug.Log("Removing most recently placed waypoint");

                    Waypoint lastWaypoint = (Waypoint)currentlySelectedDrone.waypointsOrder[currentlySelectedDrone.waypointsOrder.Count - 1];

                    // Remove from collisions list
                    currentCollisions.RemoveAll(collision => collision.waypoint == lastWaypoint &&
                                                collision.type == CollisionType.WAYPOINT);
                    currentCollisions.RemoveAll(collision => collision.waypoint == lastWaypoint &&
                                            collision.type == CollisionType.LINE);

                    // Catching edge case in which most recent collision was the last waypoint
                    if (lastWaypoint == mostRecentCollision.waypoint)
                    {
                        mostRecentCollision.type = CollisionType.NOTHING;
                        mostRecentCollision.waypoint = null;
                    }

                    // Delete the waypoint
                    currentlySelectedDrone.DeleteWaypoint(lastWaypoint);
                }
                currentControllerState = ControllerState.IDLE;

            }
        }

        /// <summary>
        /// Print statements to help debug the collisions logic.
        /// </summary>
        private void CollisionsDebug()
        {
            if (mostRecentCollision.waypoint != null)
            {
                Debug.Log("Most Recent Collision: " + mostRecentCollision.type + ", " + mostRecentCollision.waypoint.id);
            }

            string debugString = "All Current Collisions: {";
            foreach (CollisionPair collision in currentCollisions)
            {
                debugString += "(" + collision.type + ", " + collision.waypoint.id + ")";
            }

            debugString += "}";
            Debug.Log(debugString);
        }

        /// <summary>
        /// This class notes the type of collision and the waypoint it is associated with. 
        /// Line collisions are associated with the waypoint they originate from -- all waypoint have lines pointing back to the previous waypoint.
        /// </summary>
        public class CollisionPair : IEquatable<CollisionPair>
        {
            public Waypoint waypoint;
            public CollisionType type;

            public CollisionPair(Waypoint waypoint, CollisionType type)
            {
                this.waypoint = waypoint;
                this.type = type;
            }

            public override bool Equals(object obj)
            {
                var other = obj as CollisionPair;
                if (other == null) return false;
                return Equals(other);
            }

            // This is the method that must be implemented to conform to the 
            // IEquatable contract

            public bool Equals(CollisionPair other)
            {
                if (other.waypoint == this.waypoint && other.type == this.type)
                {
                    return true;
                }
                return false;
            }

            public int GetHashcode()
            {
                return this.waypoint.GetHashCode() * 17 + this.type.GetHashCode();
            }
        }
    }
}
