namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class WorldProperties : MonoBehaviour
    {
        public GameObject droneBaseObject;
        public GameObject waypointBaseObject;
        public static Dictionary<char, Drone> dronesDict;
        public static Drone selectedDrone;
        public static char nextDroneId;
        public static GameObject worldObject; // Refers to the ground
        public static Vector3 actualScale;
        public static Vector3 currentScale;
        private static float maxHeight;

        // Use this for initialization
        void Start()
        {
            dronesDict = new Dictionary<char, Drone>(); // Collection of all the drone classObjects
            nextDroneId = 'A'; // Used as an incrementing key for the dronesDict and for a piece of the communication about waypoints across the ROSBridge
            worldObject = gameObject;
            actualScale = new Vector3(1, 1, 1);
            currentScale = new Vector3(1, 1, 1);
            maxHeight = 5;
        }

        /// <summary>
        /// Returns the maximum height that a waypoint can be placed at
        /// </summary>
        /// <returns></returns>
        public static float GetMaxHeight()
        {
            return (maxHeight * (actualScale.y)) + worldObject.transform.position.y;
        }

        public void newDrone()
        {
            Drone newDrone = new Drone(worldObject.transform.position + new Vector3(0, 0.5f, 0));
        }
    }
}
