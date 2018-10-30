namespace ISAACS
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    // THis class handles all controller interactions with the waypoints and drone

    public class ControllerInteractions : MonoBehaviour
    {
        public enum ControllerState {IDLE, GRABBING, PLACING_DRONE, PLACING_WAYPOINT, POINTING, SETTING_HEIGHT, SCALING, DELETING, SELECTING_DRONES}; // These are the possible values for the controller's state
        public static ControllerState currentControllerState; // We use this to determine what state the controller is in - and what actions are available

        public enum CollisionType { NOTHING, WAYPOINT, LINE, DRONE, OTHER }; // These are the possible values for objects we could be colliding with
        public CollisionPair mostRecentCollision;
        private List<CollisionPair> currentCollisions;
        private CollisionPair nothingCollision = new CollisionPair(null, CollisionType.NOTHING);

        // These fields signify whether current intention is to select/deselect drones or is undetermined.
        private enum SelectionMode { NONE, SELECTING, DESELECTING};
        private SelectionMode selectionMode = SelectionMode.NONE;

        public GameObject controller_right; // Our right controller
        private GameObject controller; //needed to access pointer

        public GameObject grabZone;
        private static GameObject placePoint; // Place waypoint in front of controller
        private GameObject heightSelectionPlane;

        private GeneralWaypoint currentWaypoint; // The current waypoint we are trying to place
        private GeneralWaypoint grabbedWaypoint; // The current waypoint we are grabbing and moving

        public Material defaultMaterial;
        public Material selectedMaterial;
        public Material opaqueMaterial;
        public Material adjustMaterial;
        public Material placePointMaterial;
        public Material heightSelectionPlaneMaterial;

        /// <summary>
        /// The start method initializes all necessary variables and creates the selection zone (grabZone) and the place point
        /// </summary>
        public void Start()
        {
            // Defining the selection zone variables
            mostRecentCollision = nothingCollision;
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
            gameObject.GetComponent<SphereCollider>().center = grabZone.transform.position;
            
            // Creating the placePoint
            placePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            placePoint.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            placePoint.transform.localPosition = grabZone.transform.position;
            placePoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            placePoint.SetActive(true);

            // Creating the heightSelectionPlane
            heightSelectionPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            heightSelectionPlane.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            heightSelectionPlane.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            heightSelectionPlane.transform.localScale = new Vector3(10f, 0.0001f, 10f);
            heightSelectionPlane.GetComponent<Renderer>().material = heightSelectionPlaneMaterial;
            heightSelectionPlane.gameObject.name = "heightSelectionPlane";
            heightSelectionPlane.layer = 8;
            heightSelectionPlane.SetActive(false);
        }

        /// <summary>
        /// The Update method calls all the various state and collision checks
        /// </summary>
        void Update()
        {
            // SELECTION POINTER  
            SelectionPointerChecks();

            // ADDING A DRONE (PRESS THUMBSTICK)
            AddDroneCheck();

            //ScalingChecks();

            if (WorldProperties.selectedDrones.AreDronesSelected())
            {
                // WAYPOINT GRABBING
                GrabbingChecks();

                // UNDO AND DELETE (B - BUTTON)
                if (OVRInput.GetUp(OVRInput.Button.Two))
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
                GeneralWaypoint collidedWaypoint = currentCollider.gameObject.GetComponent<WaypointProperties>().classPointer;
                if (!currentCollisions.Any(x => (x.type == CollisionType.WAYPOINT && x.waypoint == collidedWaypoint)))
                {
                    //Debug.Log("A waypoint is entering the grab zone");

                    // We automatically default to the most recent waypointCollision
                    mostRecentCollision = new CollisionPair(collidedWaypoint, CollisionType.WAYPOINT);
                    //Debug.Log("New mostRecentCollision is a waypoint - " + mostRecentCollision.waypoint.id);
                    currentCollisions.Add(mostRecentCollision);
                }
            }

            // LINE COLLISION
            // We must have left a waypoint (and had our mostRecentCollision.type set to NOTHING) in order to switch to selecting a line
            else if (currentCollider.tag == "Line Collider")
            {
                // This is the waypoint at the end of the line (the line points back toward the path origin / previous waypoint)
                GeneralWaypoint lineOriginWaypoint = currentCollider.GetComponent<LineProperties>().originWaypoint.GetPrevWaypoint();
                if (!currentCollisions.Any(x => (x.type == CollisionType.LINE && x.waypoint == lineOriginWaypoint)))
                {
                    //Debug.Log("A line is entering the grab zone");
                    currentCollisions.Add(new CollisionPair(lineOriginWaypoint, CollisionType.LINE));
                }

            }

            // DRONE COLLISION
            else if (currentCollider.tag == "Drone")
            {
                // Currently assuming that the collider and the DroneProperties script are both attached to the same game object.
                Drone currentDrone = currentCollider.GetComponent<DroneProperties>().classPointer;
                if (!currentCollisions.Any(x => (x.type == CollisionType.DRONE && x.drone == currentDrone)))
                {
                    //Debug.Log("A drone is entering the grab zone");
                    currentCollisions.Add(new CollisionPair(currentDrone));

                    // Check whether controller state is in valid mode to start drone selection.
                    // Perform drone selection/deselection (by contact with controller object).
                    if (currentControllerState == ControllerState.POINTING || currentControllerState == ControllerState.SELECTING_DRONES)
                    {
                        changeControllerState(ControllerState.SELECTING_DRONES);
                        if (this.selectionMode == SelectionMode.NONE)
                        {
                            this.selectionMode = currentDrone.selected ? SelectionMode.DESELECTING : SelectionMode.SELECTING;
                        }
                        if (this.selectionMode == SelectionMode.SELECTING)
                        {
                            currentDrone.Select();
                        } else if (this.selectionMode == SelectionMode.DESELECTING)
                        {
                            currentDrone.Deselect();
                        }                        
                    }
                }
            }

            // Update mostRecentCollision
            if (currentCollisions.Count > 0)
            {
                mostRecentCollision = currentCollisions[currentCollisions.Count - 1];
            }
            else
            {
                mostRecentCollision = nothingCollision;
            }
        }

        /// <summary>
        /// This function handles objects leaving the selection zone for adapative interactions
        /// </summary>
        /// <param name="currentCollider">  This is the collider for the object leaving our zone </param>
        public void OnTriggerExit(Collider currentCollider)
        {
            //CollisionsDebug();
            if (currentCollider.gameObject.CompareTag("waypoint"))
            {
                GeneralWaypoint collidedWaypoint = currentCollider.gameObject.GetComponent<WaypointProperties>().classPointer;
                currentCollisions.RemoveAll(collision => collision.type == CollisionType.WAYPOINT && collision.waypoint == collidedWaypoint);

                //Debug.Log("A waypoint is leaving the grab zone");
            }
            else if (currentCollider.tag == "Line Collider")
            {
                GeneralWaypoint lineOriginWaypoint = currentCollider.GetComponent<LineProperties>().originWaypoint.GetPrevWaypoint();
                currentCollisions.RemoveAll(collision => collision.type == CollisionType.LINE && collision.waypoint == lineOriginWaypoint);

                //Debug.Log("A line is leaving the grab zone");
            }
            else if (currentCollider.tag == "Drone")
            {
                Drone currentDrone = currentCollider.GetComponent<DroneProperties>().classPointer;
                currentCollisions.RemoveAll(collision => collision.type == CollisionType.DRONE && collision.drone == currentDrone);

                //Debug.Log("A drone is leaving the grab zone");
            }

            // Update most recent collision.
            if (currentCollisions.Count > 0)
            {
                mostRecentCollision = currentCollisions[currentCollisions.Count - 1];
            } else
            {
                mostRecentCollision = nothingCollision;
            }
            
        }

        /// <summary>
        /// Handles the controller state switch to grabbing
        /// </summary>
        private void GrabbingChecks()
        {
            // Check for preconditions for entering GRABBING mode and enter it if necessary.
            if (grabbedWaypoint == null &&
                controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null)
            {
                // Assumption: only objects with WaypointProperties is grabbable by VRTK_InteractGrab
                grabbedWaypoint = controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject().GetComponent<WaypointProperties>().classPointer;
                changeControllerState(ControllerState.GRABBING);
            }

            //Check for preconditions for exiting GRABBING mode and exit it if necessary.
            else if (currentControllerState == ControllerState.GRABBING &&
              controller_right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null)
            {
                // Assumption: if grabbing mode was entered, then it must have been a Waypoint that
                // was grabbed and is stored in grabbedWaypoint

                // Commit location change of the waypoint to the drone or group since the user has
                // now released it.
                if (grabbedWaypoint is Waypoint)
                {
                    Waypoint droneWaypoint = (Waypoint)grabbedWaypoint;
                    droneWaypoint.referenceDrone.OnModifyWaypoint(droneWaypoint);
                } else if (grabbedWaypoint is GroupWaypoint)
                {
                    GroupWaypoint groupPoint = (GroupWaypoint)grabbedWaypoint;
                    WorldProperties.groupedDrones[groupPoint.GetGroupID()].OnModifyWaypoint(groupPoint);
                }
                
                
                // Updating the controller state and noting that we are not grabbing anything
                grabbedWaypoint = null;
                changeControllerState(ControllerState.IDLE);
            } else if (grabbedWaypoint != null && grabbedWaypoint is GroupWaypoint)
            {
                GroupWaypoint groupPoint = (GroupWaypoint)grabbedWaypoint;
                WorldProperties.groupedDrones[groupPoint.GetGroupID()].UpdateIndividualWaypointUI(groupPoint);
            }
        }

        /// <summary>
        /// Checks to see if we are scaling (actual scaling code is in MapInteractions)
        /// </summary>
        private void ScalingChecks()
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                changeControllerState(ControllerState.SCALING);
            } else if (currentControllerState == ControllerState.SCALING)
            {
                changeControllerState(ControllerState.IDLE);
            }
        }

        /// <summary>
        /// Checks whether user has performed action for adding a drone to the scene.
        /// </summary>
        private void AddDroneCheck()
        {
            if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick))
            {
                new Drone(placePoint.transform.position);
            }
        }

        /// <summary>
        /// This handles the Selection Pointer toggling, which is activated by the right grip trigger.
        /// Both grip triggers at the same time means we are scaling.
        /// </summary>
        private void SelectionPointerChecks()
        {
            // Check for preconditions for entering POINTING mode and enter it if appropriate.
            if (currentControllerState == ControllerState.IDLE 
                && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
            {
                changeControllerState(ControllerState.POINTING); // Switch to the controller's pointing state
            }

            // Handle case where controller is already in POINTING or SELECTING_DRONES mode.
            else if (currentControllerState == ControllerState.POINTING || currentControllerState == ControllerState.SELECTING_DRONES)
            {
                // Check for entering back into IDLE mode (release of hand trigger).
                if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
                {
                    changeControllerState(ControllerState.IDLE); // Switch to the controller's idle state
                }
                // Check for also depressing the index trigger to perform beam selection.
                // Actual mode change to SELECTING_DRONES left to CheckForDroneSelection if drone is actually selected.
                else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    CheckForDroneSelection();
                }
                // Reset selection mode on release of the index trigger.
                else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                {
                    selectionMode = SelectionMode.NONE;
                }
            }


        }

        /// <summary>
        /// Perform raycast selection of drones
        /// </summary>
        private void CheckForDroneSelection()
        {
            RaycastHit hit;
            // Check whether beam from controller actually hits something.
            if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, out hit))
            {
                // Check whether beamed object is actually a drone.
                DroneProperties targetDrone = hit.collider.gameObject.GetComponent<DroneProperties>();
                if (targetDrone != null)
                {
                    changeControllerState(ControllerState.SELECTING_DRONES);
                    Drone drone = targetDrone.classPointer;

                    // Set selection mode if it isn't already set, based on whether first drone selected is
                    // not already selected.
                    if (selectionMode == SelectionMode.NONE)
                    {
                        selectionMode = (drone.selected) ? SelectionMode.DESELECTING : SelectionMode.SELECTING;
                    }
                    // Actually perform selection/deselection
                    if (selectionMode == SelectionMode.SELECTING)
                    {
                        drone.Select();
                    }
                    else
                    {
                        drone.Deselect();
                    }
                }
            }
        }

        /// <summary>
        /// Handles changing the currentControllerState
        /// Also handles any clean-up/setup work necessary for enter/leaving certain modes.
        /// </summary>
        private void changeControllerState(ControllerState newState)
        {
            if (newState == currentControllerState)
            {
                return;
            }

            // Turn off UI features pertaining to the old selection mode.
            if (currentControllerState == ControllerState.POINTING || currentControllerState == ControllerState.SELECTING_DRONES)
            {
                toggleRaycastOff();
            } else if (currentControllerState == ControllerState.SETTING_HEIGHT)
            {
                toggleHeightPlaneOff();
            }
            // Turn on UI features pertaining to the new selection mode.
            if (newState == ControllerState.POINTING || newState == ControllerState.SELECTING_DRONES)
            {
                Debug.Log("Mode: Pointing");
                toggleRaycastOn();
                selectionMode = SelectionMode.NONE;
            } else if (newState == ControllerState.SETTING_HEIGHT)
            {
                Debug.Log("Mode: Height Selection");
                toggleHeightPlaneOn();
            } else if (newState == ControllerState.GRABBING)
            {
                Debug.Log("Mode: Grabbing");
            }

            currentControllerState = newState;
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
            // Check for action to add waypoint at current controller position
            // Precondition: controller mode is IDLE and only index trigger is pressed and released.
            if (currentControllerState == ControllerState.IDLE && 
                OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) &&
                mostRecentCollision.type != CollisionType.WAYPOINT)
            {
                currentWaypoint = CreateWaypoint(placePoint.transform.position);

                //Check to make sure we have successfully placed a waypoint
                if (currentWaypoint != null)
                {
                    changeControllerState(ControllerState.PLACING_WAYPOINT);
                }
            }

            // Releases the waypoint when the right index is released 
            // and commit waypoint position change to selected drones.
            else if (currentControllerState == ControllerState.PLACING_WAYPOINT && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (currentWaypoint is Waypoint)
                {
                    Waypoint droneWaypoint = (Waypoint)currentWaypoint;
                    droneWaypoint.referenceDrone.OnModifyWaypoint(droneWaypoint);
                }
                else if (currentWaypoint is GroupWaypoint)
                {
                    GroupWaypoint groupPoint = (GroupWaypoint)currentWaypoint;
                    WorldProperties.groupedDrones[groupPoint.GetGroupID()].OnModifyWaypoint(groupPoint);
                }

                changeControllerState(ControllerState.IDLE);
            }
            // Updates new waypoint location as long as the index is held
            else if (currentControllerState == ControllerState.PLACING_WAYPOINT)
            {
                currentWaypoint.AssignPosition(placePoint.transform.position);
                if (currentWaypoint is GroupWaypoint)
                {
                    WorldProperties.groupedDrones[((GroupWaypoint)currentWaypoint).GetGroupID()].UpdateIndividualWaypointUI(((GroupWaypoint)currentWaypoint));
                }
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
                changeControllerState(ControllerState.POINTING);

                if (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                {
                    changeControllerState(ControllerState.IDLE);
                }
            }
            // Initializing groundPoint when pointing and pressing index trigger
            if (currentControllerState == ControllerState.POINTING && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (controller.GetComponent<VRTK_Pointer>().IsStateValid() && 
                    controller.GetComponent<VRTK_StraightPointerRenderer>().GetDestinationHit().point.y < WorldProperties.placementPlane.transform.position.y+0.1)

                {
                    Vector3 groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetDestinationHit().point;
                    currentWaypoint = CreateWaypoint(groundPoint);
                    changeControllerState(ControllerState.SETTING_HEIGHT);
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
        private void AdjustHeight(GeneralWaypoint newWaypoint)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            Vector3 waypointLocation = newWaypoint.GetPosition();
        
            RaycastHit upHit;
            RaycastHit downHit;

            // Set waypoint position to position of first object with the correct layerMask directly above it.
            if (Physics.Raycast(waypointLocation, Vector3.up, out upHit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(waypointLocation, Vector3.up * upHit.distance, Color.yellow);
                Debug.Log("Did Hit");
                newWaypoint.AssignPosition(upHit.point);
            }
            // Otherwise set waypoint poisition to position of first object below it with the correct layerMask
            else if (Physics.Raycast(waypointLocation, -Vector3.up, out downHit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(waypointLocation, -Vector3.up * downHit.distance, Color.yellow);
                Debug.Log("Did Hit");
                newWaypoint.AssignPosition(downHit.point);
            }
            // Did not find anything with correct layerMask directly above or below waypoint.
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
        private GeneralWaypoint CreateWaypoint(Vector3 groundPoint)
        {
            // Use groundPoint position with placepoint height.
            Vector3 newLocation = new Vector3(groundPoint.x, placePoint.transform.position.y, groundPoint.z);
            SelectedDrones currentSelection = WorldProperties.selectedDrones; // Grabbing the drone that we are creating this waypoint for

            // Make sure our drone exists
            if (currentSelection.AreDronesSelected())
            {
                // INSERT: Placing a new waypoint in between old ones - triggers if a line is in the selection zone
                if (mostRecentCollision.type == CollisionType.LINE)
                {
                    // Find the waypoint before which to insert current waypoint.
                    GeneralWaypoint mostRecentWaypoint = mostRecentCollision.waypoint;
                    GeneralWaypoint newWaypoint = null;

                    Debug.Log("Inserting after " + mostRecentWaypoint);

                    // Create and insert appropriate type of waypoint depending on the type of mostRecentWaypoint.
                    if (mostRecentWaypoint is GroupWaypoint)
                    {
                        GroupWaypoint prevGroupWaypoint = (GroupWaypoint)mostRecentWaypoint;
                        newWaypoint = currentSelection.InsertWayPoint(newLocation, prevGroupWaypoint);
                    } else if (mostRecentWaypoint is Waypoint)
                    {
                        Waypoint prevWaypoint = (Waypoint)mostRecentWaypoint;
                        newWaypoint = currentSelection.InsertWayPoint(newLocation, prevWaypoint);
                    }

                    // Return the waypoint to announce that we successfully created one
                    return newWaypoint;
                }

                // ADD
                // If we don't have a line selected, we default to placing the new waypoint at the end of the path
                else
                {
                    //Waypoint newWaypoint = new Waypoint(currentSelection, newLocation);

                    // Add the new waypoint to the drone's path
                    GeneralWaypoint newWaypoint = currentSelection.AddWayPoint(newLocation);
                    Debug.Log("Adding " + newWaypoint);
                    return newWaypoint;
                }
            }
            return null;
        }

        /// <summary>
        /// This method handles the undo and delete functionality
        /// Removes the waypoint from the scene and from the drone's path
        /// </summary>
        public void UndoAndDeleteWaypoints()
        {
            // Make sure the currently selected drone has waypoints
            if (currentControllerState == ControllerState.IDLE)
            {
                SelectedDrones currentSelection = WorldProperties.selectedDrones;
                changeControllerState(ControllerState.DELETING);

                CollisionPair lastWaypointCollision = currentCollisions.FindLast(collision => collision.type == CollisionType.WAYPOINT);
                //Checking to see if we are colliding with one of those
                if (lastWaypointCollision != null)
                {
                    // Remove the highlighted waypoint (DELETE)

                    GeneralWaypoint selectedWaypoint = mostRecentCollision.waypoint;
                    GeneralWaypoint previousWaypoint = selectedWaypoint.GetPrevWaypoint();
                    if (currentSelection.DeleteWayPoint(selectedWaypoint))
                    {
                        // Remove from collisions zone list and variables
                        currentCollisions.RemoveAll(collision => collision.type == CollisionType.WAYPOINT && collision.waypoint == selectedWaypoint);
                        currentCollisions.RemoveAll(collision => collision.type == CollisionType.LINE && collision.waypoint == previousWaypoint);
                        currentCollisions.RemoveAll(collision => collision.type == CollisionType.LINE && collision.waypoint == selectedWaypoint);

                        mostRecentCollision = (currentCollisions.Count > 0) ? currentCollisions[currentCollisions.Count - 1] : nothingCollision;
                    }
                }
                else
                {
                    //// UNSUPPORTED
                    //// Otherwise we default to removing the last waypoint (UNDO)
                    //Debug.Log("Removing most recently placed waypoint");

                    //Waypoint lastWaypoint = (Waypoint) currentlySelection.waypointsOrder[currentlySelection.waypointsOrder.Count - 1];

                    //// Remove from collisions list
                    //currentCollisions.RemoveAll(collision => collision.waypoint == lastWaypoint &&
                    //                            collision.type == CollisionType.WAYPOINT);
                    //currentCollisions.RemoveAll(collision => collision.waypoint == lastWaypoint &&
                    //                        collision.type == CollisionType.LINE);

                    //// Catching edge case in which most recent collision was the last waypoint
                    //if (lastWaypoint == mostRecentCollision.waypoint)
                    //{
                    //    mostRecentCollision = nothingCollision;
                    //}

                    //// Delete the waypoint
                    //currentlySelection.DeleteWaypoint(lastWaypoint);
                }
                changeControllerState(ControllerState.IDLE);
            }
        }
        
        /// <summary>
        /// Print statements to help debug the collisions logic.
        /// </summary>
        private void CollisionsDebug()
        {
            if (mostRecentCollision.waypoint != null)
            {
                Debug.Log("Most Recent Collision: " + mostRecentCollision.type + ", " + mostRecentCollision.waypoint.ToString());
            }
           
            string debugString = "All Current Collisions: {";
            foreach (CollisionPair collision in currentCollisions)
            {
                debugString += "(" + collision.type + ", " + collision.waypoint.ToString() + ")";
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
            public GeneralWaypoint waypoint = null;
            public Drone drone = null;
            public CollisionType type;

            public CollisionPair(GeneralWaypoint waypoint, CollisionType type)
            {
                this.waypoint = waypoint;
                this.type = type;
            }

            public CollisionPair(Drone drone)
            {
                this.drone = drone;
                this.type = CollisionType.DRONE;
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
                if (this.type != other.type)
                {
                    return false;
                }
                if (this.type == CollisionType.DRONE)
                {
                    return this.drone == other.drone;
                }
                return this.waypoint == other.waypoint;
            }

            public override int GetHashCode()
            {
                return ((this.type == CollisionType.DRONE) ? this.drone.GetHashCode() * 17 : this.waypoint.GetHashCode() * 17) + this.type.GetHashCode();
            }
        }
    }
}