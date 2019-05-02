namespace ISAACS
{
    using UnityEngine;
    using VRTK;


    /// <summary>
    /// This class handles the rendering of a waypoint's groundpoint and path line
    /// </summary>
    public class WaypointProperties : MonoBehaviour
    {
        public GeneralWaypoint classPointer;
        public Drone referenceDrone = null;
        public DroneGroup referenceGroup = null;
        public GameObject referenceDroneGameObject;
        private GameObject prevPoint = null;

        public Material unpassedWaypoint;
        public Material passedWaypoint;
        public Material selectedGroundpoint;
        public Material selectedUnpassedLine;
        public Material unselectedUnpassedLine;
        public Material selectedPassedLine;
        public Material unselectedPassedLine;
        public Material selectedGroundpointLine;
        public Material unselectedGroundpointLine;

        public bool passed; // Indicates whether this waypoint has been passed by the drone

        public GameObject modelGroundpoint; // Refers to the groundpoint object being instantiated
        private GameObject thisGroundpoint; // groundpoint instantiated under current waypoint
        private LineRenderer groundpointLine; // Connects the groundpoint to the waypoint

        private LineRenderer LineProperties;
        private GameObject lineElement = null;
        private CapsuleCollider lineCollider;
        private float lineOpacity = 1;

        private GameObject world;
        private GameObject controller;

        public static GameObject controller_right;


        void Start()
        {
            passed = false;
            


            if (classPointer != null)
            {
                if (classPointer is Waypoint)
                {
                    referenceDrone = ((Waypoint)classPointer).referenceDrone;
                    referenceDroneGameObject = referenceDrone.gameObjectPointer;
                } else if (classPointer is GroupWaypoint)
                {
                    referenceGroup = WorldProperties.groupedDrones[((GroupWaypoint)classPointer).GetGroupID()];
                }
                
            }

            Debug.Log("Wapoint Properties for drone: " + referenceDrone.id);
            unselectedGroundpointLine = referenceDrone.droneMaterial;
            unselectedUnpassedLine = referenceDrone.droneMaterial;
            unpassedWaypoint = referenceDrone.droneMaterial;

            //Debug.Log("Creating line collider for " + classPointer);

            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");
            controller_right = GameObject.Find("controller_right");

            LineProperties = this.GetComponent<LineRenderer>();
            lineCollider = new GameObject("Line Collider").AddComponent<CapsuleCollider>();
            lineCollider.tag = "Line Collider";
            lineCollider.isTrigger = true;
            lineCollider.radius = 0.1f;
            lineCollider.gameObject.AddComponent<LineProperties>().originWaypoint = classPointer;
            lineCollider.transform.parent = this.gameObject.transform;

            if (classPointer.GetPrevWaypoint() != null)
            {
                // Establishing the previous point in the path. (Null if it is the drone)
                prevPoint = classPointer.GetPrevWaypoint().gameObjectPointer;

                // Create the collider around the line renderer
                SetLineCollider();
            }

            CreateGroundpoint();

            // Sets up interaction events
            GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(InteractableObjectUngrabbed);
        }

        void Update()
        {
            // Establishing the previous point in the path. (could be the drone)
            if (classPointer.GetPrevWaypoint() != null)
            {
                prevPoint = classPointer.GetPrevWaypoint().gameObjectPointer;
            } else
            {
                if (prevPoint != null)
                {
                    DeleteLineCollider();
                }
                prevPoint = null;
            }

            
            SetPassedState();

            if (prevPoint != null)
            {
                SetLine();

                UpdateLineCollider();
            }          

            CreateWaypointIndicator();

            ChangeColor();

            UpdateGroundpointLine();
        }

        // Positions line between waypoints and drones
        public void SetLine()
        {
            if (prevPoint != null)
            {
            
                LineProperties.SetPosition(0, prevPoint.transform.position);

                //if (referenceDroneGameObject.GetComponent<MoveDrone>().targetWaypoint != this.gameObject || passed)
                //{
                //    endpoint = prevPoint.transform.position;
                //} else
                //{
                //    endpoint = referenceDroneGameObject.transform.position;
                //}
                //LineProperties.SetPosition(1, endpoint);

                LineProperties.SetPosition(1, transform.position);

                LineProperties.startWidth = world.GetComponent<MapInteractions>().actualScale.y / 200;
                LineProperties.endWidth = world.GetComponent<MapInteractions>().actualScale.y / 200;
                LineProperties.enabled = classPointer.IsVisible();
            }
        }

        public void SetLineOpacity(float newOpacity)
        {
            this.lineOpacity = Mathf.Clamp01(newOpacity);
        }
        
        // Places a collider around the waypoint line
        public void SetLineCollider()
        {
            Vector3 endpoint = prevPoint.transform.position;

            lineCollider.transform.parent = LineProperties.transform;
            lineCollider.radius = world.GetComponent<MapInteractions>().actualScale.y / 50;
            lineCollider.center = Vector3.zero;
            lineCollider.transform.position = (endpoint + this.gameObject.transform.position) / 2;
            lineCollider.direction = 2;
            lineCollider.transform.LookAt(this.gameObject.transform, Vector3.up);
            lineCollider.height = (endpoint - this.transform.position).magnitude;
            lineCollider.transform.parent = world.transform;
        }

        // Places a collider around the waypoint line
        public void UpdateLineCollider()
        {
            if (lineCollider.enabled && !classPointer.IsInteractable())
            {
                controller_right.GetComponent<ControllerInteractions>().OnTriggerExit(lineCollider);
            }
            lineCollider.enabled = classPointer.IsInteractable();
            if (lineCollider.enabled)
            {
                Vector3 endpoint = prevPoint.transform.position;
                lineCollider.transform.position = (endpoint + this.gameObject.transform.position) / 2;
                lineCollider.transform.LookAt(this.gameObject.transform, Vector3.up);
                lineCollider.height = (endpoint - this.transform.position).magnitude;
            }
        }

        // Creates the groundpoint under waypoint
        public void CreateGroundpoint()
        {
            if (groundpointLine != null)
                Destroy(groundpointLine);

            Vector3 groundpoint = new Vector3(this.transform.position.x, world.transform.position.y + modelGroundpoint.transform.localScale.y, this.transform.position.z);
            thisGroundpoint = Instantiate(modelGroundpoint, groundpoint, Quaternion.identity);
            thisGroundpoint.transform.localScale = world.GetComponent<MapInteractions>().actualScale / 100;
            thisGroundpoint.transform.parent = world.transform;
            groundpointLine = thisGroundpoint.GetComponent<LineRenderer>();
        }

        // Creates a new Waypoint Indicator
        public void CreateWaypointIndicator()
        {
            groundpointLine.SetPosition(0, thisGroundpoint.transform.position);
            groundpointLine.SetPosition(1, this.transform.position);
            groundpointLine.startWidth = world.GetComponent<MapInteractions>().actualScale.y / 400;
            groundpointLine.endWidth = world.GetComponent<MapInteractions>().actualScale.y / 400;
            if (isReferenceSelected())
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
                if (isReferenceSelected())
                {
                    LineProperties.material = selectedPassedLine;

                }
                else
                {
                    LineProperties.material = unselectedPassedLine;

                }
            } else if (( controller_right.GetComponent<ControllerInteractions>().mostRecentCollision.waypoint != null && 
                controller_right.GetComponent<ControllerInteractions>().mostRecentCollision.waypoint.gameObjectPointer == prevPoint) &&
                isReferenceSelected())
            {
                LineProperties.material = unpassedWaypoint;
            } else
            {
                this.GetComponent<MeshRenderer>().material = unpassedWaypoint;
                if (isReferenceSelected())
                {
                    LineProperties.material = selectedUnpassedLine;
                    if (lineElement != null)
                    {
                        lineElement.GetComponent<MeshRenderer>().material = selectedUnpassedLine;
                    }
                }
                else
                {
                    LineProperties.material = unselectedUnpassedLine;
                    if (lineElement != null)
                    {
                        lineElement.GetComponent<MeshRenderer>().material = unselectedUnpassedLine;
                    }
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
            if (!passed && referenceDroneGameObject && referenceDroneGameObject.transform.position == this.transform.position)
            {
                passed = true;
            }
        }

        void InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
        {
            //CreateGroundpoint();
        }

        public void DeleteLineCollider()
        {
            Debug.Log("Destroying line collider for " + classPointer);
            Destroy(lineCollider.gameObject);
            Destroy(LineProperties);

        }

        //Update groundpoint line 
        public void UpdateGroundpointLine()
        { 
            if (thisGroundpoint == null) {
                return;
            }

            Vector3 groundPointLocation = new Vector3(this.transform.position.x, world.transform.position.y + modelGroundpoint.transform.localScale.y, this.transform.position.z);
            thisGroundpoint.transform.position = groundPointLocation;
            groundpointLine = thisGroundpoint.GetComponent<LineRenderer>();
            groundpointLine.enabled = classPointer.IsVisible();
        }

        private bool isReferenceSelected()
        {
            return ((referenceDrone != null && referenceDrone.selected) || (referenceGroup != null && referenceGroup.selected));
        }
    }
}
