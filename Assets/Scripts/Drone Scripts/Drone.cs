namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drone
    {

        public GameObject gameObjectPointer; // This is the related game object
        public char id; // This is the identifier of the drone in the dronesDict and across the ROSBridge
        public bool selected;

        public ArrayList waypoints; // All waypoints held by the drone
        public ArrayList waypointOrder; // Keeps track of the order in which waypoints were created for the undo function

        public int nextWaypointId; // Incrementing counter to give all waypoints a unique ID when combined with the Drone ID
        public static Dictionary<string, Waypoint> waypointDict; // Collection of the waypoints in this drone's path

        // Constructor
        public Drone(GameObject drone_obj)
        {
            // Initializing variables
            gameObjectPointer = drone_obj;
            id = WorldProperties.nextDroneId;
            selected = true;

            waypoints = new ArrayList(0);
            waypointOrder = new ArrayList(0);

            nextWaypointId = 0;
            waypointDict = new Dictionary<string, Waypoint>(); 
        
            // Updating the world properties to reflect a new drone being added
            WorldProperties.dronesDict.Add(id, this);
            WorldProperties.nextDroneId++;

            //Creating the starter waypoint
            Waypoint startWaypoint = new Waypoint(this, gameObjectPointer.transform.position);
            this.AddWaypoint(startWaypoint);

            Debug.Log("Created new drone with id: " + id);
        }

        // Use this to add a new waypoint to the end of the path
        public void AddWaypoint(Waypoint newWaypoint)
        {
            waypointDict.Add(newWaypoint.id, newWaypoint);
            waypoints.Add(newWaypoint);
            waypointOrder.Add(newWaypoint);

            //Sending a ROS ADD Update
            UserpointInstruction msg = new UserpointInstruction(newWaypoint.transform.localPosition.x, newWaypoint.transform.localPosition.y, newWaypoint.transform.localPosition.z);
            world.GetComponent<ROSDroneConnection>().PublishWaypointUpdateMessage(msg);
        }

        // Use this to insert a new waypoint into the path
        public void InsertWaypoint(Waypoint newWaypoint, Waypoint prevWaypoint)
        {
            // Adding the new waypoint to the dictionary and placement order
            waypointDict.Add(newWaypoint.id, newWaypoint);
            waypointOrder.Add(newWaypoint);

            // Adding the waypoint to the array
            int previousIndex = Mathf.Max(0, waypoints.IndexOf(lineOriginWaypoint));
            int newIndex = previousIndex + 1;
            waypoints.Insert(newIndex, newWaypoint);

            // Inserting into the path linked list by adjusting the next and previous pointers of the surrounding waypoints
            newWaypoint.prevPathPoint = prevWayPoint.gameObjectPointer;
            newWaypoint.nextPathPoint = prevPathPoint.nextPathPoint;

            prevWaypoint.nextPathPoint.prev = newWaypoint.gameObjectPointer;
            prevWaypoint.nextPathPoint = newWaypoint.gameObjectPointer;

            //Sending a ROS INSERT Update
        }
    }
}
