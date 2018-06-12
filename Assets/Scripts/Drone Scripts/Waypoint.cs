namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Waypoint
    {
        public GameObject gameObjectPointer; // This is the related game object
        public Drone referenceDrone;
        public ArrayList ROSpoints; // Keeps track of the ROSpoints received over the ROSBridge
        public string id; // This is the identifier of the drone in the dronesDict and across the ROSBridge

        public Waypoint(Drone myDrone, GameObject newWaypoint)
        {
            referenceDrone = myDrone;
            gameObjectPointer = newWaypoint;

            // Establishing the unique waypoint identifier
            id = "" + referenceDrone.id + referenceDrone.nextWaypointId;
            referenceDrone.nextWaypointId ++;

            Debug.Log("Created new waypoint with id: " + id);
        }
    }
}
