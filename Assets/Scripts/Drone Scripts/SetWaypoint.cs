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
        private bool adjustingHeight = false;
        private GameObject adjustingWaypoint; // new instantiated waypoint
        private Vector3 currentScale; // Current Scale of the World
        private Vector3 originalScale; // Scale that the world started in
        public Vector3 actualScale; // currentScale / OriginalScale
        private static bool clearWaypointsToggle;

        public bool settingInterWaypoint;
        public GameObject interWaypoint;

        public GameObject waypointPlacer; // Place waypoint in front of controller
        private static bool currentlySetting = false;
        private static bool doneSetting = true; // Prevent accidental waypoint placement

        private static bool setWaypointState = false;

        void Start()
        {
            selected = true;
            adjustingHeight = false;
            actualScale = new Vector3(0, 0, 0);
            currentScale = new Vector3(0, 0, 0);
            waypoints = new ArrayList(0);
            waypointOrder = new ArrayList(0);
            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");
            settingInterWaypoint = false;

            waypointPlacer = Instantiate(waypoint);
            waypointPlacer.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            waypointPlacer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            waypointPlacer.SetActive(false);
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
                // Allows user to select a groundpoint which a new waypoint will appear above
                if (setWaypointState && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                {
                    adjustingWaypoint = SetGroundpoint();
                    if (adjustingWaypoint == null)
                    {
                        return;
                    }

                    deactivateSetWaypointState();
                    currentlySetting = false;
                    doneSetting = false;
                } else if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                {
                    activateSetWaypointState();
                }
                if (!firstClickFinished && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
                {
                    firstClickFinished = true;
                }
                // Allows user to adjust the newly placed waypoints height
                if (adjustingHeight && firstClickFinished)
                {
                    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        activateSetWaypointState();
                    }
                    AdjustHeight(adjustingWaypoint);
                    doneSetting = true;
                } else if (firstClickFinished)
                {
                    doneSetting = true;
                    firstClickFinished = false;
                    activateSetWaypointState();
                }
                

            }

            else
            {
                // Changes the drones color to indicated that it has been unselected
                transform.Find("group3").Find("Outline").GetComponent<MeshRenderer>().material = deselectedMaterial;
            }
        }


        // Allows user to select where the waypoint will appear above
        private GameObject SetGroundpoint()
        {
            if (controller.GetComponent<VRTK_Pointer>().IsActivationButtonPressed())
            {
                if (controller.GetComponent<VRTK_StraightPointerRenderer>().IsSettingWaypoint())
                {
                    adjustingHeight = true;
                    groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetGroundPoint();
                } else
                {
                    return null;
                }
            } else
            {
                groundPoint = waypointPlacer.transform.position;
            }
            GameObject newWaypoint = CreateWaypoint(groundPoint);
            controller.GetComponent<VRTK_StraightPointerRenderer>().OnClick();
            return newWaypoint;
        }

        // Instantiates and returns a new waypoint
        private GameObject CreateWaypoint(Vector3 groundPoint)
        {
            if (waypoints.Count == 0) // Sets the first waypoint at drone's position
            {
                GameObject startWaypoint = Instantiate(waypoint, this.transform.position, Quaternion.identity);
                startWaypoint.tag = "waypoint";
                startWaypoint.transform.localScale = actualScale / 100;
                startWaypoint.transform.parent = world.transform;
                startWaypoint.GetComponent<WaypointProperties>().referenceDrone = this.gameObject;
                startWaypoint.GetComponent<MeshRenderer>().material = startWaypointMaterial;
                this.GetComponentInParent<MoveDrone>().prevPoint = startWaypoint;
                waypoints.Add(startWaypoint);
                waypointOrder.Add(startWaypoint);
            }

            groundPoint.y = waypointPlacer.transform.position.y;
            GameObject newWaypoint = Instantiate(waypoint, groundPoint, Quaternion.identity);
            newWaypoint.tag = "waypoint";
            newWaypoint.transform.localScale = actualScale / 100;
            newWaypoint.transform.parent = world.transform;
            

            if (settingInterWaypoint) // Placing a new waypoint in between old ones
            {
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
            else // Placing a new waypoint at the end
            {
                waypoints.Add(newWaypoint);
                waypointOrder.Add(newWaypoint);
                newWaypoint.GetComponent<WaypointProperties>().prevPoint = (GameObject) waypoints[waypoints.Count - 2];
            }
            return newWaypoint;
        }

        // Adjusts the height at which the waypoint appears
        private void AdjustHeight(GameObject newWaypoint)
        {
            if (newWaypoint.transform.position.y < groundPoint.y + actualScale.y / 200)
            {
                newWaypoint.transform.position = new Vector3(newWaypoint.transform.position.x, groundPoint.y + actualScale.y / 100, newWaypoint.transform.position.z);
            } else if (newWaypoint.transform.position.y > MaxHeight())
            {
                newWaypoint.transform.position = new Vector3(newWaypoint.transform.position.x, MaxHeight(), newWaypoint.transform.position.z);
            }

            float height = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).x / 40;
            newWaypoint.transform.Translate(0f, height, 0f);

            adjustingHeight = !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            firstClickFinished = !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            settingInterWaypoint = !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        }

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

        // Clears 'furthest' placed waypoint
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

        public static void ClearSpecificWayPoint(GameObject currentWayPoint)
        {
            WaypointProperties tempProperties = currentWayPoint.GetComponent<WaypointProperties>();
            int tempIndex = waypoints.IndexOf(currentWayPoint); //Gets index of Waypoint in waypoint list 
            int tempIndex2 = waypointOrder.IndexOf(currentWayPoint);//Gets index of Waypoint in waypoint Order
            if (waypoints.Count > 0)
            {
                //Checking to see if it is the latest waypoint and calling ClearWaypoint if so...
                if (tempIndex == waypoints.Count - 1 && tempIndex != 0)
                {
                    Debug.Log("Specific Waypoint happened to be the latest");
                    ClearWaypoint();
                    return;
                }

                //Checking to see if the waypoint is the drone waypoint and deleting the entire drone if so
                if (tempIndex == 0)
                {
                    Debug.Log("destroying drone as last resort");
                    Destroy((GameObject) waypoints[0]); // Getting rid of last Waypoint gameObject before destroying Drone
                    Destroy(tempProperties.referenceDrone);
                    waypoints = new ArrayList(0); // resetting both lists 
                    waypointOrder = new ArrayList(0); // ^
                    return;
                }

                GameObject nextDrone = (GameObject) waypoints[(tempIndex + 1)];

                
                Debug.Log("removing specific waypoint");
                nextDrone.GetComponent<WaypointProperties>().prevPoint = tempProperties.prevPoint;
                tempProperties.deleteLineCollider();
                Destroy((GameObject)waypoints[tempIndex]);
                waypoints.RemoveAt(tempIndex);
                waypointOrder.RemoveAt(tempIndex2);
                
            }


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

        // Destroys the drone menu and waypoints associated with this drone
        private void OnDestroy()
        {
            GameObject menu = GameObject.FindGameObjectWithTag("DroneMenu");
            if (menu != null)
            {
                Destroy(menu);
            }

            foreach (GameObject i in waypoints)
            {
                Destroy(i);
            }
        }

        public static GameObject getCurrentDrone()
        {
            return currentDrone;
        }

        //Toggle waypoint placement mode if menu is pressed (called from waypoint adder).
        public static void menuPressed()
        {
            if (doneSetting)
            {
                currentlySetting = true;
            }
        }

        //Set setWaypointState to true
        public static void activateSetWaypointState()
        {
            if (doneSetting)
            {
                setWaypointState = true;
            }
        }        
        //Set setWaypointState to false
        public static void deactivateSetWaypointState()
        {
                setWaypointState = false;
        }


    }
}
