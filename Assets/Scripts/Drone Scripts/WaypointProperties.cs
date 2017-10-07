namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class WaypointProperties : MonoBehaviour
    {
        public Material unpassedWaypoint;
        public Material passedWaypoint;
        public Material selectedGroundpoint;
        public Material selectedUnpassedLine;
        public Material unselectedUnpassedLine;
        public Material selectedPassedLine;
        public Material unselectedPassedLine;
        public Material selectedGroundpointLine;
        public Material unselectedGroundpointLine;

        public GameObject prevPoint; // Refers to previous waypoint/ drone
        public GameObject referenceDrone; // Refers to drone waypoint is attached to
        private GameObject thisGroundpoint; // groundpoint instantiated under current waypoint

        public bool passed; // Indicates whether this waypoint has been passed by the drone

        public GameObject modelGroundpoint; // Refers to the groundpoint object being instantiated

        private LineRenderer waypointLine;
        private CapsuleCollider lineCollider;

        private GameObject world;
        private GameObject controller;

        private LineRenderer groundpointLine; // Connects the groundpoint to the waypoint

        public bool setInterwaypointToggle;

        void Start()
        {
            passed = false;

            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");

            waypointLine = this.GetComponentInParent<LineRenderer>();
            if (prevPoint != null)
            {
                referenceDrone = prevPoint.GetComponent<WaypointProperties>().referenceDrone;
            }

            lineCollider = new GameObject("Collider").AddComponent<CapsuleCollider>();
            lineCollider.tag = "Line Collider";
            lineCollider.gameObject.AddComponent<WaypointLine>().waypoint = gameObject;
            
            // Commented out due to child collider conflicts with parent collider.
            //lineCollider.transform.parent = waypointLine.transform;

            setInterwaypointToggle = true;

            // Sets up interaction events
            GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(InteractableObjectUngrabbed);
        }

        void Update()
        {
            if (prevPoint != null)
            {
                ResetWaypoint();

                SetPassedState();

                SetLine();
                
                if (thisGroundpoint == null)
                {
                    CreateGroundpoint();
                }             

                CreateWaypointIndicator();

                ChangeColor();
            }
        }

        // Positions line between waypoints and drones
        public void SetLine()
        {
            if (prevPoint != null) // For all waypoints past the first one;
            {
                waypointLine.SetPosition(0, this.transform.position);

                Vector3 endpoint = new Vector3();
                if (referenceDrone.GetComponent<MoveDrone>().targetWaypoint != this.gameObject || passed)
                {
                    endpoint = prevPoint.transform.position;
                    waypointLine.SetPosition(1, endpoint);
                } else
                {
                    endpoint = referenceDrone.transform.position;
                    waypointLine.SetPosition(1, endpoint);
                }
                SetLineCollider(endpoint);

                // If line being selected by controller
                if (controller.GetComponent<VRTK_StraightPointerRenderer>().lineSelected == this.gameObject && referenceDrone.GetComponent<SetWaypoint>().selected)
                {
                    waypointLine.startWidth = world.GetComponent<ControllerInteractions>().actualScale.y / 100;
                    waypointLine.endWidth = world.GetComponent<ControllerInteractions>().actualScale.y / 100;

                    if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        if (setInterwaypointToggle)
                        {
                            SetInterwaypoint();
                        }
                    } else
                    {
                        setInterwaypointToggle = true;
                    }
                }
                else
                {
                    waypointLine.startWidth = world.GetComponent<ControllerInteractions>().actualScale.y / 200;
                    waypointLine.endWidth = world.GetComponent<ControllerInteractions>().actualScale.y / 200;
                }
            }
        }
        
        // Places a collider around the waypoint line
        public void SetLineCollider(Vector3 endpoint)
        {
            if (passed)
            {
                Destroy(lineCollider);
            } else if (referenceDrone.GetComponent<SetWaypoint>().selected)
            {
                lineCollider.transform.parent = waypointLine.transform;
                lineCollider.radius = world.GetComponent<ControllerInteractions>().actualScale.y / 50;
                lineCollider.center = Vector3.zero;
                lineCollider.transform.position = (endpoint + this.gameObject.transform.position) / 2;
                lineCollider.direction = 2;
                lineCollider.transform.LookAt(this.gameObject.transform, Vector3.up);
                lineCollider.height = (endpoint - this.transform.position).magnitude;
                lineCollider.transform.parent = world.transform;
            }
        }

        // Creates the groundpoint under waypoint
        public void CreateGroundpoint()
        {
            if (groundpointLine != null)
                Destroy(groundpointLine);

            Vector3 groundpoint = new Vector3(this.transform.position.x, world.transform.position.y + modelGroundpoint.transform.localScale.y, this.transform.position.z);
            thisGroundpoint = Instantiate(modelGroundpoint, groundpoint, Quaternion.identity);
            thisGroundpoint.transform.localScale = world.GetComponent<ControllerInteractions>().actualScale / 100;
            thisGroundpoint.transform.parent = world.transform;
            groundpointLine = thisGroundpoint.GetComponent<LineRenderer>();
        }

        // Creates a new Waypoint Indicator
        public void CreateWaypointIndicator()
        {
            groundpointLine.SetPosition(0, thisGroundpoint.transform.position);
            groundpointLine.SetPosition(1, this.transform.position);
            groundpointLine.startWidth = world.GetComponent<ControllerInteractions>().actualScale.y / 400;
            groundpointLine.endWidth = world.GetComponent<ControllerInteractions>().actualScale.y / 400;
            if (referenceDrone.GetComponent<SetWaypoint>().selected)
            {
                groundpointLine.material = selectedGroundpointLine;
            } else
            {
                groundpointLine.material = unselectedGroundpointLine;
            }
        }

        // Changes the colors of waypoints and lines based on their selected and passed states
        public void ChangeColor()
        {
            if (passed)
            {
                this.GetComponent<MeshRenderer>().material = passedWaypoint;
                if (referenceDrone.GetComponent<SetWaypoint>().selected)
                {
                    waypointLine.material = selectedPassedLine;
                }
                else
                {
                    waypointLine.material = unselectedPassedLine;
                }
            } else if (controller.GetComponent<VRTK_StraightPointerRenderer>().lineSelected == this.gameObject && referenceDrone.GetComponent<SetWaypoint>().selected)
            {
                waypointLine.material = unpassedWaypoint;
            } else
            {
                this.GetComponent<MeshRenderer>().material = unpassedWaypoint;
                if (referenceDrone.GetComponent<SetWaypoint>().selected)
                {
                    waypointLine.material = selectedUnpassedLine;
                }
                else
                {
                    waypointLine.material = unselectedUnpassedLine;
                }
            }
        }

        // Destroys groundpoint when waypoint is destroyed
        public void OnDestroy()
        {
            Destroy(thisGroundpoint);
        }

        // Sets this waypoint's passed state
        public void SetPassedState()
        {
            if (!passed && referenceDrone.transform.position == this.transform.position)
            {
                passed = true;
            }
        }

        // Sets a new waypoint in between two old ones
        public void SetInterwaypoint()
        {
            controller.GetComponent<VRTK_StraightPointerRenderer>().OnClick();
            referenceDrone.GetComponent<SetWaypoint>().settingInterWaypoint = true;
            referenceDrone.GetComponent<SetWaypoint>().interWaypoint = this.gameObject;
            setInterwaypointToggle = false;
        }

        public void ResetWaypoint()
        {
            if (OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) == this.transform.position)
            {
                Debug.Log("move");
            }
            //Debug.Log(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
            //Debug.Log(this.transform.position);
        }

        void InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
        {
            CreateGroundpoint();
        }
    }
}
