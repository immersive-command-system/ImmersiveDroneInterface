namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Waypoint
    {
        public Drone referenceDrone; // The drone whose path this waypoint belongs to
        public GameObject gameObjectPointer; // This is the related game object
        public ArrayList ROSpoints; // Keeps track of the ROSpoints received over the ROSBridge
        public string id; // This is the identifier of the drone in the dronesDict and across the ROSBridge

        // The PathPoints are used by the line renderer to connect the full path.
        // NOTE: The assignment of these variables is handled by the drone based on how the waypoint is added/removed from the path
        public GameObject prevPathPoint; // Refers to previous waypoint or the drone
        public GameObject nextPathPoint; // Refers to next waypoint or the drone

        public Waypoint(Drone myDrone, Vector3 position)
        {
            // Linking this waypoint to its drone
            referenceDrone = myDrone;

            // Setting up all the related gameObject parameters
            GameObject baseObject = Resources.Load("GameObjects/waypoint");
            gameObjectPointer = Instantiate(baseObject, position, Quaternion.identity);
            gameObjectPointer.tag = "waypoint";
            gameObjectPointer.GetComponent<VRTK_InteractableObject>().ignoredColliders[0] = controller_right.GetComponent<SphereCollider>(); //Ignoring Collider from Controller so that WayPoint Zone is used
            gameObjectPointer.transform.localScale = actualScale / 100; // ?
            gameObjectPointer.transform.parent = world.transform; // ?

            // Initializing the ROSpoints Arraylist
            ROSpoints = new ArrayList(0);

            // Establishing the unique waypoint identifier
            id = "" + referenceDrone.id + referenceDrone.nextWaypointId;
            referenceDrone.nextWaypointId ++;

            Debug.Log("Created new waypoint with id: " + id);
        }
    }
}
