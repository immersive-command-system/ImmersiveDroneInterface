namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This is the only class that should have static variables or functions that are consistent throughout the entire program.
    /// </summary>
    public class WorldProperties : MonoBehaviour
    {
        public GameObject droneBaseObject;
        public GameObject waypointBaseObject;
        public static Shader clipShader;
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
            clipShader = GameObject.FindWithTag("Ground").GetComponent<Renderer>().material.shader;
        }

        /// <summary>
        /// Returns the maximum height that a waypoint can be placed at
        /// </summary>
        /// <returns></returns>
        public static float GetMaxHeight()
        {
            return (maxHeight * (actualScale.y)) + worldObject.transform.position.y;
        }

        /// <summary>
        /// Recursively adds the clipShader to the parent and all its children
        /// </summary>
        /// <param name="parent">The topmost container of the objects which will have the shader added to them</param>
        public static void AddClipShader(Transform parent)
        {
            if (parent.GetComponent<Renderer>())
            {
                parent.GetComponent<Renderer>().material.shader = clipShader;
            }

            foreach (Transform child in parent)
            {
                AddClipShader(child);
            }
        }
        
        /// <summary>
        /// Creates a new drone
        /// </summary>
        public static void NewDrone()
        {
            if (!GameObject.FindWithTag("Drone"))
            {
                Drone newDrone = new Drone(worldObject.transform.position + new Vector3(0, 0.1f, 0));
            }
        }
    }
}
