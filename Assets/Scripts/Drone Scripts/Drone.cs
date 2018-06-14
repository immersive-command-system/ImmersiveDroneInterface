namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using ROSBridgeLib;
    using ROSBridgeLib.interface_msgs;

    public class Drone
    {

        public GameObject gameObjectPointer; // This is the related game object
        public char id; // This is the identifier of the drone in the dronesDict and across the ROSBridge
        public bool selected;

        public ArrayList waypoints; // All waypoints held by the drone
        public ArrayList waypointsOrder; // Keeps track of the order in which waypoints were created for the undo function

        public int nextWaypointId; // Incrementing counter to give all waypoints a unique ID when combined with the Drone ID
        public static Dictionary<string, Waypoint> waypointsDict; // Collection of the waypoints in this drone's path

        // Constructor
        public Drone(GameObject drone_obj)
        {
            // Initializing variables
            gameObjectPointer = drone_obj;
            id = WorldProperties.nextDroneId;
            selected = true;

            waypoints = new ArrayList(0);
            waypointsOrder = new ArrayList(0);

            nextWaypointId = 0;
            waypointsDict = new Dictionary<string, Waypoint>(); 
        
            // Updating the world properties to reflect a new drone being added
            WorldProperties.dronesDict.Add(id, this);
            WorldProperties.nextDroneId++;

            //Creating the starter waypoint
            Waypoint startWaypoint = new Waypoint(this, gameObjectPointer.transform.position);
            this.AddWaypoint(startWaypoint);

            Debug.Log("Created new drone with id: " + id);
        }

        /// <summary>
        /// Use this to add a new Waypoint to the end of the drone's path
        /// </summary>
        /// <param name="newWaypoint"> The Waypoint which is to be added to the path </param>        
        public void AddWaypoint(Waypoint newWaypoint)
        {
            string prev_id;

            // Adding to the end of the path linked list by adjusting the previous waypoint
            if (waypoints.Count >= 1) {
                // If there is already another waypoint, we can add as normal
                Waypoint prevWaypoint = (Waypoint) waypoints[waypoints.Count -1]; // Grabbing the waypoint at the end of our waypoints path
                newWaypoint.prevPathPoint = prevWaypoint; // setting the previous of the new waypoint
                prevWaypoint.nextPathPoint = newWaypoint; // setting the next of the previous waypoint

                // Storing this for the ROS message
                prev_id = prevWaypoint.id;
            } else
            {
                // Otherwise, this is the first waypoint.
                newWaypoint.prevPathPoint = null; // This means the previous point of the path is the Drone.

                // Storing this for the ROS message
                prev_id = "DRONE";
            }

            // Adding to dictionary, order, and path list
            waypointsDict.Add(newWaypoint.id, newWaypoint);
            waypoints.Add(newWaypoint);
            waypointsOrder.Add(newWaypoint);

            //Send a ROS ADD Update only if this is not the initial waypoint
            if (prev_id != "DRONE") {
                string curr_id = newWaypoint.id;
                float x = newWaypoint.gameObjectPointer.transform.localPosition.x;
                float y = newWaypoint.gameObjectPointer.transform.localPosition.y;
                float z = newWaypoint.gameObjectPointer.transform.localPosition.z;

                UserpointInstruction msg = new UserpointInstruction(curr_id, prev_id, x, y, z, "ADD");
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
            }
            
        }

        /// <summary>
        /// Use this to insert a new waypoint into the path
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
            int newIndex = previousIndex;
            waypoints.Insert(newIndex, newWaypoint);

            // Inserting into the path linked list by adjusting the next and previous pointers of the surrounding waypoints
            newWaypoint.prevPathPoint = prevWaypoint;
            newWaypoint.nextPathPoint = prevWaypoint.nextPathPoint;
            
            newWaypoint.prevPathPoint.nextPathPoint = newWaypoint;
            newWaypoint.nextPathPoint.prevPathPoint = newWaypoint;

            //Sending a ROS INSERT Update
            string curr_id = newWaypoint.id;
            string prev_id = prevWaypoint.id;
            float x = newWaypoint.gameObjectPointer.transform.localPosition.x;
            float y = newWaypoint.gameObjectPointer.transform.localPosition.y;
            float z = newWaypoint.gameObjectPointer.transform.localPosition.z;

            UserpointInstruction msg = new UserpointInstruction(curr_id, prev_id, x, y, z, "INSERT");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
        }

        // Use this to remove a waypoint from the path and from the scene
        public void DeleteWaypoint(Waypoint deletedWaypoint)
        {
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
            deletedWaypoint.nextPathPoint.prevPathPoint = deletedWaypoint.prevPathPoint;

            // Removing line collider
            WaypointProperties tempProperties = deletedWaypoint.gameObjectPointer.GetComponent<WaypointProperties>();
            tempProperties.deleteLineCollider();

            // Deleting the waypoint gameObject
            Object.Destroy(deletedWaypoint.gameObjectPointer);
        }
    }
}
