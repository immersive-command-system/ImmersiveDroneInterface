namespace VRTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SetWaypoint : MonoBehaviour {

        public static GameObject currentDrone;
        public GameObject drone; // Drone object
        public GameObject waypoint; // Waypoint object
        public static GameObject controller_right;
     

        public float maxHeight; // maximum height waypoint can be at when adjusting
        public bool selected; // Indicated if the drone is selected
        public bool toggleDeselectOtherDrones;
        public Material selectedMaterial;
        public Material deselectedMaterial;
        public static ArrayList waypointOrder; //Keeps Track of Waypoints in the Order they were created for the undo function
        public static ArrayList waypoints;
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

        public bool settingInterWaypoint;
        private static bool currentlySetting = false;
        public GameObject interWaypoint;

        private static GameObject ghostPoint; // Place waypoint in front of controller
        private GameObject currentWaypoint; // The current waypoint we are trying to place
        private bool placeAtHand = false; // Are we placing the waypoint at our hand?
        public Material ghostMaterial;
        public Material adjustMaterial;
        private static bool setWaypointState = false;

        void Start()
        {
            controller_right = GameObject.Find("controller_right");
            selected = true;
            adjustingHeight = false;
            actualScale = new Vector3(0, 0, 0);
            currentScale = new Vector3(0, 0, 0);
            waypoints = new ArrayList(0);
            waypointOrder = new ArrayList(0);
            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");
            settingInterWaypoint = false;

            ghostPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ghostPoint.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            ghostPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            ghostPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            ghostPoint.SetActive(true);
        }

        
        void Update()
        {
            currentDrone = drone;
            if (selected)
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
                    settingInterWaypoint = false;
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
            // Sets the first waypoint at drone's position if none exist yet.
            if (waypoints.Count == 0) 
            {
                GameObject startWaypoint = Instantiate(waypoint, this.transform.position, Quaternion.identity);
                startWaypoint.tag = "waypoint";
                startWaypoint.GetComponent<VRTK_InteractableObject>().ignoredColliders[0] = controller_right.GetComponent<SphereCollider>(); //Ignoring Collider from Controller so that WayPoint Zone is used
                startWaypoint.transform.localScale = actualScale / 100;
                startWaypoint.transform.parent = world.transform;
                startWaypoint.GetComponent<WaypointProperties>().referenceDrone = this.gameObject;
                startWaypoint.GetComponent<MeshRenderer>().material = startWaypointMaterial;
                this.GetComponentInParent<MoveDrone>().prevPoint = startWaypoint;
                waypoints.Add(startWaypoint);
                waypointOrder.Add(startWaypoint);
            }

            // Now we set the actual waypoint at the ghostPoint position.
            groundPoint.y = ghostPoint.transform.position.y;
            GameObject newWaypoint = Instantiate(waypoint, groundPoint, Quaternion.identity);
            newWaypoint.tag = "waypoint";
            newWaypoint.GetComponent<VRTK_InteractableObject>().ignoredColliders[0] = controller_right.GetComponent<SphereCollider>(); //Ignoring Collider from Controller so that WayPoint Zone is used
            newWaypoint.transform.localScale = actualScale / 100;
            newWaypoint.transform.parent = world.transform;

            // INSERT - ROS
            // Placing a new waypoint in between old ones - triggers if a line is selected
            if (settingInterWaypoint) 
            {
                if (controller_right.GetComponent<ControllerInteractions>().LineCollided())
                {
                    interWaypoint = controller_right.GetComponent<ControllerInteractions>().GetInterWaypoint();
                    Debug.Log(interWaypoint);
                }

                int index = waypoints.IndexOf(interWaypoint);
                if (index < 0)
                {
                    index = 0;
                }
                waypoints.Insert(index, newWaypoint);
                waypointOrder.Add(newWaypoint);
                interWaypoint.GetComponent<WaypointProperties>().prevPoint = newWaypoint;
                newWaypoint.GetComponent<WaypointProperties>().prevPoint = (GameObject) waypoints[index - 1];
            }
            // ADD - ROS
            // If we don't have a line selected, we default to placing the new waypoint at the end of the path
            else
            {
                waypoints.Add(newWaypoint);
                waypointOrder.Add(newWaypoint);
                newWaypoint.GetComponent<WaypointProperties>().prevPoint = (GameObject) waypoints[waypoints.Count - 2];
            }

            return newWaypoint;
        }

        // REMOVE - ROS
        // Removes the most recently placed waypoint
        public static void ClearWaypoint()
        {
            
            Debug.Log("removing latest waypoint");
            GameObject latestWayPoint = (GameObject)waypoints[waypoints.Count - 1];
            int tempIndex2 = waypointOrder.IndexOf(latestWayPoint);
            WaypointProperties tempProperties = latestWayPoint.GetComponent<WaypointProperties>();
            tempProperties.deleteLineCollider();
            Destroy(latestWayPoint);
            waypoints.RemoveAt(waypoints.Count - 1); //removing latest waypoint from both lists
            waypointOrder.RemoveAt(tempIndex2);//^

        }

        // REMOVE - ROS
        // Removes a highlighted waypoint
        public static void ClearSpecificWayPoint(GameObject currentWayPoint)
        {
            WaypointProperties tempProperties = currentWayPoint.GetComponent<WaypointProperties>();
            int tempIndex = waypoints.IndexOf(currentWayPoint); //Gets index of Waypoint in waypoint list 
            int tempIndex2 = waypointOrder.IndexOf(currentWayPoint);//Gets index of Waypoint in waypoint Order
            if (waypoints.Count > 0)
            {
                // Checking to see if it is the latest waypoint and calling ClearWaypoint if so...
                if (tempIndex == waypoints.Count - 1 && tempIndex != 0)
                {
                    Debug.Log("Specific Waypoint happened to be the latest");
                    ClearWaypoint();
                }

                // Checking to see if the waypoint is the drone waypoint and deleting the entire drone if so
                if (tempIndex == 0)
                {
                    // If there are no waypoints remaining, we remove the drone.
                    Debug.Log("Destroying drone as last resort");
                    for (int i = 0; i < waypoints.Count; i++)
                    {
                        GameObject forLoopWayPoint = (GameObject)waypoints[i]; //Getting each waypoints
                        tempProperties = forLoopWayPoint.GetComponent<WaypointProperties>(); //Getting each wayPoints properties
                        tempProperties.deleteLineCollider(); // Deleting Line collider attached to waypoint
                        Destroy(forLoopWayPoint); //Deleting waypoint itself 
                    }
                    //Destroy((GameObject) waypoints[0]); // Getting rid of last Waypoint gameObject before destroying Drone
                    Destroy(tempProperties.referenceDrone);
                    waypoints = new ArrayList(0); // resetting both lists 
                    waypointOrder = new ArrayList(0); // ^
                    deactivateGhost();
                }
                else
                {
                    GameObject nextDrone = (GameObject)waypoints[(tempIndex + 1)];

                    Debug.Log("Removing specific waypoint");
                    nextDrone.GetComponent<WaypointProperties>().prevPoint = tempProperties.prevPoint;
                    tempProperties.deleteLineCollider();
                    Destroy((GameObject)waypoints[tempIndex]);
                    waypoints.RemoveAt(tempIndex);
                    waypointOrder.RemoveAt(tempIndex2);
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
            settingInterWaypoint = !ControllerInteractions.secondIndexPressed();
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
                    i.GetComponent<DroneMenuActivator>().selected = false;
                }
            }

            toggleDeselectOtherDrones = false;
        }

        // Destroys the waypoints associated with this drone
        private void OnDestroy()
        {
            foreach (GameObject i in waypoints)
            {
                Destroy(i);
            }
        }

        public static GameObject getCurrentDrone()
        {
            return currentDrone;
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
