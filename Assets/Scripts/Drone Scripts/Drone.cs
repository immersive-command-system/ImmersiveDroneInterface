namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drone
    {

        public GameObject gameObjectPointer; // This is the related game object
        public char id; // This is the identifier of the drone in the dronesDict and across the ROSBridge

        public ArrayList waypoints; // All waypoints held by the drone
        public ArrayList waypointOrder; // Keeps track of waypoints in the order they were created for the undo function

        public int nextWaypointId; // Incrementing counter to give all waypoints a unique ID when combined with the Drone ID
        public static Dictionary<char, Waypoint> waypointsDict; // Collection of the waypoints in this drone's path

        public Drone(GameObject drone_obj)
        {
            // Initializing variables
            gameObjectPointer = drone_obj;
            id = WorldProperties.nextDroneId;

            waypoints = new ArrayList(0);
            waypointOrder = new ArrayList(0);

            nextWaypointId = 0;
            waypointsDict = new Dictionary<char, Waypoint>(); 
        
            // Updating the world properties to reflect a new drone being added
            WorldProperties.dronesDict.Add(id, this);
            WorldProperties.nextDroneId++;

            Debug.Log("Created new drone with id: " + id);
        }

        // Use this to insert a new waypoint into the path
        public void AddWaypoint(Waypoint newWaypoint, Waypoint prevWaypoint)
        {
            //waypointsDict.Add(newWaypoint.id, );
        }
    }
}
