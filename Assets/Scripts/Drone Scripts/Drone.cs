namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using ROSBridgeLib.interface_msgs;

    public class Drone
    {

        public GameObject gameObjectPointer; // This is the related game object
        public string id; // This is the identifier of the drone in the dronesDict and across the ROSBridge
        public string groupID = null;
        public bool selected = false;

        public ArrayList waypoints; // All waypoints held by the drone
        public ArrayList waypointsOrder; // Keeps track of the order in which waypoints were created for the undo function

        public int nextWaypointId; // Incrementing counter to give all waypoints a unique ID when combined with the Drone ID
        public Dictionary<string, Waypoint> waypointsDict; // Collection of the waypoints in this drone's path

        /// <summary>
        /// Constructor method for Drone class objects
        /// </summary>
        /// <param name="drone_obj"> We pass in a Gameobject for the drone -- this will be phased out and the new drone_obj gameObject will be instantiated in this method </param>
        public Drone(Vector3 position)
        {
            // Create gameObject at position
            GameObject baseObject = (GameObject)WorldProperties.worldObject.GetComponent<WorldProperties>().droneBaseObject;
            gameObjectPointer = Object.Instantiate(baseObject, position, Quaternion.identity);
            gameObjectPointer.GetComponent<DroneProperties>().classPointer = this; // Connect the gameObject back to the classObject
            gameObjectPointer.tag = "Drone";
            gameObjectPointer.name = baseObject.name;
            gameObjectPointer.transform.localScale = WorldProperties.actualScale / 5;
            gameObjectPointer.transform.parent = WorldProperties.worldObject.transform;
            WorldProperties.AddClipShader(gameObjectPointer.transform);
            this.gameObjectPointer.transform.Find("group3/Outline").GetComponent<MeshRenderer>().material =
                this.gameObjectPointer.GetComponent<DroneProperties>().deselectedMaterial;

            // Initialize path and placement order lists
            waypoints = new ArrayList(0);
            waypointsOrder = new ArrayList(0);

            // Add waypoints container
            nextWaypointId = 0;
            waypointsDict = new Dictionary<string, Waypoint>();

            // Updating the world properties to reflect a new drone being added
            id = WorldProperties.getNextDroneId();
            WorldProperties.dronesDict.Add(id, this);
            Debug.Log("Created new drone with id: " + id);

            // Select this drone
            this.Select();
        }

        /// <summary>
        /// Use this to add a new Waypoint to the end of the drone's path
        /// </summary>
        /// <param name="newWaypoint"> The Waypoint which is to be added to the end of path </param>        
        public void AddWaypoint(Waypoint newWaypoint)
        {
            string prev_id;

            // Check to see if we need to add the starter waypoint
            if (waypoints.Count < 1)
            {
                //Creating the starter waypoint
                Waypoint startWaypoint = new Waypoint(this, gameObjectPointer.transform.TransformPoint(new Vector3(0, 1, 0)));

                // Otherwise, this is the first waypoint.
                startWaypoint.prevPathPoint = null; // This means the previous point of the path is the Drone.

                // Storing this for the ROS message
                prev_id = "DRONE";

                // Swapping the ids so the order makes sense
                string tempId = startWaypoint.id;
                startWaypoint.id = newWaypoint.id;
                newWaypoint.id = tempId;

                // Adding to dictionary, order, and path list
                waypointsDict.Add(startWaypoint.id, startWaypoint);
                waypoints.Add(startWaypoint);
                waypointsOrder.Add(startWaypoint);

                // Send a special Userpoint message marking this as the start
                UserpointInstruction msg = new UserpointInstruction(
                    startWaypoint.id, "DRONE", 0, 1, 0, "ADD");
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
            } else
            {
                // Otherwise we can add as normal
                Waypoint prevWaypoint = (Waypoint)waypoints[waypoints.Count - 1]; // Grabbing the waypoint at the end of our waypoints path
                newWaypoint.prevPathPoint = prevWaypoint; // setting the previous of the new waypoint
                prevWaypoint.nextPathPoint = newWaypoint; // setting the next of the previous waypoint

                // Storing this for the ROS message
                prev_id = prevWaypoint.id;

                // Adding to dictionary, order, and path list
                waypointsDict.Add(newWaypoint.id, newWaypoint);
                waypoints.Add(newWaypoint);
                waypointsOrder.Add(newWaypoint);
            }

            // Send a generic ROS ADD Update only if this is not the initial waypoint
            if (prev_id != "DRONE") {
                UserpointInstruction msg = new UserpointInstruction(newWaypoint, "ADD");
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
            } else
            {
                // Otherwise we have just set the starter waypoint and still need to create the real waypoint
                this.AddWaypoint(newWaypoint);
            }
        }

        /// <summary>
        /// Use this to insert a new waypoint into the path (between two existing waypoints)
        /// </summary>
        /// <param name="newWaypoint"> The Waypoint which is to be added to the path </param>
        /// <param name="prevWaypoint"> The existing Waypoint just before the one which is to be added to the path </param>
        public void InsertWaypoint(Waypoint newWaypoint, Waypoint prevWaypoint)
        {
            // Adding the new waypoint to the dictionary and placement order
            waypointsDict.Add(newWaypoint.id, newWaypoint);
            waypointsOrder.Add(newWaypoint);

            // Adding the waypoint to the array
            int previousIndex = Mathf.Max(0, waypoints.IndexOf(prevWaypoint));
            int newIndex = previousIndex + 1;
            waypoints.Insert(newIndex, newWaypoint);

            // Inserting into the path linked list by adjusting the next and previous pointers of the surrounding waypoints
            newWaypoint.prevPathPoint = prevWaypoint;
            newWaypoint.nextPathPoint = prevWaypoint.nextPathPoint;

            newWaypoint.prevPathPoint.nextPathPoint = newWaypoint;
            newWaypoint.nextPathPoint.prevPathPoint = newWaypoint;

            //Sending a ROS INSERT Update
            UserpointInstruction msg = new UserpointInstruction(newWaypoint, "INSERT");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
        }

        /// <summary>
        /// Use this to remove a waypoint from the path and from the scene
        /// </summary>
        /// <param name="deletedWaypoint"> The waypoint which is to be deleted </param>
        public void DeleteWaypoint(Waypoint deletedWaypoint)
        {
            Debug.Log("Deleting");
            //Sending a ROS DELETE Update
            string curr_id = deletedWaypoint.id;
            string prev_id = deletedWaypoint.prevPathPoint.id;
            float x = deletedWaypoint.gameObjectPointer.transform.localPosition.x;
            float y = deletedWaypoint.gameObjectPointer.transform.localPosition.y;
            float z = deletedWaypoint.gameObjectPointer.transform.localPosition.z;
            UserpointInstruction msg = new UserpointInstruction(curr_id, prev_id, x, y, z, "DELETE");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);

            // Removing the new waypoint from the dictionary, waypoints array and placement order
            waypointsDict.Remove(deletedWaypoint.id);
            waypoints.Remove(deletedWaypoint);
            waypointsOrder.Remove(deletedWaypoint);

            // Removing from the path linked list by adjusting the next and previous pointers of the surrounding waypoints
            deletedWaypoint.prevPathPoint.nextPathPoint = deletedWaypoint.nextPathPoint;
            // Need to check if this is the last waypoint in the list -- if it has a next or not
            if (deletedWaypoint.nextPathPoint != null)
            {
                deletedWaypoint.nextPathPoint.prevPathPoint = deletedWaypoint.prevPathPoint;
            }

            // Removing line collider
            WaypointProperties tempProperties = deletedWaypoint.gameObjectPointer.GetComponent<WaypointProperties>();
            tempProperties.DeleteLineCollider();

            // Deleting the waypoint gameObject
            Object.Destroy(deletedWaypoint.gameObjectPointer);
        }

        public void OnModifyWaypoint(Waypoint waypoint)
        {
            // Sending a ROS MODIFY
            UserpointInstruction msg = new UserpointInstruction(waypoint, "MODIFY");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
        }

        /// <summary>
        /// Use this to add this drone to the selection and reflect that change in the interface.
        /// </summary>
        public void Select() {
            WorldProperties.selectedDrones.AddDrone(this);
            OnSelect();
        }

        public void OnSelect()
        {
            // Changes the color of the drone to indicate that it has been selected
            this.gameObjectPointer.transform.Find("group3/Outline").GetComponent<MeshRenderer>().material =
                this.gameObjectPointer.GetComponent<DroneProperties>().selectedMaterial;
            this.selected = true;
        }

        /// <summary>
        /// Use this to remove this drone from the selection and reflect that change in the interface.
        /// </summary>
        public void Deselect()
        {
            WorldProperties.selectedDrones.DeselectDrone(this);
            OnDeselect();
        }

        public void OnDeselect()
        {
            this.selected = false;
            this.gameObjectPointer.transform.Find("group3/Outline").GetComponent<MeshRenderer>().material =
                this.gameObjectPointer.GetComponent<DroneProperties>().deselectedMaterial;
        }

        ///// <summary>
        ///// Use this to toggle the selection state of the drone and reflect that change in the interface.
        ///// </summary>
        //public void ToggleSelect()
        //{
        //    if (this.selected)
        //    {
        //        Deselect();
        //    } else
        //    {
        //        Select();
        //    }
        //}

        /// <summary>
        /// Finds the first waypoint in this.waypoints that has the same position.
        /// </summary>
        private int findWaypoint(Vector3 position)
        {
            for (int i = 0; i < this.waypoints.Count; i++)
            {
                if (((Waypoint)this.waypoints[i]).GetPosition() == position)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
