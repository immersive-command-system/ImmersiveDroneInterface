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
        public GameObject torus;
        public GameObject cart;

        public static Shader clipShader;
        public static Dictionary<char, Drone> dronesDict;
        public static Drone selectedDrone;
        public static char nextDroneId;
        public static GameObject worldObject; // Refers to the ground
        public static Vector3 actualScale;
        public static Vector3 currentScale;
        public static Vector3 droneModelOffset;
        private static float maxHeight;

        // Use this for initialization
        void Start()
        {
            selectedDrone = null;
            dronesDict = new Dictionary<char, Drone>(); // Collection of all the drone classObjects
            nextDroneId = 'A'; // Used as an incrementing key for the dronesDict and for a piece of the communication about waypoints across the ROSBridge
            worldObject = gameObject;
            actualScale = new Vector3(1, 1, 1);
            currentScale = new Vector3(1, 1, 1);
            droneModelOffset = new Vector3(0.0044f, -0.0388f, 0.0146f);
            maxHeight = 5;
            clipShader = GameObject.FindWithTag("Ground").GetComponent<Renderer>().material.shader;
            NewDrone();
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
                selectedDrone = newDrone;
            }
        }

        /// <summary>
        /// Converts the worldPosition vector to the ROSPosition vector
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 WorldSpaceToRosSpace(Vector3 worldPosition)
        {
            return new Vector3(
                -worldPosition.x,
                -worldPosition.z,
                worldPosition.y - 0.148f
                );
        }

        /// <summary>
        /// Converts the ROSPosition to WorldPosition
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 RosSpaceToWorldSpace(Vector3 ROSPosition)
        {
            return new Vector3(
                -ROSPosition.x,
                ROSPosition.z + 0.148f,
                -ROSPosition.y
                );
        }

        /// <summary>
        /// Converts the ROSPosition to WorldPosition
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 RosSpaceToWorldSpace(float pose_x, float pose_y, float pose_z)
        {
            return new Vector3(
                -pose_x,
                pose_z + 0.148f,
                -pose_y
                );
        }

        /// <summary>
        /// Converts the ROSRotation to a yaw angle
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static float RosRotationToWorldYaw(float pose_x_rot, float pose_y_rot, float pose_z_rot, float pose_w_rot)
        {
            Quaternion q = new Quaternion(
                pose_x_rot,
                pose_y_rot,
                pose_z_rot,
                pose_w_rot
                );

            float sqw = q.w * q.w;
            float sqz = q.z * q.z;
            float yaw = 57.2958f * (float)Mathf.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (sqz + sqw));

            Debug.Log(yaw);
            return yaw;
        }
    }
}
