namespace VRTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SetWaypoint : MonoBehaviour {

        public GameObject drone; // Drone object
        public GameObject waypoint; // Waypoint object    
        public float maxHeight; // maximum height waypoint can be at when adjusting
        public bool selected; // Indicated if the drone is selected
        public bool toggleDeselectOtherDrones;
        public Material selectedMaterial;
        public Material deselectedMaterial;
        public ArrayList waypoints;
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
        private bool clearWaypointsToggle;

        public bool settingInterWaypoint;
        public GameObject interWaypoint;

        void Start()
        {
            selected = true;
            adjustingHeight = false;
            actualScale = new Vector3(0, 0, 0);
            currentScale = new Vector3(0, 0, 0);
            waypoints = new ArrayList(0);
            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");
            settingInterWaypoint = false;
        }

        void Update()
        {
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
                if (controller.GetComponent<VRTK_StraightPointerRenderer>().IsSettingWaypoint() && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                {
                    adjustingWaypoint = SetGroundpoint();
                    adjustingHeight = true;
                }
                if (adjustingHeight && !firstClickFinished && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
                {
                    firstClickFinished = true;
                }
                // Allows user to adjust the newly placed waypoints height
                if (adjustingHeight && firstClickFinished)
                {
                    AdjustHeight(adjustingWaypoint);                    
                }

                // Allows user to clear the most recently placed waypoint
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                {
                    if (clearWaypointsToggle)
                    {
                        ClearWaypoint();
                    }
                } else
                {
                    clearWaypointsToggle = true;
                }
            } else
            {
                // Changes the drones color to indicated that it has been unselected
                transform.Find("group3").Find("Outline").GetComponent<MeshRenderer>().material = deselectedMaterial;
            }
        }

        // Allows user to select where the waypoint will appear above
        private GameObject SetGroundpoint()
        {
            groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetGroundPoint();
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
            }

            groundPoint.y = drone.transform.position.y;
            GameObject newWaypoint = Instantiate(waypoint, groundPoint, Quaternion.identity);
            newWaypoint.tag = "waypoint";
            newWaypoint.transform.localScale = actualScale / 100;
            newWaypoint.transform.parent = world.transform;

            if (settingInterWaypoint) // Placing a new waypoint in between old ones
            {
                int index = waypoints.IndexOf(interWaypoint);
                waypoints.Insert(index, newWaypoint);
                interWaypoint.GetComponent<WaypointProperties>().prevPoint = newWaypoint;
                newWaypoint.GetComponent<WaypointProperties>().prevPoint = (GameObject) waypoints[index - 1];
            }
            else // Placing a new waypoint at the end
            {
                waypoints.Add(newWaypoint);
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
            originalScale = world.GetComponent<ControllerInteractions>().originalScale;
            actualScale.x = (currentScale.x / originalScale.x);
            actualScale.y = (currentScale.y / originalScale.y);
            actualScale.z = (currentScale.z / originalScale.z);
        }

        // Clears currently placed waypoints
        private void ClearWaypoint()
        {
            if (waypoints.Count > 0)
            {
                Destroy((GameObject)waypoints[waypoints.Count - 1]);
                waypoints.RemoveAt(waypoints.Count - 1);
                clearWaypointsToggle = false; 
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
    }
}
