namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    public class Waypoint
    {
        public Drone referenceDrone; // The drone whose path this waypoint belongs to
        public GameObject gameObjectPointer; // This is the related game object
        public ArrayList ROSpoints; // Keeps track of the ROSpoints received over the ROSBridge
        public string id; // This is the identifier of the drone in the dronesDict and across the ROSBridge

        // The PathPoints are used by the line renderer to connect the full path.
        // NOTE: The assignment of these variables is handled by the drone based on how the waypoint is added/removed from the path
        public Waypoint prevPathPoint; // Refers to previous waypoint
        public Waypoint nextPathPoint; // Refers to next waypoint

        /// <summary>
        /// Waypoint class object constructor
        /// </summary>
        /// <param name="myDrone"> This is the drone that the new waypoint should belong to (remember that you still need to add the waypoint to the path using this drone's methods) </param>
        /// <param name="position"> This is the 3d location at which the new waypoint gameObject should be placed. </param>
        public Waypoint(Drone myDrone, Vector3 position)
        {
            // Linking this waypoint to its drone
            referenceDrone = myDrone;

            // Setting up all the related gameObject parameters
            GameObject baseObject = (GameObject)WorldProperties.worldObject.GetComponent<WorldProperties>().waypointBaseObject;
            gameObjectPointer = Object.Instantiate(baseObject, position, Quaternion.identity);
            gameObjectPointer.GetComponent<VRTK_InteractableObject>().ignoredColliders[0] = GameObject.Find("controller_right").GetComponent<SphereCollider>(); //Ignoring Collider from Controller so that WayPoint Zone is used
            gameObjectPointer.GetComponent<WaypointProperties>().classPointer = this; // Connect the gameObject back to the classObject
            gameObjectPointer.tag = "waypoint";
            gameObjectPointer.name = baseObject.name;
            gameObjectPointer.transform.localScale = WorldProperties.actualScale / 100;
            gameObjectPointer.transform.parent = WorldProperties.worldObject.transform;
            WorldProperties.AddClipShader(gameObjectPointer.transform);

            // Initializing the ROSpoints Arraylist
            ROSpoints = new ArrayList(0);

            // Establishing the unique waypoint identifier
            id = "" + referenceDrone.id + referenceDrone.nextWaypointId;
            referenceDrone.nextWaypointId ++;

            Debug.Log("Created new waypoint with id: " + id);
        }

        public void UpdateLineColliders()
        {
            gameObjectPointer.GetComponent<WaypointProperties>().SetLineCollider();
            if (nextPathPoint != null)
            {
                nextPathPoint.gameObjectPointer.GetComponent<WaypointProperties>().SetLineCollider();
            }
        }
    }
}
