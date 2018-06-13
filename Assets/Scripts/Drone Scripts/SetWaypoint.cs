namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;
    using ROSBridgeLib;
    using ROSBridgeLib.interface_msgs;
    using SimpleJSON;

    public class SetWaypoint : MonoBehaviour {

        public Drone thisDrone; // Drone classObject

        public GameObject waypoint; // Waypoint object
        public static GameObject controller_right;

        public float maxHeight; // maximum height waypoint can be at when adjusting
        public bool toggleDeselectOtherDrones;
        public Material selectedMaterial;
        public Material deselectedMaterial;
        public int order;
        public Material startWaypointMaterial;

        private GameObject controller; // Pointer Controller
        private GameObject world; // Refers to the ground
        private Vector3 groundPoint; // Vector3 indicating where the pointer is pointing on the ground
        private bool firstClickFinished = false;
        private static bool adjustingHeight = false;
        private GameObject adjustingWaypoint; // new instantiated waypoint
        private Vector3 currentScale; // Current Scale of the World
        private Vector3 originalScale; // Scale that the world started in
        public Vector3 actualScale; // currentScale / OriginalScale
        private static bool clearWaypointsToggle;

        public bool settingIntermediateWaypoint;
        private static bool currentlySetting = false;
        public GameObject LineOriginWaypoint;

        private static GameObject ghostPoint; // Place waypoint in front of controller
        private GameObject currentWaypoint; // The current waypoint we are trying to place
        private bool placeAtHand = false; // Are we placing the waypoint at our hand?
        public Material ghostMaterial;
        public Material adjustMaterial;
        private static bool setWaypointState = false;

        void Start()
        {
            thisDrone = gameObject.GetComponent<DroneProperties>().classPointer;

            controller_right = GameObject.Find("controller_right");
            adjustingHeight = false;
            actualScale = new Vector3(0, 0, 0);
            currentScale = new Vector3(0, 0, 0);
            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");
            settingIntermediateWaypoint = false;
            ghostPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ghostPoint.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            ghostPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            ghostPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            ghostPoint.SetActive(true);
        }

        
        void Update()
        {
            if (thisDrone.selected)
            {
                // Changes the color of the drone to indicate that it has been selected
                transform.Find("group3").Find("Outline").GetComponent<MeshRenderer>().material = selectedMaterial;

                // Deselects all other drones in the scene
                if (toggleDeselectOtherDrones)
                {
                    DeselectOtherDrones();
                }
               
                UpdateScale();

                // Prevents the ghost point from blocking the raycast.
                if (ControllerInteractions.IsRaycastOn())
                {
                    deactivateGhost();
                } else
                {
                    activateGhost();
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

                } else if (ControllerInteractions.IsIndexPressed() && !ControllerInteractions.IsGrabbing())
                {
                    activateSetWaypointState();
                }

                // Checking to see if you have let go
                if (currentlySetting && !firstClickFinished && ControllerInteractions.IsIndexReleased())
                {
                    firstClickFinished = true;
                } else if (currentlySetting && !firstClickFinished && placeAtHand)
                {
                    currentWaypoint.transform.position = ghostPoint.transform.position;
                    currentWaypoint.GetComponent<WaypointProperties>().UpdateLine();
          
                }
                // Allows user to adjust the newly placed waypoints height
                if (adjustingHeight && firstClickFinished)
                {
                    if (ControllerInteractions.IsIndexPressed())
                    {
                        activateSetWaypointState();
                    }
                    deactivateGhost();
                    AdjustHeight(adjustingWaypoint);

                } else if (firstClickFinished)
                {
                    firstClickFinished = false;
                    activateSetWaypointState();
                    settingIntermediateWaypoint = false;
                    currentlySetting = false;
                    placeAtHand = false; 
                }
            }

            else
            {
                // Changes the drones color to indicated that it has been unselected
                transform.Find("group3").Find("Outline").GetComponent<MeshRenderer>().material = deselectedMaterial;
                ghostPoint.SetActive(false);
            }
        }

        // Instantiates and returns a new waypoint at the ghostPoint position
        private GameObject CreateWaypoint(Vector3 groundPoint)
        {
            // We will use the ghostpoint location.
            Vector3 newLocation = Vector3(groundPoint.x, ghostPoint.transform.position.y, groundPoint.z);

            // Create a new waypoint at that location
            Waypoint newWaypoint = new Waypoint(thisDrone, newLocation);

            // INSERT
            // Placing a new waypoint in between old ones - triggers if a line is in the grab zone
            if (settingIntermediateWaypoint && controller_right.GetComponent<ControllerInteractions>().LineCollided()) 
            {
                // Grabbing the waypoint at the origin of the line
                Waypoint lineOriginWaypoint = controller_right.GetComponent<ControllerInteractions>().GetLineOriginWaypoint();

                // Insert the waypoint into the drone path
                thisDrone.InsertWaypoint(newWaypoint, lineOriginWaypoint);
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

        // REMOVE - ROS
        // Removes the most recently placed waypoint
        public void ClearWaypoint()
        {
            Debug.Log("removing latest waypoint");
            GameObject latestWayPoint = (GameObject) thisDrone.waypoints[thisDrone.waypoints.Count - 1];
            int tempIndex2 = thisDrone.waypointOrder.IndexOf(latestWayPoint);
            WaypointProperties tempProperties = latestWayPoint.GetComponent<WaypointProperties>();
            tempProperties.deleteLineCollider();
            Destroy(latestWayPoint);
            thisDrone.waypoints.RemoveAt(thisDrone.waypoints.Count - 1); //removing latest waypoint from both lists
            thisDrone.waypointOrder.RemoveAt(tempIndex2);//^
        }

        // REMOVE - ROS
        // Removes a highlighted waypoint
        public void ClearSpecificWayPoint(GameObject currentWayPoint)
        {
            WaypointProperties tempProperties = currentWayPoint.GetComponent<WaypointProperties>();
            int tempIndex = thisDrone.waypoints.IndexOf(currentWayPoint); //Gets index of Waypoint in waypoint list 
            int tempIndex2 = thisDrone.waypointOrder.IndexOf(currentWayPoint);//Gets index of Waypoint in waypoint Order
            if (thisDrone.waypoints.Count > 0)
            {
                // Checking to see if it is the latest waypoint and calling ClearWaypoint if so...
                if (tempIndex == thisDrone.waypoints.Count - 1 && tempIndex != 0)
                {
                    Debug.Log("Specific Waypoint happened to be the latest");
                    ClearWaypoint();
                }

                // Checking to see if the waypoint is the drone waypoint and deleting the entire drone if so
                if (tempIndex == 0)
                {
                    // If there are no waypoints remaining, we remove the drone.
                    Debug.Log("Destroying drone as last resort");
                    for (int i = 0; i < thisDrone.waypoints.Count; i++)
                    {
                        GameObject forLoopWayPoint = (GameObject) thisDrone.waypoints[i]; //Getting each waypoints
                        tempProperties = forLoopWayPoint.GetComponent<WaypointProperties>(); //Getting each wayPoints properties
                        tempProperties.deleteLineCollider(); // Deleting Line collider attached to waypoint
                        Destroy(forLoopWayPoint); //Deleting waypoint itself 
                    }
                    //Destroy((GameObject) waypoints[0]); // Getting rid of last Waypoint gameObject before destroying Drone
                    Destroy(tempProperties.referenceDrone);
                    thisDrone.waypoints = new ArrayList(0); // resetting both lists 
                    thisDrone.waypointOrder = new ArrayList(0); // ^
                    deactivateGhost();
                }
                else
                {
                    GameObject nextDrone = (GameObject) thisDrone.waypoints[(tempIndex + 1)];

                    Debug.Log("Removing specific waypoint");
                    nextDrone.GetComponent<WaypointProperties>().prevPoint = tempProperties.prevPoint;
                    tempProperties.deleteLineCollider();
                    Destroy((GameObject) thisDrone.waypoints[tempIndex]);
                    thisDrone.waypoints.RemoveAt(tempIndex);
                    thisDrone.waypointOrder.RemoveAt(tempIndex2);
                }
            }
        }

        // SECONDARY PLACEMENT METHOD

        // Allows user to select the point on the terrain that the waypoint will appear above.
        private GameObject SetGroundpoint()

        {
            if (ControllerInteractions.IsRaycastOn())
            {

                if (controller.GetComponent<VRTK_StraightPointerRenderer>().OnGround())
                {
                    adjustingHeight = true;
                    groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetGroundPoint();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                groundPoint = ghostPoint.transform.position;
                placeAtHand = true;
            }
            GameObject newWaypoint = CreateWaypoint(groundPoint);
            currentWaypoint = newWaypoint;
            return newWaypoint;
        }

        // Adjusts the height at which the waypoint appears
        private void AdjustHeight(GameObject newWaypoint)
        {
            float groundX = newWaypoint.transform.position.x;
            float groundY = newWaypoint.transform.position.y;
            float groundZ = newWaypoint.transform.position.z;
            float localX = controller.transform.position.x;
            float localY = controller.transform.position.y;
            float localZ = controller.transform.position.z;
            float height = 2.147f + (float)Distance(groundX, groundZ, 0f, 0f, localX, localZ) * (float)Math.Tan(Math.PI * (ControllerInteractions.getLocalControllerRotation(OVRInput.Controller.RTouch).x));
            float heightMin = 2.3f + actualScale.y / 200; //mesh height = 2.147

            height = Math.Min(MaxHeight(), Math.Max(heightMin, height));
            newWaypoint.transform.position = new Vector3(groundX, height, groundZ);

            adjustingHeight = !ControllerInteractions.secondIndexPressed();
            firstClickFinished = !ControllerInteractions.secondIndexPressed();
            settingIntermediateWaypoint = !ControllerInteractions.secondIndexPressed();
            currentlySetting = !ControllerInteractions.secondIndexPressed();
            if (!adjustingHeight)
            {
                activateGhost();
            }

        }

        // UTILITY FUNCTIONS

        // Returns the maximum height that the waypoint can be placed
        private float MaxHeight()
        {
            return (maxHeight * (actualScale.y)) + world.transform.position.y;
        }

        // Updates the scale for placing waypoints and adjusting heights
        private void UpdateScale()
        {
            currentScale = world.transform.localScale;
            originalScale = world.GetComponent<MapInteractions>().originalScale;
            actualScale.x = (currentScale.x / originalScale.x);
            actualScale.y = (currentScale.y / originalScale.y);
            actualScale.z = (currentScale.z / originalScale.z);
        }

        // Returns the actual scale
        public Vector3 GetScale()
        {
            return actualScale;
        }

        // Sets the "selected" field in the SetWaypoint methods of all other instantiated drones to false
        private void DeselectOtherDrones()
        {
            GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
            foreach (GameObject i in drones)
            {
                if (i.gameObject != this.gameObject)
                {
                    i.GetComponent<SetWaypoint>().selected = false;
                }
            }

            toggleDeselectOtherDrones = false;
        }

        // Destroys the waypoints associated with this drone
        private void OnDestroy()
        {
            foreach (GameObject i in thisDrone.waypoints)
            {
                Destroy(i);
            }
        }

        //Set setWaypointState to true
        public static void activateSetWaypointState()
        {

                setWaypointState = true;
            
        }        
        //Set setWaypointState to false
        public static void deactivateSetWaypointState()
        {
                setWaypointState = false;
        }

        public static bool IsAdjustingHeight()
        {
            return adjustingHeight;
        }

        private double Distance(float groundX, float groundZ, float groundY, float controllerY, float controllerX, float controllerZ)
        {
            return Math.Sqrt(Math.Pow((controllerX - groundX), 2) + Math.Pow((controllerY - groundY),2)+ Math.Pow((controllerZ - groundZ), 2));
        }

        //Activate ghost waypoint
        private static void activateGhost()
        {
            ghostPoint.SetActive(true);
        }

        //Deactivate ghost waypoint
        private static void deactivateGhost()
        {
            ghostPoint.SetActive(false);
        }

        //Setting?
        public static bool IsCurrentlySetting()
        {
            return currentlySetting;
        }

    }
}
