namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drone
    {
        public GameObject gameObjectPointer; // This is the related game object
        public char id; // This is the identifier of the drone in the dronesDict and across the ROSBridge
        public ArrayList waypointOrder; // Keeps Track of Waypoints in the Order they were created for the undo function
        public ArrayList waypoints; // All waypoints held by the drone
        public int nextWaypointId; // Incrementing counter to give all waypoints a unique ID when combined with the Drone ID

        public Drone(GameObject drone_obj)
        {
            gameObjectPointer = drone_obj;
            waypoints = new ArrayList(0);
            waypointOrder = new ArrayList(0);
            nextWaypointId = 0;
            id = WorldProperties.nextDroneId;
        
            // Updating the world properties to reflect a new drone being added
            WorldProperties.dronesDict.Add(id, this);
            WorldProperties.nextDroneId++;

            Debug.Log("Created new drone with id: " + id);
        }
    }
}
